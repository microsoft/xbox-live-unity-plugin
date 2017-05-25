// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System.Threading.Tasks;

    internal interface IUserImpl
    {
        bool IsSignedIn { get; }
        string XboxUserId { get; }
        string Gamertag { get; }
        string AgeGroup { get; }
        string Privileges { get; }
        string WebAccountId { get; }
        AuthConfig AuthConfig { get; }
#if WINDOWS_UWP
        Windows.System.User CreationContext { get; }
#endif

        Task<SignInResult> SignInImpl(bool showUI, bool forceRefresh);

        Task<TokenAndSignatureResult> InternalGetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh);
    }
}