// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Matchmaking
{
    public class HopperStatisticsResponse
    {

        public uint PlayersWaitingToMatch
        {
            get;
            private set;
        }

        public TimeSpan EstimatedWaitTime
        {
            get;
            private set;
        }

        public string HopperName
        {
            get;
            private set;
        }

    }
}
