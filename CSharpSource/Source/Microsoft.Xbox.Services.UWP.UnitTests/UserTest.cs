// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.UWP.UnitTests
{
    using global::System;
    using Windows.System;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.Xbox.Services.System;
    using global::System.Linq;
    using Moq;
    using global::System.Threading.Tasks;
    using UITestMethod = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;
    using Windows.Security.Authentication.Web.Core;
    using global::System.Collections.Generic;
    using global::System.Threading;

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

        private TokenRequestResult CreateSuccessTokenResponse()
        {
            var result = new TokenRequestResult(null);
            result.ResponseStatus = WebTokenRequestStatus.Success;
            result.Token = mockToken;
            result.WebAccountId = mockWebAccountId;
            result.Properties = new Dictionary<string, string>();
            result.Properties.Add("XboxUserId", mockXuid);
            result.Properties.Add("Gamertag", mockGamerTag);
            result.Properties.Add("AgeGroup", mockAgeGroup);
            result.Properties.Add("Environment", mockEnvironment);
            result.Properties.Add("Sandbox", mockSandbox);
            result.Properties.Add("Signature", mockSignature);
            result.Properties.Add("Privileges", mockPrivileges);

            return result;
        }

        private Mock<AccountProvider> CreateMockAccountProvider(TokenRequestResult silentResult, TokenRequestResult uiResult)
        {
            var provider = new Mock<AccountProvider>();
            if (silentResult != null)
            {
                provider
                .Setup(o => o.GetTokenSilentlyAsync(It.IsAny<WebTokenRequest>()))
                .ReturnsAsync(silentResult);
            }

            if (uiResult != null)
            {
                provider
                .Setup(o => o.RequestTokenAsync(It.IsAny<WebTokenRequest>()))
                .Callback(()=> 
                {
                    // Make sure it is called on the UI thread with a coreWindow.
                    // Calling API can only be called on UI thread.
                    var resourceContext = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView();
                })
                .ReturnsAsync(uiResult);
            }

            return provider;
        }

        [TestCleanup]
        public void Cleanup()
        {
            XboxLiveUser.CleanupEventHandler();
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

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task UserSigninSilentlySuccess()
        {
            var user = new XboxLiveUser();
            Assert.IsFalse(user.IsSignedIn);

            AutoResetEvent signinEvent = new AutoResetEvent(false);
            XboxLiveUser.SignInCompleted += (Object o, SignInCompletedEventArgs args) =>
            {
                Assert.AreEqual(args.User, user);
                signinEvent.Set();
            };
            var response = CreateSuccessTokenResponse();
            user.Impl.Provider = CreateMockAccountProvider(response, null).Object;

            // Create xbl user with system user
            var silentResult = await user.SignInSilentlyAsync();
            Assert.AreEqual(silentResult.Status, SignInStatus.Success);

            Assert.IsTrue(user.IsSignedIn);
            Assert.AreEqual(user.Gamertag, mockGamerTag);
            Assert.AreEqual(user.XboxUserId, mockXuid);
            Assert.AreEqual(user.AgeGroup, mockAgeGroup);
            Assert.AreEqual(user.Privileges, mockPrivileges);
            Assert.AreEqual(user.WebAccountId, mockWebAccountId);

            Assert.IsTrue(signinEvent.WaitOne(100), "wait signin event time out");

        }

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task UserSigninWithUiSuccess()
        {
            var user = new XboxLiveUser();
            Assert.IsFalse(user.IsSignedIn);

            AutoResetEvent signinEvent = new AutoResetEvent(false);
            XboxLiveUser.SignInCompleted += ((Object o, SignInCompletedEventArgs args) =>
            {
                Assert.AreEqual(args.User, user);
                signinEvent.Set();
            });

            var response = CreateSuccessTokenResponse();
            user.Impl.Provider = CreateMockAccountProvider(null, response).Object;

            var signinResult = await user.SignInAsync();
            Assert.AreEqual(signinResult.Status, SignInStatus.Success);
            Assert.IsTrue(user.IsSignedIn);
            Assert.AreEqual(user.Gamertag, mockGamerTag);
            Assert.AreEqual(user.XboxUserId, mockXuid);
            Assert.AreEqual(user.AgeGroup, mockAgeGroup);
            Assert.AreEqual(user.Privileges, mockPrivileges);
            Assert.AreEqual(user.WebAccountId, mockWebAccountId);

            Assert.IsTrue(signinEvent.WaitOne(100), "wait signin event time out");
        }

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task UserSigninSilentlyUserInteractionRequired()
        {
            var user = new XboxLiveUser();
            var result = new TokenRequestResult(null);
            result.ResponseStatus = WebTokenRequestStatus.UserInteractionRequired;
            user.Impl.Provider = CreateMockAccountProvider(result, null).Object;

            var signinResult = await user.SignInSilentlyAsync();
            Assert.AreEqual(signinResult.Status, SignInStatus.UserInteractionRequired);
            Assert.IsFalse(user.IsSignedIn);
        }

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task UserSigninUIUserCancel()
        {
            var user = new XboxLiveUser();
            var result = new TokenRequestResult(null);
            result.ResponseStatus = WebTokenRequestStatus.UserCancel;
            user.Impl.Provider = CreateMockAccountProvider(null, result).Object;

            var signinResult = await user.SignInAsync();
            Assert.AreEqual(signinResult.Status, SignInStatus.UserCancel);
            Assert.IsFalse(user.IsSignedIn);
        }

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task UserSigninProviderError()
        // provider error 
        {
            var user = new XboxLiveUser();
            var result = new TokenRequestResult(null);
            result.ResponseStatus = WebTokenRequestStatus.ProviderError;
            result.ResponseError = new WebProviderError(mockErrorcode, mockErrorMessage);
            user.Impl.Provider = CreateMockAccountProvider(result, result).Object;

            // ProviderError will convert to exception
            try
            {
                var silentResult = await user.SignInSilentlyAsync();
            }
            catch (XboxException ex)
            {
                Assert.AreEqual(ex.HResult, mockErrorcode);
                Assert.IsFalse(string.IsNullOrEmpty(ex.Message));
                Assert.IsFalse(user.IsSignedIn);

                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        [TestCategory("XboxLiveUser")]
        [TestMethod]
        public async Task UserSignOut()
        // provider error 
        {
            var user = new XboxLiveUser();
            Assert.IsFalse(user.IsSignedIn);

            AutoResetEvent signoutEvent = new AutoResetEvent(false);
            XboxLiveUser.SignOutCompleted += ((Object o, SignOutCompletedEventArgs args) =>
            {
                Assert.AreEqual(args.User, user);
                signoutEvent.Set();
            });

            var successResponse = CreateSuccessTokenResponse();
            var errorResponse = new TokenRequestResult(null);
            errorResponse.ResponseStatus = WebTokenRequestStatus.UserInteractionRequired;

            var provider = new Mock<AccountProvider>();
            provider
            .SetupSequence(o => o.GetTokenSilentlyAsync(It.IsAny<WebTokenRequest>()))
            .ReturnsAsync(successResponse)
            .ReturnsAsync(errorResponse);

            user.Impl.Provider = provider.Object;

            var silentResult = await user.SignInSilentlyAsync();
            Assert.AreEqual(silentResult.Status, SignInStatus.Success);
            Assert.IsTrue(user.IsSignedIn);

            try
            {
                var token = await user.GetTokenAndSignatureAsync("GET", "", "");
            }
            catch(XboxException ex)
            {
                Assert.IsFalse(string.IsNullOrEmpty(ex.Message));
                Assert.IsFalse(user.IsSignedIn);

                Assert.IsTrue(signoutEvent.WaitOne(100), "wait signout event time out");

                return;
            }

            Assert.Fail("No exception was thrown.");
        }
    }
}
