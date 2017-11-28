// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.UnitTests.Leaderboards
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Privacy;

    [TestClass]
    public class PrivacyTests : TestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            MockXboxLiveData.Load(Environment.CurrentDirectory + "\\Privacy\\MockDataForPrivacyTests.json");
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        void VerifyPermissionCheckResult(PermissionCheckResult result, JObject resultToVerify)
        {
            var isAllowed = resultToVerify.SelectToken("isAllowed").Value<bool>();
            Assert.AreEqual(result.IsAllowed, isAllowed);

            int index = 0;
            JArray jsonReasons = (JArray)(resultToVerify)["reasons"];
            if (jsonReasons != null)
            {
                foreach (var reason in jsonReasons)
                {
                    Assert.AreEqual(result.Reasons[index].Reason, reason["reason"].ToString());
                    ++index;
                }
            }
        }

        void VerifyMultiplePermissionsCheckResult(List<MultiplePermissionsCheckResult> result, JObject resultToVerify)
        {
            int multiplePermIndex = 0;
            JArray jsonResponses = (JArray)(resultToVerify)["responses"];
            foreach (var response in jsonResponses)
            {
                Assert.AreEqual(result[multiplePermIndex].XboxUserId, response["user"]["xuid"].ToString());

                JArray jsonPermissions = (JArray)(response)["permissions"];
                int index = 0;
                foreach (var permission in jsonPermissions)
                {
                    VerifyPermissionCheckResult(result[multiplePermIndex].Items[index], (JObject)permission);
                    ++index;
                }

                ++multiplePermIndex;
            }
        }

        [TestMethod]
        public async Task CheckPermissionWithTargetUserAsync()
        {
            PermissionCheckResult result = await this.user.Services.PrivacyService.CheckPermissionWithTargetUserAsync(PermissionIdConstants.ViewTargetVideoHistory, "2814680291986301");
            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["defaultCheckPermissionsResponse"];
            JObject responseJson = JObject.Parse(mockRequestData.Response.ResponseBodyString);
            Assert.AreEqual("GET", mockRequestData.Request.Method);
            Assert.AreEqual("https://privacy.xboxlive.com/users/xuid(2814662072777140)/permission/validate?setting=ViewTargetVideoHistory&target=xuid(2814680291986301)", mockRequestData.Request.Url);
            VerifyPermissionCheckResult(result, responseJson);
        }

        [TestMethod]
        public async Task CheckMultiplePermissionsWithMultipleTargetUsersAsync()
        {
            List<string> permissionIds = new List<string>();
            permissionIds.Add(PermissionIdConstants.ViewTargetVideoHistory);
            permissionIds.Add(PermissionIdConstants.ViewTargetMusicStatus);
            permissionIds.Add(PermissionIdConstants.ViewTargetGameHistory);

            List<string> xuids = new List<string>();
            xuids.Add("2814680291986301");
            xuids.Add("2814634309691161");

            List<MultiplePermissionsCheckResult> result = await this.user.Services.PrivacyService.CheckMultiplePermissionsWithMultipleTargetUsersAsync(permissionIds, xuids);
            MockXboxLiveData.MockRequestData mockRequestData = MockXboxLiveData.MockResponses["defaultCheckMultiplePermissionsResponse"];
            JObject responseJson = JObject.Parse(mockRequestData.Response.ResponseBodyString);
            Assert.AreEqual("POST", mockRequestData.Request.Method);
            Assert.AreEqual("https://privacy.xboxlive.com/users/xuid(2814662072777140)/permission/validate", mockRequestData.Request.Url);
            VerifyMultiplePermissionsCheckResult(result, responseJson);
        }
    }
}