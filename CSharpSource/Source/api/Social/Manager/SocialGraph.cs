// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Presence;
    using Microsoft.Xbox.Services.RealTimeActivity;

    internal class SocialGraph : IDisposable
    {
        private const uint MaxEventsPerFrame = 5;

        private static readonly TimeSpan RefreshDuration = TimeSpan.FromSeconds(30);

        private readonly XboxLiveUser localUser;
        private readonly SocialManagerExtraDetailLevel detailLevel;

        private readonly InternalEventQueue internalEventQueue = new InternalEventQueue();
        private EventQueue eventQueue = new EventQueue();
        private UserBuffersHolder userBuffer;

        private SocialGraphState socialGraphState;
        private int numEventsThisFrame;

        private CancellationTokenSource backgroundTaskCancellationTokenSource;

        private bool isPollingRichPresence;
        private CancellationTokenSource richPresenceCancellationTokenSource;

        private readonly ReaderWriterLockSlim refreshLock = new ReaderWriterLockSlim();

        // TODO: Move this into a more generic spot so we don't create an instance per user.
        private PeopleHubService peopleHubService;

        private bool disposed;

        public SocialGraph(XboxLiveUser localUser, SocialManagerExtraDetailLevel detailLevel)
        {
            this.localUser = localUser;
            this.detailLevel = detailLevel;

            this.peopleHubService = new PeopleHubService();
        }

        public bool IsInitialized { get; private set; }

        public XboxLiveUser LocalUser
        {
            get
            {
                return this.localUser;
            }
        }

        public Dictionary<ulong, XboxSocialUser> ActiveBufferSocialGraph
        {
            get
            {
                return this.userBuffer.Active.SocialUserGraph;
            }
        }

        public IEnumerable<XboxSocialUser> ActiveUsers
        {
            get
            {
                return this.userBuffer.Active.SocialUserGraph.Values;
            }
        }

        public Task Initialize()
        {
            if (this.IsInitialized)
            {
                throw new InvalidOperationException("Unable to initialize SocialGraph twice.");
            }

            var getProfileTask = this.peopleHubService.GetProfileInfo(this.localUser, this.detailLevel);
            var getGraphTask = this.peopleHubService.GetSocialGraph(this.localUser, this.detailLevel);

            return Task.WhenAll(getProfileTask, getGraphTask).ContinueWith(getTasks =>
            {
                if (getProfileTask.IsFaulted)
                {
                    throw new XboxException("PeopleHub call failed with " + getProfileTask.Exception);
                }

                if (getGraphTask.IsFaulted)
                {
                    throw new XboxException("PeopleHub call failed with " + getGraphTask.Exception);
                }

                // Wait for the task to throw any exceptions.
                getTasks.Wait();

                List<XboxSocialUser> users = getGraphTask.Result;
                users.Add(getProfileTask.Result);

                this.userBuffer = new UserBuffersHolder(users);

                // Kickoff the background tasks.
                this.backgroundTaskCancellationTokenSource = new CancellationTokenSource();
                this.RefreshGraphAsync(this.backgroundTaskCancellationTokenSource.Token);
                this.ProcessEventsAsync(this.backgroundTaskCancellationTokenSource.Token);

                this.IsInitialized = true;
            });
        }

        public Task AddUsers(IList<ulong> users)
        {
            InternalSocialEvent socialEvent = new InternalSocialEvent(InternalSocialEventType.UsersAdded, users);
            this.internalEventQueue.Enqueue(socialEvent);

            return socialEvent.Task;
        }

        public Task RemoveUsers(IList<ulong> users)
        {
            InternalSocialEvent socialEvent = new InternalSocialEvent(InternalSocialEventType.UsersRemoved, users);
            this.internalEventQueue.Enqueue(socialEvent);

            return socialEvent.Task;
        }

        public Task RemoveUsers(IList<XboxSocialUser> users)
        {
            InternalSocialEvent socialEvent = new InternalSocialEvent(InternalSocialEventType.UsersRemoved, users);
            this.internalEventQueue.Enqueue(socialEvent);

            return socialEvent.Task;
        }

        /// <summary>
        /// Process all events for this social graph
        /// </summary>
        /// <param name="events"></param>
        public void DoWork(List<SocialEvent> events)
        {
            this.refreshLock.EnterWriteLock();
            try
            {
                this.numEventsThisFrame = 0;

                if (this.socialGraphState == SocialGraphState.Normal)
                {
                    this.userBuffer.SwapIfEmpty();

                    EventQueue currentQueue = Interlocked.CompareExchange(ref this.eventQueue, new EventQueue(), this.eventQueue);

                    if (currentQueue.Count > 0)
                    {
                        events.AddRange(currentQueue);
                    }
                }
            }
            finally
            {
                this.refreshLock.ExitWriteLock();
            }
        }

        private void EnableRichPresencePolling(bool shouldEnablePolling)
        {
            bool wasEnabled = this.isPollingRichPresence;
            this.isPollingRichPresence = shouldEnablePolling;

            if (wasEnabled)
            {
                this.richPresenceCancellationTokenSource.Cancel();
            }

            if (shouldEnablePolling)
            {
                this.richPresenceCancellationTokenSource = new CancellationTokenSource();
                this.RefreshPresenceAsync(this.richPresenceCancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// Background task to refresh the graph.  This would be great to do in a loop but there's not
        /// an easy way to do so without await, so we'll go ahead with the current pattern.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private void RefreshGraphAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.peopleHubService.GetSocialGraph(this.localUser, this.detailLevel)
                .ContinueWith(t =>
                {
                    try
                    {
                        if (!t.IsFaulted)
                        {
                            this.refreshLock.EnterWriteLock();
                            this.socialGraphState = SocialGraphState.Refresh;

                            List<XboxSocialUser> userRefreshList = new List<XboxSocialUser>();
                            foreach (XboxSocialUser graphUser in this.userBuffer.Inactive.SocialUserGraph.Values)
                            {
                                if (!graphUser.IsFollowedByCaller)
                                {
                                    userRefreshList.Add(graphUser);
                                }
                            }

                            // TODO: We have some RTA triggers to called based on the userRefreshList.

                            // Regardless, we can perform the diff which will give us any change events.
                            this.PerformDiff(t.Result);
                        }
                    }
                    finally
                    {
                        this.socialGraphState = SocialGraphState.Normal;
                        this.refreshLock.ExitWriteLock();
                        // Setup another refresh for the future.
                        Task nextRefresh = Task.Delay(RefreshDuration).ContinueWith(
                            delayTask => this.RefreshGraphAsync(cancellationToken),
                            cancellationToken);
                    }
                });
        }

        private void RefreshPresenceAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.refreshLock.EnterWriteLock();
            try
            {
                this.socialGraphState = SocialGraphState.Refresh;

                // TODO: Fire a presence refresh for these users.
                List<ulong> userIds = this.userBuffer.Inactive.SocialUserGraph.Keys.ToList();
            }
            finally
            {
                this.socialGraphState = SocialGraphState.Normal;
                this.refreshLock.ExitWriteLock();

                // Setup another refresh for the future.
                // TODO: Make this delay the correct value.  Should be something based on RTA.
                Task.Delay(RefreshDuration).ContinueWith(
                    delayTask => this.RefreshPresenceAsync(cancellationToken),
                    cancellationToken);
            }
        }

        private void ProcessEventsAsync(CancellationToken cancellationToken)
        {
            while (this.ProcessNextEvent())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }

            // Queue up a processing task again.
            Task.Delay(TimeSpan.FromMilliseconds(30)).ContinueWith(
                _ => this.ProcessEventsAsync(cancellationToken),
                cancellationToken);
        }

        /// <summary>
        /// Process the next event that's available and return true if there are more events to process.
        /// </summary>
        /// <returns>True if there are more events to process.</returns>
        private bool ProcessNextEvent()
        {
            bool hasRemainingEvent = false;
            bool hasCachedEvents = false;

            this.refreshLock.EnterWriteLock();
            try
            {
                this.socialGraphState = SocialGraphState.EventProcessing;
                hasCachedEvents = this.IsInitialized && !userBuffer.Inactive.EventQueue.IsEmpty;
                if (hasCachedEvents)
                {
                    ProcessCachedEvents();
                    hasRemainingEvent = true;
                }
                else if (this.IsInitialized)
                {
                    this.socialGraphState = SocialGraphState.Normal;
                    hasRemainingEvent = ProcessEvents();
                }
                else
                {
                    this.socialGraphState = SocialGraphState.Normal;
                }
            }
            finally
            {
                this.refreshLock.ExitWriteLock();
            }

            return hasRemainingEvent;
        }

        private bool ProcessEvents()
        {
            if (this.numEventsThisFrame < MaxEventsPerFrame)
            {
                InternalSocialEvent internalEvent;
                if (this.internalEventQueue.TryDequeue(out internalEvent))
                {
                    Interlocked.Increment(ref this.numEventsThisFrame);
                    this.ApplyEvent(internalEvent, true);
                    internalEvent.ProcessEvent();
                }
            }

            return !this.internalEventQueue.IsEmpty && this.numEventsThisFrame < MaxEventsPerFrame;
        }

        private void ProcessCachedEvents()
        {
            InternalSocialEvent internalEvent;
            if (this.userBuffer.Inactive.EventQueue.TryDequeue(out internalEvent))
            {
                this.ApplyEvent(internalEvent, false);
            }

            this.socialGraphState = SocialGraphState.Normal;
        }

        private void PerformDiff(List<XboxSocialUser> xboxSocialUsers)
        {
            try
            {
                this.socialGraphState = SocialGraphState.Diff;

                List<XboxSocialUser> usersAddedList = new List<XboxSocialUser>();
                List<XboxSocialUser> usersRemovedList = new List<XboxSocialUser>();
                List<XboxSocialUser> presenceChangeList = new List<XboxSocialUser>();
                List<XboxSocialUser> socialRelationshipChangeList = new List<XboxSocialUser>();
                List<XboxSocialUser> profileChangeList = new List<XboxSocialUser>();

                foreach (XboxSocialUser currentUser in xboxSocialUsers)
                {
                    XboxSocialUser existingUser;
                    if (!this.userBuffer.Inactive.SocialUserGraph.TryGetValue(currentUser.XboxUserId, out existingUser))
                    {
                        usersAddedList.Add(currentUser);
                        continue;
                    }

                    var changes = existingUser.GetChanges(currentUser);
                    if (changes.HasFlag(ChangeListType.ProfileChange))
                    {
                        profileChangeList.Add(currentUser);
                    }

                    if (changes.HasFlag(ChangeListType.SocialRelationshipChange))
                    {
                        socialRelationshipChangeList.Add(currentUser);
                    }

                    if (changes.HasFlag(ChangeListType.PresenceChange))
                    {
                        presenceChangeList.Add(currentUser);
                    }
                }

                foreach (XboxSocialUser socialUser in this.userBuffer.Inactive.SocialUserGraph.Values)
                {
                    if (socialUser.XboxUserId.ToString() == this.localUser.XboxUserId)
                    {
                        continue;
                    }

                    if (!xboxSocialUsers.Contains(socialUser, XboxSocialUserIdEqualityComparer.Instance))
                    {
                        usersRemovedList.Add(socialUser);
                    }
                }

                if (usersAddedList.Count > 0)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.UsersChanged, usersAddedList);
                }
                if (usersRemovedList.Count > 0)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.UsersRemoved, usersRemovedList);
                }
                if (presenceChangeList.Count > 0)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.PresenceChanged, presenceChangeList);
                }
                if (profileChangeList.Count > 0)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.ProfilesChanged, profileChangeList);
                }
                if (socialRelationshipChangeList.Count > 0)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.SocialRelationshipsChanged, socialRelationshipChangeList);
                }
            }
            finally
            {
                this.socialGraphState = SocialGraphState.Normal;
            }
        }

        private void ApplyEvent(InternalSocialEvent internalEvent, bool isFreshEvent)
        {
            UserBuffer inactiveBuffer = this.userBuffer.Inactive;
            switch (internalEvent.Type)
            {
                case InternalSocialEventType.UsersAdded:
                {
                    this.ApplyUsersAddedEvent(internalEvent, isFreshEvent);
                    break;
                }
                case InternalSocialEventType.UsersChanged:
                {
                    this.ApplyUsersChangeEvent(internalEvent, isFreshEvent);
                    break;
                }
                case InternalSocialEventType.UsersRemoved:
                {
                    this.ApplyUsersRemovedEvent(internalEvent, isFreshEvent);
                    break;
                }
                case InternalSocialEventType.DevicePresenceChanged:
                {
                    this.ApplyDevicePresenceChangedEvent(internalEvent, isFreshEvent);
                    break;
                }
                case InternalSocialEventType.TitlePresenceChanged:
                {
                    this.ApplyTitlePresenceChangedEvent(internalEvent, isFreshEvent);
                    break;
                }
                case InternalSocialEventType.PresenceChanged:
                {
                    this.ApplyPresenceChangedEvent(internalEvent, inactiveBuffer, isFreshEvent);
                    break;
                }
                case InternalSocialEventType.SocialRelationshipsChanged:
                case InternalSocialEventType.ProfilesChanged:
                {
                    foreach (var affectedUser in internalEvent.UsersAffected)
                    {
                        inactiveBuffer.SocialUserGraph[affectedUser.XboxUserId] = affectedUser;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException("internalEvent", internalEvent.Type, "Unknown event type.");
            }

            if (isFreshEvent)
            {
                this.eventQueue.Enqueue(internalEvent, this.localUser);
                this.userBuffer.AddEvent(internalEvent);
            }
        }

        private void ApplyUsersAddedEvent(InternalSocialEvent socialEvent, bool isFreshEvent)
        {
            List<XboxSocialUser> usersAdded = new List<XboxSocialUser>();

            if (socialEvent.UsersAffected == null)
            {
                Debug.WriteLine("Unable to apply UsersAdded event because we only have userIds.");
                return;
            }

            foreach (XboxSocialUser socialUser in socialEvent.UsersAffected)
            {
                if (this.userBuffer.Inactive.TryAdd(socialUser))
                {
                    usersAdded.Add(socialUser);
                }
            }

            if (usersAdded.Count > 0)
            {
                this.userBuffer.AddUsers(usersAdded);

                if (isFreshEvent)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.UsersAdded, usersAdded);
                }
            }
        }

        private void ApplyUsersChangeEvent(InternalSocialEvent socialEvent, bool isFreshEvent)
        {
            List<XboxSocialUser> usersToAdd = new List<XboxSocialUser>();
            List<XboxSocialUser> usersToChange = new List<XboxSocialUser>();

            foreach (XboxSocialUser user in socialEvent.UsersAffected)
            {
                if (this.userBuffer.Inactive.SocialUserGraph.ContainsValue(user))
                {
                    usersToChange.Add(user);
                }
                else
                {
                    usersToAdd.Add(user);
                }
            }

            if (usersToAdd.Count > 0)
            {
                this.userBuffer.AddUsers(usersToAdd);

                if (isFreshEvent)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.UsersAdded, usersToAdd);
                }
            }

            if (usersToChange.Count > 0 && isFreshEvent)
            {
                this.internalEventQueue.Enqueue(InternalSocialEventType.ProfilesChanged, usersToChange);
            }
        }

        private void ApplyUsersRemovedEvent(InternalSocialEvent socialEvent, bool isFreshEvent)
        {
            List<XboxSocialUser> usersToRemove = new List<XboxSocialUser>();

            foreach (XboxSocialUser user in socialEvent.UsersAffected)
            {
                if (this.userBuffer.Inactive.SocialUserGraph.ContainsValue(user))
                {
                    usersToRemove.Add(user);
                }
            }

            if (usersToRemove.Count > 0)
            {
                this.userBuffer.RemoveUsers(usersToRemove);

                if (isFreshEvent)
                {
                    this.internalEventQueue.Enqueue(InternalSocialEventType.UsersRemoved, usersToRemove);
                }
            }
        }

        private void ApplyDevicePresenceChangedEvent(InternalSocialEvent socialEvent, bool isFreshEvent)
        {
            throw new NotImplementedException();
        }

        private void ApplyTitlePresenceChangedEvent(InternalSocialEvent socialEvent, bool isFreshEvent)
        {
            var titlePresenceChanged = socialEvent.TitlePresenceArgs;
            var xuid = Convert.ToUInt64(titlePresenceChanged.XboxUserId);

            var eventUser = this.userBuffer.Inactive.SocialUserGraph[xuid];
            if (eventUser != null)
            {
                if (titlePresenceChanged.TitleState == TitlePresenceState.Ended)
                {
                    var titleRecord = eventUser.PresenceDetails.FirstOrDefault(r => r.TitleId == titlePresenceChanged.TitleId);
                    eventUser.PresenceDetails.Remove(titleRecord);
                }
            }
        }

        private void ApplyPresenceChangedEvent(InternalSocialEvent socialEvent, UserBuffer inactiveBuffer, bool isFreshEvent)
        {
            List<XboxSocialUser> usersToAddList = new List<XboxSocialUser>();

            foreach (XboxSocialUser user in socialEvent.UsersAffected)
            {
                IList<SocialManagerPresenceTitleRecord> presenceDetails = user.PresenceDetails;
                UserPresenceState presenceState = user.PresenceState;

                var xuid = Convert.ToUInt64(user.XboxUserId);
                var eventUser = this.userBuffer.Inactive.SocialUserGraph[xuid];

                if (eventUser != null)
                {
                    if  ((eventUser.PresenceState != presenceState) ||
                        (eventUser.PresenceDetails != null && presenceDetails != null &&
                         eventUser.PresenceDetails.Count > 0 && presenceDetails.Count > 0 && 
                         !eventUser.PresenceDetails.All(record => presenceDetails.Contains(record))))
                    {
                        eventUser.PresenceDetails = presenceDetails;
                        eventUser.PresenceState = presenceState;
                        usersToAddList.Add(eventUser);
                    }
                }
            }

            if (usersToAddList.Count > 0 && isFreshEvent)
            {
                this.internalEventQueue.Enqueue(InternalSocialEventType.PresenceChanged, usersToAddList);
            }
        }

        private void presence_timer_callback(IList<string> users)
        {
            throw new NotImplementedException();
        }

        private Task<IList<XboxSocialUser>> social_graph_timer_callback(IList<string> users, object completionContext)
        {
            TaskCompletionSource<IList<XboxSocialUser>> tcs = new TaskCompletionSource<IList<XboxSocialUser>>();

            this.peopleHubService.GetSocialGraph(this.localUser, this.detailLevel, users).ContinueWith(getSocialGraphTask =>
            {
                if (getSocialGraphTask.IsFaulted)
                {
                    tcs.SetException(getSocialGraphTask.Exception);
                    return;
                }

                List<XboxSocialUser> socialUsers = getSocialGraphTask.Result;
                this.internalEventQueue.Enqueue(InternalSocialEventType.UsersChanged, socialUsers).ContinueWith(enqueueTask =>
                {
                    if (enqueueTask.IsFaulted)
                    {
                        tcs.SetException(enqueueTask.Exception);
                        return;
                    }

                    tcs.SetResult(socialUsers);
                });
            });

            return tcs.Task;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.backgroundTaskCancellationTokenSource.Cancel();
                this.disposed = true;
            }
        }
    }
}