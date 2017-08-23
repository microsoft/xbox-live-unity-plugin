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

    public class MockSocialManager : ISocialManager
    {
        private static Random rng = new Random();
        private List<SocialEvent> events;
        private static readonly List<SocialEvent> emptyEventsList = new List<SocialEvent>();

        internal MockSocialManager()
        {
            this.events = new List<SocialEvent>();
            this.LocalUsers = new List<XboxLiveUser>();
        }

        public IList<XboxLiveUser> LocalUsers { get; private set; }

        public Task AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            this.LocalUsers.Add(user);
            this.events.Add(new SocialEvent(SocialEventType.LocalUserAdded, user, null));
            return Task.FromResult(true);
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            this.LocalUsers.Remove(user);
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, List<ulong> userIds)
        {
            var group = new XboxSocialUserGroup(user, userIds);

            // Create 'real' users for the userIds
            var users = userIds
                .Select(CreateUser)
                .ToDictionary(u => u.XboxUserId);

            group.InitializeGroup(users.Values);
            group.UpdateView(users, new List<SocialEvent>());
            this.events.Add(new SocialEvent(SocialEventType.SocialUserGroupLoaded, user, null, group));

            return group;
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            var group = new XboxSocialUserGroup(user, presenceFilter, relationshipFilter, XboxLiveAppConfiguration.Instance.TitleId);

            var users = Enumerable.Range(0, 5)
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

            group.InitializeGroup(users.Values);
            group.UpdateView(users, new List<SocialEvent>());
            this.events.Add(new SocialEvent(SocialEventType.SocialUserGroupLoaded, user, null, group));

            return group;
        }

        private void InitUserForOfflinePresence(ref XboxSocialUser groupUser)
        {
            groupUser.PresenceState = UserPresenceState.Offline;
            groupUser.TitleHistory = new TitleHistory
            {
                HasUserPlayed = true,
                LastTimeUserPlayed = DateTime.UtcNow.AddDays(-1),
            };
        }

        private void InitUserForOnlinePresence(ref XboxSocialUser groupUser)
        {
            groupUser.PresenceState = UserPresenceState.Online;
            groupUser.PresenceDetails = new List<SocialManagerPresenceTitleRecord>
            {
                new SocialManagerPresenceTitleRecord
                {
                    TitleId = XboxLiveAppConfiguration.Instance.TitleId,
                    IsTitleActive = true,
                }
            };
        }

        public IList<SocialEvent> DoWork()
        {
            List<SocialEvent> returnList = null;
            if (this.events.Count > 0)
            {
                returnList = this.events;
                this.events = new List<SocialEvent>();
            }
            else
            {
                returnList = emptyEventsList;
            }
            return returnList;
        }

        private static XboxSocialUser CreateUser(ulong id = 0)
        {
            if (id == 0)
            {
                id = (ulong)(rng.NextDouble() * ulong.MaxValue);
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