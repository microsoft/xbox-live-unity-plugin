// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Presence;
    using Microsoft.Xbox.Services.System;

    // todo update this method
    public class MockSocialManager : ISocialManager
    {
        static Random rng = new Random();
        List<XboxLiveUser> mLocalUsers;
        List<SocialEvent> mEvents;

        internal MockSocialManager()
        {
            mLocalUsers = new List<XboxLiveUser>();
            mEvents = new List<SocialEvent>();
        }

        public IList<XboxLiveUser> LocalUsers
        {
            get
            {
                return mLocalUsers.AsReadOnly();
            }
        }

        public void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel = SocialManagerExtraDetailLevel.NoExtraDetail)
        {
            mLocalUsers.Add(user);
            mEvents.Add(new SocialEvent(SocialEventType.LocalUserAdded, user));
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {

            Dictionary<string, XboxSocialUser> users = Enumerable.Range(0, 5)
                .Select(id =>
                {
                    var groupUser = CreateUser();

                    switch (presenceFilter)
                    {
                        case PresenceFilter.AllOnline:
                        case PresenceFilter.TitleOnline:
                            InitUserForOnlinePresence(ref groupUser);
                            break;

                        case PresenceFilter.AllOffline:
                        case PresenceFilter.TitleOffline:
                            InitUserForOfflinePresence(ref groupUser);
                            break;

                        case PresenceFilter.AllTitle:
                        case PresenceFilter.All:
                            if (id % 2 == 0)
                            {
                                InitUserForOnlinePresence(ref groupUser);
                            }
                            else
                            {
                                InitUserForOfflinePresence(ref groupUser);
                            }
                            break;
                    }

                    switch (relationshipFilter)
                    {
                        case RelationshipFilter.Friends:
                            groupUser.IsFollowedByCaller = true;
                            break;
                        case RelationshipFilter.Favorite:
                            groupUser.IsFollowedByCaller = true;
                            groupUser.IsFavorite = true;
                            break;
                    }

                    return groupUser;
                }).ToDictionary(u => u.XboxUserId);

            XboxSocialUserGroup group = new XboxSocialUserGroup(user, SocialUserGroupType.Filter, presenceFilter, relationshipFilter, users);
            mEvents.Add(new SocialEvent(SocialEventType.SocialUserGroupLoaded, user, null, group));

            return group;
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IList<string> xboxUserIdList)
        {
            Dictionary<string, XboxSocialUser> users = xboxUserIdList
                .Select(CreateUser)
                .ToDictionary(u => u.XboxUserId);

            XboxSocialUserGroup group = new XboxSocialUserGroup(
                    user, 
                    SocialUserGroupType.UserList, 
                    PresenceFilter.Unknown, 
                    RelationshipFilter.Friends, 
                    users);

            mEvents.Add(new SocialEvent(SocialEventType.SocialUserGroupLoaded, user, null, group));

            return group;
        }

        public void DestroySocialUserGroup(XboxSocialUserGroup group)
        {
            // do nothing
        }

        public IList<SocialEvent> DoWork()
        {
            List<SocialEvent> currentEvents = mEvents;
            mEvents = new List<SocialEvent>();
            return currentEvents;
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            mLocalUsers.Remove(user);
            mEvents.Add(new SocialEvent(SocialEventType.LocalUserRemoved, user));
        }

        public void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling)
        {
            // do nothing
        }

        public void UpdateSocialUserGroup(XboxSocialUserGroup group, IList<string> userIds)
        {
            Dictionary<string, XboxSocialUser> users = 
                userIds
                .Select(CreateUser)
                .ToDictionary(u => u.XboxUserId);

            group.UpdateGroup(users);

            mEvents.Add(new SocialEvent(SocialEventType.SocialUserGroupUpdated, group.LocalUser, null, group));
        }


        // Mock Helpers

        private void InitUserForOfflinePresence(ref XboxSocialUser groupUser)
        {
            groupUser.PresenceRecord = new SocialManagerPresenceRecord(UserPresenceState.Offline, null);
            groupUser.TitleHistory = new TitleHistory
            {
                HasUserPlayed = true,
                LastTimeUserPlayed = DateTime.UtcNow.AddDays(-1),
            };
        }

        private void InitUserForOnlinePresence(ref XboxSocialUser groupUser)
        {
            List<SocialManagerPresenceTitleRecord> titles = new List<SocialManagerPresenceTitleRecord>
            {
                new SocialManagerPresenceTitleRecord
                {
                    TitleId = XboxLiveAppConfiguration.SingletonInstance.TitleId,
                    IsTitleActive = true,
                }
            };

            groupUser.PresenceRecord = new SocialManagerPresenceRecord(UserPresenceState.Online, titles);
        }

        private static XboxSocialUser CreateUser(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                id = (rng.NextDouble() * ulong.MaxValue).ToString();
            }

            return new XboxSocialUser
            {
                XboxUserId = id,
                Gamertag = "Gamer" + id,
                DisplayName = "User " + id,
                RealName = "Real User " + id,
                Gamerscore = id.ToString(),
                DisplayPicRaw = "http://images-eds.xboxlive.com/image?url=z951ykn43p4FqWbbFvR2Ec.8vbDhj8G2Xe7JngaTToBrrCmIEEXHC9UNrdJ6P7KI4AAOijCgOA3.jozKovAH98vieJP1ResWJCw2dp82QtambLRqzQbSIiqrCug0AvP4&format=png",
                PreferredColor = new PreferredColor
                {
                    PrimaryColor = "1081ca",
                    SecondaryColor = "10314f",
                    TertiaryColor = "105080"
                },
            };
        }
    }
}