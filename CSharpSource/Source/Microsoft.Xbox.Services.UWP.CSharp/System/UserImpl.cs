// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using Windows.Foundation;
    using Windows.Security.Authentication.Web.Core;
    using Windows.System;
    using Windows.System.Threading;
    using Windows.UI.Core;

    using global::System;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Threading.Tasks;
    using global::System.Collections.Concurrent;

    internal class UserImpl : IUserImpl
    {
        public event EventHandler SignInCompleted;
        public event EventHandler SignOutCompleted;

        private static bool? isMultiUserApplication;
        private static CoreDispatcher dispatcher;
        private static UserWatcher userWatcher;
        private static readonly ConcurrentDictionary<string, UserImpl> trackingUsers = new ConcurrentDictionary<string, UserImpl>();

        private readonly object userImplLock = new object();
        internal AccountProvider Provider { get; set; } = new AccountProvider();

        public bool IsSignedIn { get; private set; }
        public string XboxUserId { get; private set; }
        public string Gamertag { get; private set; }
        public string AgeGroup { get; private set; }
        public string Privileges { get; private set; }
        public string WebAccountId { get; private set; }
        public AuthConfig AuthConfig { get; private set; }
        public User CreationContext { get; private set; }

        public static CoreDispatcher Dispatcher
        {
            get
            {
                return dispatcher ?? (dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher);
            }
        }

        private ThreadPoolTimer threadPoolTimer;

        public UserImpl(User systemUser)
        {
            if (IsMultiUserApplication)
            {
                if (systemUser == null)
                {
                    throw new XboxException("Xbox Live User object is required to be constructed by a Windows.System.User object for a multi-user application.");
                }

                //Initiate user watcher
                if (userWatcher == null)
                {
                    userWatcher = User.CreateWatcher();
                    userWatcher.Removed += UserWatcher_UserRemoved;
                }
            }

            this.CreationContext = systemUser;

            // TODO: This config is broken.
            var appConfig = XboxLiveAppConfiguration.Instance;
            this.AuthConfig = new AuthConfig
            {
                Sandbox = appConfig.Sandbox,
                EnvironmentPrefix = appConfig.EnvironmentPrefix,
                Environment = appConfig.Environment,
                UseCompactTicket = appConfig.UseFirstPartyToken
            };
        }

        public async Task<SignInResult> SignInImpl(bool showUI, bool forceRefresh)
        {
            await this.Provider.InitializeProvider(this.CreationContext);

            // Try get the default system user for single user application
            if (!IsMultiUserApplication)
            {
                var allUser = await Windows.System.User.FindAllAsync();
                var validSysUser = allUser.Where(user => (user.Type != Windows.System.UserType.LocalGuest || user.Type != Windows.System.UserType.RemoteGuest)).ToList();
                if (validSysUser.Count > 0)
                {
                    this.CreationContext = validSysUser[0];
                }
            }

            TokenAndSignatureResult result = await this.InternalGetTokenAndSignatureHelperAsync("GET", this.AuthConfig.XboxLiveEndpoint, "", null, showUI, false);
            SignInStatus status = ConvertWebTokenRequestStatus(result.TokenRequestResultStatus);

            if (status != SignInStatus.Success)
            {
                return new SignInResult(status);
            }

            if (string.IsNullOrEmpty(result.Token))
            {
                // TODO: set presence
            }

            this.UserSignedIn(result.XboxUserId, result.Gamertag, result.AgeGroup, result.Privileges, result.WebAccountId);

            return new SignInResult(status);
        }

        private static void UserWatcher_UserRemoved(UserWatcher sender, UserChangedEventArgs args)
        {
            UserImpl signoutUser;
            if (trackingUsers.TryGetValue(args.User.NonRoamableId, out signoutUser))
            {
                signoutUser.UserSignedOut();
            }
        }

        private static bool IsMultiUserApplication
        {
            get
            {
                if (isMultiUserApplication == null)
                {
                    try
                    {
                        bool apiExist = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.System.UserPicker", "IsSupported");
                        isMultiUserApplication = (apiExist && UserPicker.IsSupported());
                    }
                    catch (Exception)
                    {
                        isMultiUserApplication = false;
                    }
                }
                return isMultiUserApplication == true;
            }
        }

        public async Task<TokenAndSignatureResult> InternalGetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh)
        {
            var result = await this.InternalGetTokenAndSignatureHelperAsync(httpMethod, url, headers, body, promptForCredentialsIfNeeded, forceRefresh);
            if (result.TokenRequestResultStatus != WebTokenRequestStatus.UserInteractionRequired)
            {
                return result;
            }

            // Failed to get 'xboxlive.com' token, sign out if already sign in (SPOP or user banned).
            // But for sign in path, it's expected.
            if (this.AuthConfig.XboxLiveEndpoint != null && url == this.AuthConfig.XboxLiveEndpoint && this.IsSignedIn)
            {
                this.UserSignedOut();
            }
            else if (url != this.AuthConfig.XboxLiveEndpoint)
            {
                // If it's not asking for xboxlive.com's token, we treat UserInteractionRequired as an error
                string errorMsg = "Failed to get token for endpoint: " + url;
                throw new XboxException(errorMsg);
            }

            return result;
        }

        private async Task<TokenAndSignatureResult> InternalGetTokenAndSignatureHelperAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh)
        {
            var request = this.Provider.CreateWebTokenRequest();
            request.Properties.Add("HttpMethod", httpMethod);
            request.Properties.Add("Url", url);
            if (!string.IsNullOrEmpty(headers))
            {
                request.Properties.Add("RequestHeaders", headers);
            }
            if (forceRefresh)
            {
                request.Properties.Add("ForceRefresh", "true");
            }

            if (body != null && body.Length > 0)
            {
                request.Properties.Add("RequestBody", Encoding.UTF8.GetString(body));
            }

            request.Properties.Add("Target", this.AuthConfig.RPSTicketService);
            request.Properties.Add("Policy", this.AuthConfig.RPSTicketPolicy);
            if (promptForCredentialsIfNeeded)
            {
                string pfn = Windows.ApplicationModel.Package.Current.Id.FamilyName;
                request.Properties.Add("PackageFamilyName", pfn);
            }

            TokenAndSignatureResult tokenAndSignatureReturnResult = null;
            var tokenResult = await RequestTokenFromIdpAsync(promptForCredentialsIfNeeded, request);
            tokenAndSignatureReturnResult = this.ConvertWebTokenRequestResult(tokenResult);
            if (tokenAndSignatureReturnResult != null && this.IsSignedIn && tokenAndSignatureReturnResult.XboxUserId != this.XboxUserId)
            {
                this.UserSignedOut();
                throw new XboxException("User has switched");
            }

            return tokenAndSignatureReturnResult;
        }

        private Task<TokenRequestResult> RequestTokenFromIdpAsync(bool promptForCredentialsIfNeeded, WebTokenRequest request)
        {
            if (!promptForCredentialsIfNeeded)
            {
                return this.Provider.GetTokenSilentlyAsync(request);
            }

            TaskCompletionSource<TokenRequestResult> tokenRequestSource = new TaskCompletionSource<TokenRequestResult>();
            IAsyncAction requestTokenTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    try
                    {
                        var result = await this.Provider.RequestTokenAsync(request);
                        tokenRequestSource.SetResult(result);
                    }
                    catch (Exception e)
                    {
                        tokenRequestSource.SetException(e);
                    }
                });

            return tokenRequestSource.Task;
        }

        private TokenAndSignatureResult ConvertWebTokenRequestResult(TokenRequestResult tokenResult)
        {
            var tokenResponseStatus = tokenResult.ResponseStatus;

            if (tokenResponseStatus == WebTokenRequestStatus.Success)
            {

                string xboxUserId = tokenResult.Properties["XboxUserId"];
                string gamertag = tokenResult.Properties["Gamertag"];
                string ageGroup = tokenResult.Properties["AgeGroup"];
                string environment = tokenResult.Properties["Environment"];
                string sandbox = tokenResult.Properties["Sandbox"];
                string webAccountId = tokenResult.WebAccountId;
                string token = tokenResult.Token;

                string signature = null;
                if (tokenResult.Properties.ContainsKey("Signature"))
                {
                    signature = tokenResult.Properties["Signature"];
                }

                string privilege = null;
                if (tokenResult.Properties.ContainsKey("Privileges"))
                {
                    privilege = tokenResult.Properties["Privileges"];
                }

                if (environment.ToLower() == "prod")
                {
                    environment = null;
                }

                var appConfig = XboxLiveAppConfiguration.Instance;
                appConfig.Sandbox = sandbox;
                appConfig.Environment = environment;

                return new TokenAndSignatureResult
                {
                    WebAccountId = webAccountId,
                    Privileges = privilege,
                    AgeGroup = ageGroup,
                    Gamertag = gamertag,
                    XboxUserId = xboxUserId,
                    Signature = signature,
                    Token = token,
                    TokenRequestResultStatus = tokenResult.ResponseStatus
                };
            }
            else if (tokenResponseStatus == WebTokenRequestStatus.AccountSwitch)
            {
                this.UserSignedOut();
                throw new XboxException("User has switched");
            }
            else if (tokenResponseStatus == WebTokenRequestStatus.ProviderError)
            {
                string errorMsg = "Provider error: " + tokenResult.ResponseError.ErrorMessage  + ", Error Code: " + tokenResult.ResponseError.ErrorCode.ToString("X");
                throw new XboxException((int)tokenResult.ResponseError.ErrorCode, errorMsg);
            }
            else
            {
                return new TokenAndSignatureResult()
                {
                    TokenRequestResultStatus = tokenResult.ResponseStatus
                };
            }

        }

        private void UserSignedIn(string xboxUserId, string gamertag, string ageGroup, string privileges, string webAccountId)
        {
            lock (this.userImplLock)
            {
                this.XboxUserId = xboxUserId;
                this.Gamertag = gamertag;
                this.AgeGroup = ageGroup;
                this.Privileges = privileges;
                this.WebAccountId = webAccountId;

                this.IsSignedIn = true;
            }

            this.OnSignInCompleted();

            // We use user watcher for MUA, if it's SUA we use own checker for sign out event.
            if (!IsMultiUserApplication)
            {
                this.threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(
                    source => { this.CheckUserSignedOut(); },
                    TimeSpan.FromSeconds(10)
                );
            }
            else
            {
                trackingUsers.TryAdd(this.CreationContext.NonRoamableId, this);
            }
        }

        private void UserSignedOut()
        {
            if (!this.IsSignedIn)
            {
                return;
            }

            lock (this.userImplLock)
            {
                this.IsSignedIn = false;
            }

            this.OnSignOutCompleted();

            lock (this.userImplLock)
            {
                // Check again on isSignedIn flag, in case users signed in again in signOutHandlers callback,
                // so we don't clean up the properties. 
                if (!this.IsSignedIn)
                {
                    this.XboxUserId = null;
                    this.Gamertag = null;
                    this.AgeGroup = null;
                    this.Privileges = null;
                    this.WebAccountId = null;

                    if (this.CreationContext != null)
                    {
                        UserImpl outResult;
                        trackingUsers.TryRemove(this.CreationContext.NonRoamableId, out outResult);
                    }

                    if (this.threadPoolTimer != null)
                    {
                        this.threadPoolTimer.Cancel();
                    }
                }
            }
        }

        private void CheckUserSignedOut()
        {
            if (!this.IsSignedIn) return;

            try
            {
                var signedInAccount = this.Provider.FindAccountAsync(this.WebAccountId);
                if (signedInAccount == null)
                {
                    this.UserSignedOut();
                }
            }
            catch (Exception)
            {
                this.UserSignedOut();
            }
        }

        private static SignInStatus ConvertWebTokenRequestStatus(WebTokenRequestStatus status)
        {
            switch (status)
            {
                case WebTokenRequestStatus.Success:
                    return SignInStatus.Success;
                case WebTokenRequestStatus.UserCancel:
                    return SignInStatus.UserCancel;
                case WebTokenRequestStatus.UserInteractionRequired:
                    return SignInStatus.UserInteractionRequired;
                case WebTokenRequestStatus.AccountSwitch:
                case WebTokenRequestStatus.AccountProviderNotAvailable:
                case WebTokenRequestStatus.ProviderError:
                    throw new XboxException("Unexpected WebTokenRequestStatus");
                default:
                    throw new ArgumentOutOfRangeException("WebTokenRequestStatus");
            }
        }

        protected virtual void OnSignInCompleted()
        {
            var onSignInCompleted = this.SignInCompleted;
            if (onSignInCompleted != null)
            {
                onSignInCompleted(this, new EventArgs());
            }
        }

        protected virtual void OnSignOutCompleted()
        {
            var onSignOutCompleted = this.SignOutCompleted;
            if (onSignOutCompleted != null)
            {
                onSignOutCompleted(this, new EventArgs());
            }
        }
    }
}