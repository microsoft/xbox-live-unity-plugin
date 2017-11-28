namespace Microsoft.Xbox.Services.Leaderboard
{
    class MockLeaderboardResultImpl : ILeaderboardResultImpl
    {
        public bool GetHasNext()
        {
            return false;
        }

        public LeaderboardQuery GetNextQueryImpl()
        {
            // todo find a better way of doing this
            return new LeaderboardQuery(null, "");
        }
    }
}
