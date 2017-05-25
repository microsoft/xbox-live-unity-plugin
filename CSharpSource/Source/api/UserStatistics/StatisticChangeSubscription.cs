// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.UserStatistics
{
    public class StatisticChangeSubscription
    {

        public string StatisticName
        {
            get;
            private set;
        }

        public string ServiceConfigurationId
        {
            get;
            private set;
        }

        public string XboxUserId
        {
            get;
            private set;
        }

        public uint SubscriptionId
        {
            get;
            private set;
        }

        public string ResourceUri
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.RealTimeActivity.RealTimeActivitySubscriptionState State
        {
            get;
            private set;
        }

    }
}
