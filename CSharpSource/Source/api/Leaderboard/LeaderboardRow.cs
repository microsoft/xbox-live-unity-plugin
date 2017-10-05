// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.Xbox.Services.Leaderboard
{
    public partial class LeaderboardRow
    {
        public IList<string> Values
        {
            get; internal set;
        }

        public uint Rank
        {
            get; internal set;
        }

        public double Percentile
        {
            get; internal set;
        }

        public string XboxUserId
        {
            get; internal set;
        }

        public string Gamertag
        {
            get; internal set;
        }

        // Used for mock services
        internal LeaderboardRow(IList<string> values, uint rank, double percentile, string xboxUserId, string gamertag)
        {
            Values = values;
            Rank = rank;
            Percentile = percentile;
            XboxUserId = xboxUserId;
            Gamertag = gamertag;
        }

        internal LeaderboardRow()
        {

        }
    }
}
