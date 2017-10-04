using Microsoft.Xbox.Services.Leaderboard;

namespace Microsoft.Xbox.Services.Leaderboard
{
    interface ILeaderboardQuery
    {
        string StatName { get; }
        string SocialGroup { get; }
        bool HasNext { get; }
        uint SkipResultToRank { get; set; }
        bool SkipResultToMe { get; set; }
        SortOrder Order { get; set; }
        uint MaxItems { get; set; }
    }
}
