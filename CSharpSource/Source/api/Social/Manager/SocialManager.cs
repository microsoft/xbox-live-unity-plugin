// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.System;

    public class SocialManager : ISocialManager
    {
        private static ISocialManager instance;

        private static readonly object instanceLock = new object();
        private readonly object syncRoot = new object();

        private readonly List<XboxLiveUser> localUsers = new List<XboxLiveUser>();
        private readonly Dictionary<XboxLiveUser, SocialGraph> userGraphs = new Dictionary<XboxLiveUser, SocialGraph>(new XboxUserIdEqualityComparer());
        private readonly Dictionary<XboxLiveUser, HashSet<WeakReference>> userGroupsMap = new Dictionary<XboxLiveUser, HashSet<WeakReference>>(new XboxUserIdEqualityComparer());

        private Queue<SocialEvent> eventQueue = new Queue<SocialEvent>();
        private static readonly List<SocialEvent> emptyEventList = new List<SocialEvent>();

        private SocialManager()
        {
        }

        internal static ISocialManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = XboxLive.UseMockServices ? new MockSocialManager() : (ISocialManager)new SocialManager();
                        }
                    }
                }
                return instance;
            }
        }

        public IList<XboxLiveUser> LocalUsers
        {
            get
            {
                return this.localUsers.AsReadOnly();
            }
        }

        public Task AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            if (user == null) throw new ArgumentNullException("user");

            if (this.userGraphs.ContainsKey(user))
            {
                throw new XboxException("User already exists in graph.");
            }

            SocialGraph graph = new SocialGraph(user, extraDetailLevel);
            return graph.Initialize().ContinueWith(
                initializeTask =>
                {
                    if (initializeTask.IsFaulted)
                    {
                        this.eventQueue.Enqueue(new SocialEvent(SocialEventType.LocalUserAdded, user, null, null, initializeTask.Exception));
                        return;
                    }

                    // Wait on the task to throw an exceptions.
                    initializeTask.Wait();

                    lock (this.syncRoot)
                    {
                        this.userGraphs[user] = graph;
                        this.localUsers.Add(user);

                        this.eventQueue.Enqueue(new SocialEvent(SocialEventType.LocalUserAdded, user));
                    }
                });
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            lock (this.syncRoot)
            {
                this.localUsers.Remove(user);
                this.userGraphs.Remove(user);

                this.eventQueue.Enqueue(new SocialEvent(SocialEventType.LocalUserRemoved, user));
            }
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            if (user == null) throw new ArgumentNullException("user");

            SocialGraph userGraph;
            if (!this.userGraphs.TryGetValue(user, out userGraph))
            {
                throw new ArgumentException("You must add a local user before you can create a social group for them.", "user");
            }

            XboxSocialUserGroup group = new XboxSocialUserGroup(user, presenceFilter, relationshipFilter, XboxLiveAppConfiguration.Instance.TitleId);
            if (userGraph.IsInitialized)
            {
                group.InitializeGroup(userGraph.ActiveUsers);
            }

            this.AddUserGroup(user, group);

            this.eventQueue.Enqueue(new SocialEvent(SocialEventType.SocialUserGroupLoaded, user, null, group));

            return group;
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, List<ulong> userIds)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (userIds == null) throw new ArgumentNullException("userIds");

            SocialGraph userGraph;
            if (!this.userGraphs.TryGetValue(user, out userGraph))
            {
                throw new ArgumentException("You must add a local user before you can create a social group for them.", "user");
            }

            XboxSocialUserGroup group = new XboxSocialUserGroup(user, userIds);
            if (userGraph.IsInitialized)
            {
                group.InitializeGroup(userGraph.ActiveUsers);
            }

            this.AddUserGroup(user, group);

            userGraph.AddUsers(userIds).ContinueWith(addUsersTask =>
            {
                this.eventQueue.Enqueue(new SocialEvent(SocialEventType.SocialUserGroupLoaded, user, userIds, group, addUsersTask.Exception));
            });

            return group;
        }

        private void AddUserGroup(XboxLiveUser user, XboxSocialUserGroup group)
        {
            lock (this.syncRoot)
            {
                HashSet<WeakReference> userGroups;
                if (!this.userGroupsMap.TryGetValue(user, out userGroups))
                {
                    this.userGroupsMap[user] = userGroups = new HashSet<WeakReference>();
                }

                WeakReference groupReference = new WeakReference(group);
                userGroups.Add(groupReference);
            }
        }

        public void UpdateUserGroup(XboxLiveUser user, XboxSocialUserGroup group, List<ulong> users)
        {
            if (group.SocialUserGroupType != SocialUserGroupType.UserList)
            {
                throw new ArgumentException("You can only modify the user list for a UserList type social group.");
            }

            this.userGraphs[user].AddUsers(users).ContinueWith(
                addUsersTask =>
                {
                    SocialEvent socialEvent = new SocialEvent(SocialEventType.SocialUserGroupUpdated, user, users);
                    this.eventQueue.Enqueue(socialEvent);
                });
        }

        public IList<SocialEvent> DoWork()
        {
            List<SocialEvent> events;
            lock (this.syncRoot)
            {
                if (this.eventQueue.Count > 0)
                {
                    events = this.eventQueue.ToList();
                    this.eventQueue.Clear();
                }
                else
                {
                    events = emptyEventList;
                }

                foreach (SocialGraph graph in this.userGraphs.Values)
                {
                    graph.DoWork(events);

                    HashSet<WeakReference> userGroups;
                    if (!this.userGroupsMap.TryGetValue(graph.LocalUser, out userGroups))
                    {
                        continue;
                    }

                    // Grab the social groups for this user and update them.
                    foreach (WeakReference groupReference in userGroups.ToList())
                    {
                        XboxSocialUserGroup group = groupReference.Target as XboxSocialUserGroup;
                        // If the target is null that means the group has been disposed so we don't 
                        // need to bother updating it anymore.
                        if (group == null)
                        {
                            userGroups.Remove(groupReference);
                            continue;
                        }

                        group.UpdateView(graph.ActiveBufferSocialGraph, events);
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// Used by tests to reset the state of the SocialManager.
        /// </summary>
        internal static void Reset()
        {
            foreach (XboxLiveUser user in Instance.LocalUsers.ToList())
            {
                Instance.RemoveLocalUser(user);
            }

            instance = null;
        }
    }
}