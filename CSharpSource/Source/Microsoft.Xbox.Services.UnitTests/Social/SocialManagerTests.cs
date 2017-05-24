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
    public class SocialManagerUnitTests : TestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            MockXboxLiveData.Load(Environment.CurrentDirectory + "\\Social\\SocialManagerUT.json");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SocialManager.Reset();
        }

        [TestMethod]
        public void GetSocialManagerSingleton()
        {
            var socialManager = SocialManager.Instance;
            Assert.IsNotNull(socialManager);
        }

        [TestMethod]
        public async Task AddAndRemoveLocalUser()
        {
            await SocialManager.Instance.AddLocalUser(user, SocialManagerExtraDetailLevel.None);
            SocialManager.Instance.RemoveLocalUser(user);
        }

        [TestMethod]
        public async Task AddLocalUserWithDetail()
        {
            await SocialManager.Instance.AddLocalUser(user, SocialManagerExtraDetailLevel.PreferredColor | SocialManagerExtraDetailLevel.TitleHistory);
        }
    }
}