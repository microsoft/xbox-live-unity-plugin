// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    public class LeaderboardQuery
    {
        /// <summary>
        /// Create a new query
        /// </summary>
        public LeaderboardQuery()
        {
        }

        /// <summary>
        /// Create a continuation query from an existing query combined with a continuation token.
        /// </summary>
        /// <param name="query">The query that this continuation query is based on.</param>
        /// <param name="continuationToken">The continuation token for the next request.</param>
        public LeaderboardQuery(LeaderboardQuery query, string continuationToken)
        {
            this.StatName = query.StatName;
            this.SocialGroup = query.SocialGroup;
            this.MaxItems = query.MaxItems;
            this.Order = query.Order;
            this.SkipResultsToRank = query.SkipResultsToRank;
            this.SkipResultToMe = query.SkipResultToMe;
            this.ContinuationToken = continuationToken;
        }

        public string StatName { get; set; }

        public string SocialGroup { get; set; }

        public uint MaxItems { get; set; }

        public SortOrder Order { get; set; }

        public bool SkipResultToMe { get; set; }

        public uint SkipResultsToRank { get; set; }

        internal string ContinuationToken { get; set; }

        public bool HasNext
        {
            get
            {
                return !string.IsNullOrEmpty(this.ContinuationToken);
            }
        }
    }
}