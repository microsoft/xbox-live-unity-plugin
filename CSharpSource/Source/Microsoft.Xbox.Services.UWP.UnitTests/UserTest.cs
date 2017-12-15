// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.UWP.UnitTests
{
    using global::System;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Windows.System;

    [TestClass]
    public class UserTest
    {
        private const string mockXuid = "123456";
        private const string mockGamerTag = "mock gamertag";
        private const string mockAgeGroup = "Adult";
        private const string mockEnvironment = "prod";
        private const string mockSandbox = "ASDFAS.1";
        private const string mockSignature = "mock signature";
        private const string mockPrivileges = "12 34 56 78 9";
        private const string mockWebAccountId = "mock web account id";
        private const string mockToken = "mock token";
        private const int mockErrorcode = 9999;
        private const string mockErrorMessage = "mock error message";

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task CreateUser()
        {
            // default constructor for SUA
            var user1 = new XboxLiveUser();

            // Create xbl user with system user
            var users = await User.FindAllAsync();
            users = users.Where(user => (user.Type != Windows.System.UserType.LocalGuest || user.Type != Windows.System.UserType.RemoteGuest)).ToList();

            Assert.IsTrue(users.Count > 0);
            var systemUser = users[0];
            // default constructor
            var xbluser = new XboxLiveUser(systemUser);
            Assert.AreEqual(systemUser.NonRoamableId, xbluser.WindowsSystemUser.NonRoamableId);
        }
    }
}
