using System.Collections.Generic;

namespace Microsoft.Xbox.Services.Leaderboard
{
    interface ILeaderboardResult
    {
        LeaderboardQuery GetNextQuery();

        // todo remove for ID sdk
        //#if !XBOX_LIVE_CREATORS_SDK
        // public Task<LeaderboardResult> GetNextAsync(uint maxItems)
        //#endif

        bool HasNext { get; }
        IList<LeaderboardRow> Rows { get;}
        IList<LeaderboardColumn> Columns { get; }
        uint TotalRowCount { get;}
    }
}
