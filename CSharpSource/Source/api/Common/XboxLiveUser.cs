// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Text;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.System;

    public partial class XboxLiveUser : IXboxLiveUser
    {
        private static readonly object instanceLock = new object();

        private readonly IUserImpl userImpl;
        private XboxLiveServices xboxLiveServices;

        private static event EventHandler<SignInCompletedEventArgs> InternalSignInCompleted;
        private static List<EventHandler<SignInCompletedEventArgs>> signInDelegates = new List<EventHandler<SignInCompletedEventArgs>>();
        public static event EventHandler<SignInCompletedEventArgs> SignInCompleted
        {
            add
            {
                InternalSignInCompleted += value;
                signInDelegates.Add(value);
            }
            remove
            {
                InternalSignInCompleted -= value;
                signInDelegates.Remove(value);
            }
        }

        private static event EventHandler<SignOutCompletedEventArgs> InternalSignOutCompleted;
        private static List<EventHandler<SignOutCompletedEventArgs>> signOutDelegates = new List<EventHandler<SignOutCompletedEventArgs>>();
        public static event EventHandler<SignOutCompletedEventArgs> SignOutCompleted
        {
            add
            {
                InternalSignOutCompleted += value;
                signOutDelegates.Add(value);
            }
            remove
            {
                InternalSignOutCompleted -= value;
                signOutDelegates.Remove(value);
            }
        }

        public string WebAccountId
        {
            get
            {
                return this.userImpl.WebAccountId;
            }
        }

        public bool IsSignedIn
        {
            get
            {
                return this.userImpl.IsSignedIn;
            }
        }

        public string Privileges
        {
            get
            {
                return this.userImpl.Privileges;
            }
        }

        public string XboxUserId
        {
            get
            {
                return this.userImpl.XboxUserId;
            }
        }

        public string AgeGroup
        {
            get
            {
                return this.userImpl.AgeGroup;
            }
        }

        public string Gamertag
        {
            get
            {
                return this.userImpl.Gamertag;
            }
        }

        public XboxLiveServices Services
        {
            get
            {
                if (this.xboxLiveServices == null)
                {
                    lock (instanceLock)
                    {
                        if (this.xboxLiveServices == null)
                        {
                            this.xboxLiveServices = new XboxLiveServices(this);
                        }
                    }
                }

                return this.xboxLiveServices;
            }
        }

        public Task<SignInResult> SignInAsync()
        {
            return this.userImpl.SignInImpl(true, false);
        }

        public Task<SignInResult> SignInSilentlyAsync()
        {
            return this.userImpl.SignInImpl(false, false);
        }

        public Task<TokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers)
        {
            return this.GetTokenAndSignatureArrayAsync(httpMethod, url, headers, null);
        }

        public Task<TokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers, string body)
        {
            return this.GetTokenAndSignatureArrayAsync(httpMethod, url, headers, body == null ? null : Encoding.UTF8.GetBytes(body));
        }

        public Task<TokenAndSignatureResult> GetTokenAndSignatureArrayAsync(string httpMethod, string url, string headers, byte[] body)
        {
            return this.userImpl.InternalGetTokenAndSignatureAsync(httpMethod, url, headers, body, false, false);
        }

        public Task RefreshToken()
        {
            return this.userImpl.InternalGetTokenAndSignatureAsync("GET", this.userImpl.AuthConfig.XboxLiveEndpoint, null, null, false, true);
        }

        protected static void OnSignInCompleted(IXboxLiveUser user)
        {
            var handler = InternalSignInCompleted;
            if (handler != null) handler(null, new SignInCompletedEventArgs(user));
        }

        protected static void OnSignOutCompleted(IXboxLiveUser user)
        {
            var handler = InternalSignOutCompleted;
            if (handler != null) handler(null, new SignOutCompletedEventArgs(user));
        }
    }
}