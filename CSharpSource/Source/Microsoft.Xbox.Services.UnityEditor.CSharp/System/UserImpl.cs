// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Threading.Tasks;

    internal class UserImpl : IUserImpl
    {
        public bool IsSignedIn { get; set; }
        public XboxLiveUser User { get; set; }

        public string XboxUserId { get; set; }
        public string Gamertag { get; set; }
        public string AgeGroup { get; set; }
        public string Privileges { get; set; }
        public string WebAccountId { get; set; }
        public AuthConfig AuthConfig { get; set; }

        public Task<SignInResult> SignInImpl(bool showUI, bool forceRefresh)
        {
            if (XboxLive.UseMockServices)
            {
                this.IsSignedIn = true;
                this.Gamertag = "Fake User";
                this.XboxUserId = "123456789";

                return Task.FromResult(new SignInResult(SignInStatus.Success));
            }

            throw new NotImplementedException();
        }

        public Task<TokenAndSignatureResult> InternalGetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh)
        {
            if (XboxLive.UseMockServices)
            {
                return Task.FromResult(new TokenAndSignatureResult
                {
                    Gamertag = this.Gamertag,
                    XboxUserId = this.XboxUserId,
                    XboxUserHash = "",
                    Token = "",
                    Signature = "",
                });
            }
            throw new NotImplementedException();
        }
    }
}