// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Presence
{
    public class PresenceData
    {
        public PresenceData(string serviceConfigurationId, string presenceId, string[] presenceTokenIds) {
        }
        public PresenceData(string serviceConfigurationId, string presenceId) {
        }

        public IList<string> PresenceTokenIds
        {
            get;
            private set;
        }

        public string PresenceId
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
