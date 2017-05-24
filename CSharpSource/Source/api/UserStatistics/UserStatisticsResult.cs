// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.UserStatistics
{
    public class UserStatisticsResult
    {

        public IList<ServiceConfigurationStatistic> ServiceConfigurationStatistics
        {
            get;
            private set;
        }

        public string XboxUserId
        {
            get;
            private set;
        }

    }
}
