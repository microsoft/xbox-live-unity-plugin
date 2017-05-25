// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;

    public class LeaderboardResult
    {
        public LeaderboardResult(uint totalRowCount, IList<LeaderboardColumn> columns, IList<LeaderboardRow> rows, LeaderboardQuery nextQuery)
        {
            if(nextQuery == null) throw new ArgumentNullException("nextQuery");

            this.TotalRowCount = totalRowCount;
            this.Columns = columns;
            this.Rows = rows;
            this.NextQuery = nextQuery;
        }

        public bool HasNext
        {
            get
            {
                return this.NextQuery.HasNext;
            }
        }

        public IList<LeaderboardRow> Rows { get; internal set; }

        public IList<LeaderboardColumn> Columns { get; internal set; }

        public uint TotalRowCount { get; internal set; }

        public LeaderboardQuery NextQuery { get; internal set; }
    }
}