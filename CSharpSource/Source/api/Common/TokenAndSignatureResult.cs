// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if WINDOWS_UWP
using Windows.Security.Authentication.Web.Core;

#endif

namespace Microsoft.Xbox.Services
{
    public class TokenAndSignatureResult
    {
        public string WebAccountId { get; set; }

        public string Privileges { get; set; }

        public string AgeGroup { get; set; }

        public string XboxUserHash { get; set; }

        public string Gamertag { get; set; }

        public string XboxUserId { get; set; }

        public string Signature { get; set; }

        public string Token { get; set; }

        internal string Reserved { get; set; }
#if WINDOWS_UWP
        internal WebTokenRequestStatus TokenRequestResultStatus { get; set; }
#endif
    }
}