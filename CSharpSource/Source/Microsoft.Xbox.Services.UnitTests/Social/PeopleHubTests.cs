// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.UnitTests.Social
{
    using global::System;
    using global::System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Social.Manager;

    [TestClass]
    public class PeopleHubTests : TestBase
    {
        private PeopleHubService service;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            MockXboxLiveData.Load(Environment.CurrentDirectory + "\\Social\\PeopleHubUT.json");
            this.service = new PeopleHubService();
        }

        [TestMethod]
        public async Task GetProfileInfo()
        {
            var profileInfo = await this.service.GetProfileInfo(user, SocialManagerExtraDetailLevel.None);
            Assert.IsNotNull(profileInfo);
        }

        [TestMethod]
        public async Task GetProfileInfoWithPreferredColor()
        {
            var profileInfo = await this.service.GetProfileInfo(user, SocialManagerExtraDetailLevel.PreferredColor);
            Assert.IsNotNull(profileInfo);
        }

        [TestMethod]
        public async Task GetProfileInfoWithTitleHistory()
        {
            var profileInfo = await this.service.GetProfileInfo(user, SocialManagerExtraDetailLevel.TitleHistory);
            Assert.IsNotNull(profileInfo);
        }

        [TestMethod]
        public async Task GetProfileInfoWithPreferredColorAndTitleHistory()
        {
            var profileInfo = await this.service.GetProfileInfo(user, SocialManagerExtraDetailLevel.PreferredColor | SocialManagerExtraDetailLevel.TitleHistory);
            Assert.IsNotNull(profileInfo);
        }

        [TestMethod]
        public async Task GetSocialGraph()
        {
            var graph = await this.service.GetSocialGraph(user, SocialManagerExtraDetailLevel.None);
            Assert.IsNotNull(graph);
        }

        [TestMethod]
        public async Task GetSocialGraphWithDetail()
        {
            var graph = await this.service.GetSocialGraph(user, SocialManagerExtraDetailLevel.PreferredColor | SocialManagerExtraDetailLevel.TitleHistory);
            Assert.IsNotNull(graph);
        }
    }
}