// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    public partial class LeaderboardQuery : ILeaderboardQuery
    {
        public uint MaxItems
        {
            get;
            set;
        }
        
        public SortOrder Order
        {
            get;
            set;
        }
        
        public bool SkipResultToMe
        {
            get;
            set;
        }

        public uint SkipResultToRank
        {
            get;
            set;
        }
    }
}
