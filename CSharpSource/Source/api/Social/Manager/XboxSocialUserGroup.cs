// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using Microsoft.Xbox.Services.Presence;

    public class XboxSocialUserGroup : IEnumerable<XboxSocialUser>
    {
        private const int MaxUsersFromList = 100;

        private readonly Dictionary<ulong, XboxSocialUser> users = new Dictionary<ulong, XboxSocialUser>();

        /// <summary>
        /// Initialize an <see cref="XboxSocialUserGroup" />
        /// </summary>
        /// <param name="localUser">The user that this group belongs to.</param>
        /// <param name="type">The type of SocialManager group.</param>
        private XboxSocialUserGroup(XboxLiveUser localUser, SocialUserGroupType type)
        {
            this.SocialUserGroupType = type;
        }

        /// <summary>
        /// Creates a list based XboxSocialUserGroup from a given set of user ids.
        /// </summary>
        /// <param name="localUser">The user who the group belongs to.</param>
        /// <param name="userIds">The list of users to include in the group.</param>
        internal XboxSocialUserGroup(XboxLiveUser localUser, ICollection<ulong> userIds)
            : this(localUser, SocialUserGroupType.UserList)
        {
            if (userIds == null) throw new ArgumentNullException("userIds");
            if (userIds.Count == 0) throw new ArgumentException("You must provide at least one user id to create a group.", "userIds");
            if (userIds.Count > MaxUsersFromList) throw new ArgumentException(string.Format("You cannot provide more than {0} user ides to create a group.", MaxUsersFromList), "userIds");

            foreach (var userId in userIds)
            {
                this.users[userId] = null;
            }
        }

        /// <summary>
        /// Creates a filter based XboxSocialUserGroup from a given set of filter parameters.
        /// </summary>
        /// <param name="localUser">The user who the group belongs to.</param>
        /// <param name="presenceFilter">Indicates the presence of users who should be included in the group.</param>
        /// <param name="relationshipFilter">Indicates the relationship to the local user of users who should be included in the group.</param>
        /// <param name="titleId">The title id to filter users to.</param>
        internal XboxSocialUserGroup(XboxLiveUser localUser, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter, uint titleId)
            : this(localUser, SocialUserGroupType.Filter)
        {
            this.PresenceFilter = presenceFilter;
            this.RelationshipFilter = relationshipFilter;
            this.TitleId = titleId;
        }

        public SocialUserGroupType SocialUserGroupType { get; private set; }

        public PresenceFilter PresenceFilter { get; private set; }

        public RelationshipFilter RelationshipFilter { get; private set; }

        public uint TitleId { get; set; }

        public int Count
        {
            get
            {
                return this.users.Count;
            }
        }

        public IList<XboxSocialUser> Users
        {
            get
            {
                return this.users.Values.ToList();
            }
        }

        public XboxSocialUser GetUser(ulong userId)
        {
            XboxSocialUser user;
            this.users.TryGetValue(userId, out user);
            return user;
        }

        public IEnumerator<XboxSocialUser> GetEnumerator()
        {
            return this.users.Values.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal void InitializeGroup(IEnumerable<XboxSocialUser> users)
        {
            switch (this.SocialUserGroupType)
            {
                case SocialUserGroupType.Filter:
                    foreach (XboxSocialUser user in users)
                    {

                        if (!this.CheckRelationshipFilter(user, this.RelationshipFilter)) continue;
                        if (this.CheckPresenceFilter(user, this.PresenceFilter))
                        {
                            this.AddOrUpdateUser(user);
                        }
                    }
                    break;
                case SocialUserGroupType.UserList:
                    foreach (XboxSocialUser user in users)
                    {
                        if (this.users.ContainsKey(user.XboxUserId))
                        {
                            this.AddOrUpdateUser(user);
                        }
                    }
                    break;
            }
        }

        private void AddOrUpdateUser(XboxSocialUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            this.users[user.XboxUserId] = user;
        }

        private void RemoveUser(XboxSocialUser user)
        {
            this.users.Remove(user.XboxUserId);
        }

        internal void UpdateView(Dictionary<ulong, XboxSocialUser> userSnapshot, List<SocialEvent> events)
        {
            switch (this.SocialUserGroupType)
            {
                case SocialUserGroupType.Filter:
                    this.UpdateFilteredView(userSnapshot, events);
                    break;
                case SocialUserGroupType.UserList:
                    // We need to convert the keys to a list because we're modifying the collection.
                    foreach (ulong userId in this.users.Keys.ToList())
                    {
                        XboxSocialUser user;
                        // Check if there's an updated user in the snapshot.
                        if (userSnapshot.TryGetValue(userId, out user))
                        {
                            this.AddOrUpdateUser(user);
                        }
                    }
                    break;
            }
        }

        private bool UpdateFilteredView(IDictionary<ulong, XboxSocialUser> users, IEnumerable<SocialEvent> events)
        {
            bool updated = false;
            foreach (SocialEvent socialEvent in events)
            {
                switch (socialEvent.EventType)
                {
                    case SocialEventType.PresenceChanged:
                    case SocialEventType.ProfilesChanged:
                    case SocialEventType.SocialRelationshipsChanged:
                        foreach (ulong userId in socialEvent.UsersAffected)
                        {
                            XboxSocialUser user = users[userId];

                            if (this.CheckRelationshipFilter(user, this.RelationshipFilter) && this.CheckPresenceFilter(user, this.PresenceFilter))
                            {
                                this.AddOrUpdateUser(user);
                            }
                            else
                            {
                                this.RemoveUser(user);
                            }
                        }
                        break;
                    case SocialEventType.UsersAddedToSocialGraph:
                        foreach (ulong userId in socialEvent.UsersAffected)
                        {
                            XboxSocialUser user = users[userId];

                            if (this.CheckRelationshipFilter(user, this.RelationshipFilter) && this.CheckPresenceFilter(user, this.PresenceFilter))
                            {
                                this.AddOrUpdateUser(user);
                            }
                        }
                        break;
                    case SocialEventType.UsersRemovedFromSocialGraph:
                        foreach (ulong userId in socialEvent.UsersAffected)
                        {
                            XboxSocialUser user;
                            if (this.users.TryGetValue(userId, out user))
                            {
                                this.RemoveUser(user);
                            }
                        }
                        break;
                    default:
                        continue;
                }

                updated = true;
            }

            return updated;
        }

        /// <summary>
        /// Determines if the user should be included based on their presence.
        /// </summary>
        /// <param name="user">The user whose presence should be checked.</param>
        /// <param name="presenceFilter">The presence filter to check against.</param>
        /// <returns>True if the user should be included, false otherwise.</returns>
        private bool CheckPresenceFilter(XboxSocialUser user, PresenceFilter presenceFilter)
        {
            switch (presenceFilter)
            {
                case PresenceFilter.All:
                    return true;
                case PresenceFilter.AllOffline:
                    return user.PresenceState == UserPresenceState.Offline;
                case PresenceFilter.AllOnline:
                    return user.PresenceState == UserPresenceState.Online;
                case PresenceFilter.AllTitle:
                    return user.TitleHistory.HasUserPlayed;
                case PresenceFilter.TitleOffline:
                    return user.PresenceState == UserPresenceState.Offline && user.TitleHistory.HasUserPlayed;
                case PresenceFilter.TitleOnline:
                    return user.IsUserPlayingTitle(this.TitleId);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the user should be included based on their relationship.
        /// </summary>
        /// <param name="user">The user whose relationship should be checked.</param>
        /// <param name="relationshipFilter">The relationship filter to check against.</param>
        /// <returns>True if the user should be included, false otherwise.</returns>
        private bool CheckRelationshipFilter(XboxSocialUser user, RelationshipFilter relationshipFilter)
        {
            switch (relationshipFilter)
            {
                case RelationshipFilter.Friends:
                    return user.IsFollowedByCaller;
                case RelationshipFilter.Favorite:
                    return user.IsFavorite;
                default:
                    throw new ArgumentOutOfRangeException("relationshipFilter", relationshipFilter, "Unexpected relationship filter value.");
            }
        }
    }
}