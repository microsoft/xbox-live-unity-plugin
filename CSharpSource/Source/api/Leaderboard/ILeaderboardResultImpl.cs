// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    interface ILeaderboardResultImpl
    {
        LeaderboardQuery GetNextQueryImpl();

        // todo remove for ID sdk
        //#if !XBOX_LIVE_CREATORS_SDK
        // public Task<LeaderboardResult> GetNextAsync(uint maxItems)
        //#endif

        bool GetHasNext();
    }
}
