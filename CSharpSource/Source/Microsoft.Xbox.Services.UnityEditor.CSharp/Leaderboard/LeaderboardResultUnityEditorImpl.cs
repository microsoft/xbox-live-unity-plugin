
namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;

    class LeaderboardResultUnityEditorImpl : ILeaderboardResultImpl
    {
        public bool GetHasNext()
        {
            return false;
        }

        public LeaderboardQuery GetNextQueryImpl()
        {
            return null;
        }
    }
}
