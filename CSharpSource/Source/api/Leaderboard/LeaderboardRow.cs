// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Xbox.Services.Leaderboard
{
    public class LeaderboardRow
    {
        public IList<string> Values
        {
            get; internal set;
        }

        public int Rank
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
    }
}
