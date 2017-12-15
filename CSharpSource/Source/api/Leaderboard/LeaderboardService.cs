// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Threading.Tasks;

    public class LeaderboardService : ILeaderboardService
    {
        internal LeaderboardService()
        {
        }

        public Task<LeaderboardResult> GetLeaderboardAsync(XboxLiveUser user, LeaderboardQuery query)
        {
            throw new NotImplementedException();
        }
    }
}