// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Newtonsoft.Json;

    public partial class LeaderboardColumn
    {
        public LeaderboardStatType StatisticType { get; set; }
        
        public string StatisticName { get; set; }
        
        // Used for mock services
        internal LeaderboardColumn(LeaderboardStatType type, string name)
        {
            StatisticType = type;
            StatisticName = name;
        }
    }
}