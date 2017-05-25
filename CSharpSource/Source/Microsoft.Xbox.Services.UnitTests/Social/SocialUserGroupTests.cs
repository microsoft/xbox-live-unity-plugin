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
            SocialManager.Instance.AddLocalUser(this.user, SocialManagerExtraDetailLevel.PreferredColor).Wait();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SocialManager.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateSocialGroupFromListWithNoUsers()
        {
            XboxSocialUserGroup group = new XboxSocialUserGroup(this.user, new List<ulong>());
        }

        [TestMethod]
        public void CreateSocialGroupFromListWithOneUser()
        {
            List<ulong> userIds = Enumerable.Range(1, 10).Select(i => (ulong)i).ToList();
            var users = userIds.Select(id => new XboxSocialUser { XboxUserId = id, Gamertag = "Gamer" + id }).ToDictionary(u => u.XboxUserId);

            XboxSocialUserGroup group = new XboxSocialUserGroup(this.user, new List<ulong> { 123456789 });
            Assert.AreEqual(0, group.Count);

            group.UpdateView(users, null);
            Assert.AreEqual(0, group.Count);

            group.UpdateView(
                new Dictionary<ulong, XboxSocialUser>
                {
                    [123456789] = new XboxSocialUser { XboxUserId = 123456789 }
                },
                null);
            Assert.AreEqual(1, group.Count);
        }

        [TestMethod]
        public void CreateSocialGroupFromListWithManyUsers()
        {
            List<ulong> userIds = Enumerable.Range(1, 20).Select(i => (ulong)i).ToList();
            var users = userIds.Select(id => new XboxSocialUser { XboxUserId = id, Gamertag = "Gamer" + id }).ToDictionary(u => u.XboxUserId);

            XboxSocialUserGroup group = new XboxSocialUserGroup(this.user, new List<ulong>(userIds));
            Assert.AreEqual(0, group.Count);

            // Update half the users
            group.UpdateView(users.Values.Where(u => u.XboxUserId % 2 == 0).ToDictionary(u => u.XboxUserId), null);
            Assert.AreEqual(10, group.Count);

            // Then update the rest.
            group.UpdateView(users, null);
            Assert.AreEqual(20, group.Count);

            // Ensure we don't accidentally double add anything.
            group.UpdateView(users, null);
            Assert.AreEqual(20, group.Count);
        }

        [TestMethod]
        public void CreateSocialGroupWithSelfFromSocialManager()
        {
            ulong userId = ulong.Parse(this.user.XboxUserId);
            var group = SocialManager.Instance.CreateSocialUserGroupFromList(this.user, new List<ulong> { userId });
            Assert.IsNotNull(group);

            DoWorkUntil(() => group.Count == 1);

            Assert.AreEqual(userId, group.First().XboxUserId);
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