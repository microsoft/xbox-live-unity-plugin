// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using Microsoft.Xbox.Services.Leaderboard;

    public partial class LeaderboardResultEventArgs : StatisticEventArgs
    {
        public LeaderboardResult Result { get; private set; }
        
        // Used for mock services
        internal LeaderboardResultEventArgs(LeaderboardResult result)
        {
            Result = result;
        }
    }
}