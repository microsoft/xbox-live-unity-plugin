// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.UnitTests.Social
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.System;

    [TestClass]
    public class SocialUserGroupUnitTests
    {
        private XboxLiveUser user;

        [TestInitialize]
        public void TestInitialize()
        {
            this.user = new XboxLiveUser();
            MockXboxLiveData.Load(Environment.CurrentDirectory + "\\Social\\SocialUserGroupUT.json");
            SocialManager.Instance.AddLocalUser(this.user, SocialManagerExtraDetailLevel.PreferredColorLevel);
        }
        
        [TestMethod]
        public void CreateSocialGroupWithSelfFromSocialManager()
        {
            var group = SocialManager.Instance.CreateSocialUserGroupFromList(this.user, new List<string> { this.user.XboxUserId });
            Assert.IsNotNull(group);

            DoWorkUntil(() => group.Count == 1);

            Assert.AreEqual(this.user.XboxUserId, group.First().XboxUserId);
        }

        [TestMethod]
        public void CreateSocialGroupWithFilterOnlineFriendsFromSocialManager()
        {
            var group = SocialManager.Instance.CreateSocialUserGroupFromFilters(this.user, PresenceFilter.AllOnline, RelationshipFilter.Friends);
            Assert.IsNotNull(group);

            DoWorkUntil(() => group.Count > 0);

            Assert.IsTrue(group.Count > 1);
        }

        [TestMethod]
        public void CreateSocialGroupWithFilterOnlineFavoritesFromSocialManager()
        {
            var group = SocialManager.Instance.CreateSocialUserGroupFromFilters(this.user, PresenceFilter.AllOnline, RelationshipFilter.Favorite);
            Assert.IsNotNull(group);

            DoWorkUntil(() => group.Count > 0);

            Assert.IsTrue(group.Count > 1);
        }

        [TestMethod]
        public void CreateSocialGroupWithFilterOfflineFavoritesFromSocialManager()
        {
            var group = SocialManager.Instance.CreateSocialUserGroupFromFilters(this.user, PresenceFilter.AllOffline, RelationshipFilter.Favorite);
            Assert.IsNotNull(group);

            DoWorkUntil(() => group.Count > 0);

            Assert.IsTrue(group.Count > 1);
        }

        [TestMethod]
        public void CreateSocialGroupWithOthers()
        {
        }

        private static Task DoWorkUntil(Func<bool> predicate)
        {
            return DoWorkUntil(predicate, TimeSpan.FromSeconds(5));
        }

        private static async Task DoWorkUntil(Func<bool> predicate, TimeSpan maxDuration)
        {
            DateTime workUntil = DateTime.UtcNow + maxDuration;

            do
            {
                IList<SocialEvent> events = SocialManager.Instance.DoWork();
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            while (!predicate() && DateTime.UtcNow < workUntil);

            if (!predicate())
            {
                Assert.Fail("Request did not complete as expected");
            }
        }
    }
}