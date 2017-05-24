// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerQuerySearchHandleRequest
    {
        public MultiplayerQuerySearchHandleRequest(string serviceConfigurationId, string sessionTemplateName) {
        }

        public string SocialGroup
        {
            get;
            set;
        }

        public string SearchFilter
        {
            get;
            set;
        }

        public bool OrderAscending
        {
            get;
            set;
        }

        public string OrderBy
        {
            get;
            set;
        }

        public string SessionTemplateName
        {
            get;
            private set;
        }

        public string ServiceConfigurationId
        {
            get;
            private set;
        }

    }
}
