// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.System
{
    using global::System.Text;
    using Windows.Foundation;
    using Windows.Security.Authentication.Web.Core;
    using Windows.Security.Credentials;
    using Windows.System;

    public partial class XboxLiveUser
    {
        User WindowsSystemUser
        {
            get;
        }

        private static bool? isSupported = null;
        WebTokenResponse webTokenResponse = null;
        private TaskCompletionSource<SignInResult> signInCompletionSource = null;
        private bool IsMultiUserApplication()
        {
            if (isSupported == null)
            {
                try
                {
                    bool APIExist = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.System.UserPicker", "IsSupported");
                    isSupported = (APIExist && UserPicker.IsSupported()) ? true : false;
                }
                catch (Exception)
                {
                    isSupported = false;
                }
            }
            return isSupported == true;
        }

        public Task<SignInResult> SignInAsync(User user)
        {
            if(signInCompletionSource == null)
            {
                signInCompletionSource = new TaskCompletionSource<SignInResult>();
                InitializeProvider(user);
            }
            return signInCompletionSource.Task;
        }

        public Task<SignInResult> SignInAsync()
        {
            return SignInAsync(null);
        }

        public Task<SignInResult> SignInSilentlyAsync()
        {
            this.IsSignedIn = true;
            return Task.FromResult(new SignInResult(SignInStatus.Success));
        }

        public Task<SignInResult> SwitchAccountAsync()
        {
            this.IsSignedIn = true;
            return Task.FromResult(new SignInResult(SignInStatus.Success));
        }

        public Task<GetTokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers)
        {
            return this.GetTokenAndSignatureAsync(httpMethod, url, headers, (byte[])null);
        }

        public Task<GetTokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers, string body)
        {
            return this.GetTokenAndSignatureAsync(httpMethod, url, headers, Encoding.UTF8.GetBytes(body));
        }

        public Task<GetTokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body)
        {
            string token = string.Empty;
            string signature = string.Empty;
            if (webTokenResponse != null)
            {
                token = webTokenResponse.Token;
                signature = webTokenResponse.Properties["Signature"];
            }
            return Task.FromResult(
                new GetTokenAndSignatureResult
                {
                    Gamertag = this.Gamertag,
                    AgeGroup = this.AgeGroup,
                    Privileges = this.Privileges,
                    XboxUserId = this.XboxUserId,
                    WebAccountId = this.WebAccountId,
                    Token = token,
                    Signature = signature
                });
        }

        private void InitializeProvider(User user)
        {
            IAsyncOperation<WebAccountProvider> provider;
            if (user != null && IsMultiUserApplication())
            {
                provider = WebAuthenticationCoreManager.FindAccountProviderAsync("https://xsts.auth.xboxlive.com", "", user);
            }
            else
            {
                provider = WebAuthenticationCoreManager.FindAccountProviderAsync("https://xsts.auth.xboxlive.com");
            }
            provider.Completed = FindAccountCompleted;
        }
#pragma warning disable 4014
        private void FindAccountCompleted(IAsyncOperation<WebAccountProvider> asyncInfo, AsyncStatus asyncStatus)
        {
            WebTokenRequest request = new WebTokenRequest(asyncInfo.GetResults());
            request.Properties.Add("HttpMethod", "GET");
            request.Properties.Add("Url", "https://xboxlive.com");
            request.Properties.Add("RequestHeaders", "");
            request.Properties.Add("ForceRefresh", "true");
            request.Properties.Add("Target", "xboxlive.signin");
            request.Properties.Add("Policy", "DELEGATION");

            request.Properties.Add("PackageFamilyName", Windows.ApplicationModel.Package.Current.Id.FamilyName);
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                WebAuthenticationCoreManager.RequestTokenAsync(request).Completed = TokenRequestCompleted;
            });
        }
#pragma warning restore 4014

        private void TokenRequestCompleted(IAsyncOperation<WebTokenRequestResult> asyncInfo, AsyncStatus asyncStatus)
        {
            WebTokenRequestResult result = asyncInfo.GetResults();
            if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                WebTokenResponse response = result.ResponseData.ElementAt(0);
                this.webTokenResponse = response;
                XboxUserId = response.Properties["XboxUserId"];
                Gamertag = response.Properties["Gamertag"];
                AgeGroup = response.Properties["AgeGroup"];
                Privileges = response.Properties["Privileges"];
                WebAccountId = response.WebAccount.Id;
                IsSignedIn = true;
                signInCompletionSource.SetResult(new SignInResult(SignInStatus.Success));
            }
            else if(result.ResponseStatus == WebTokenRequestStatus.UserCancel)
            {
                signInCompletionSource.SetResult(new SignInResult(SignInStatus.UserCancel));
            }
            else if (result.ResponseStatus == WebTokenRequestStatus.UserInteractionRequired)
            {
                signInCompletionSource.SetResult(new SignInResult(SignInStatus.UserInteractionRequired));
            }
            else
            {
                signInCompletionSource.SetResult(new SignInResult(SignInStatus.ProviderError));
            }
            signInCompletionSource = null;

        }
    }
}
