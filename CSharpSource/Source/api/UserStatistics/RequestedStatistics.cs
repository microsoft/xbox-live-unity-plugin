// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.UserStatistics
{
    public class RequestedStatistics
    {
        public RequestedStatistics(string serviceConfigurationId, string[] statistics) {
        }

        public IList<string> Statistics
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
