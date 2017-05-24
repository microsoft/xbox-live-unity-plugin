// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using Microsoft.Xbox.Services.System;

    public partial class XboxLiveUser
    {
        public XboxLiveUser()
        {
            this.userImpl = new UserImpl();
        }

        public XboxLiveUser(string xuid, string gamertag)
        {
            this.userImpl = new UserImpl(xuid, gamertag);
        }
    }
}