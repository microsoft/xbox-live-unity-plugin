// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System.Threading.Tasks;

    public interface IXboxLiveUser
    {
        string WebAccountId { get; }

        bool IsSignedIn { get; }

        string Privileges { get; }

        string AgeGroup { get; }

        string Gamertag { get; }

        string XboxUserId { get; }

        XboxLiveServices Services { get; }

#if WINDOWS_UWP
        Windows.System.User WindowsSystemUser { get; }
#endif

        Task<SignInResult> SignInAsync();

        Task<SignInResult> SignInSilentlyAsync();

        Task<TokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers);

        Task<TokenAndSignatureResult> GetTokenAndSignatureAsync(string httpMethod, string url, string headers, string body);

        Task<TokenAndSignatureResult> GetTokenAndSignatureArrayAsync(string httpMethod, string url, string headers, byte[] body);
    }
}