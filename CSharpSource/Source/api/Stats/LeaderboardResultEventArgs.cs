// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using Microsoft.Xbox.Services.Leaderboard;

    public class LeaderboardResultEventArgs : StatEventArgs
    {
        public LeaderboardResult Result { get; private set; }

        public LeaderboardResultEventArgs(LeaderboardResult result)
        {
            this.Result = result;
        }
    }
}