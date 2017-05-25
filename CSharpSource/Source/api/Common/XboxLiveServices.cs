// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using Microsoft.Xbox.Services.Privacy;

    public partial class XboxLiveServices
    {
        private PrivacyService privacyService;

        public XboxLiveServices(XboxLiveUser user)
        {
            this.privacyService = new PrivacyService(user);
        }

        public PrivacyService PrivacyService
        {
            get
            {
                return this.privacyService;
            }
        }
    }
}