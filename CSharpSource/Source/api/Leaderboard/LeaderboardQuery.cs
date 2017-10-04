// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    public partial class LeaderboardQuery : ILeaderboardQuery
    {
        /// <summary>
        /// Create a continuation query from an existing query combined with a continuation token.
        /// </summary>
        /// <param name="query">The query that this continuation query is based on.</param>
        /// <param name="continuationToken">The continuation token for the next request.</param>
        public LeaderboardQuery(LeaderboardQuery query, string continuationToken)
        {
            // todo needs XboxLive.Instance.Invoke<IntPtr, LeaderboardQueryCreate>();?
            // todo need to have this method or nah

            this.StatName = query.StatName;
            this.SocialGroup = query.SocialGroup;
            this.MaxItems = query.MaxItems;
            this.Order = query.Order;
            this.SkipResultToRank = query.SkipResultToRank;
            this.SkipResultToMe = query.SkipResultToMe;
        }

        public string StatName { get; private set; }

        public string SocialGroup { get; private set; }
                
        public bool HasNext { get; private set; }
    }
}