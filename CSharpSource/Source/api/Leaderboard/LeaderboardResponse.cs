// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class LeaderboardInfo
    {
        [JsonProperty("columnDefinition")]
        public LeaderboardColumn Column { get; set; }

        [JsonProperty("totalCount")]
        public uint TotalCount { get; set; }
    }

    internal class PagingInfo
    {
        [JsonProperty("continuationToken")]
        public string ContinuationToken { get; set; }
    }

    internal class LeaderboardRowResponse
    {
        public IList<string> Values
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public int Rank
        {
            get;
            set;
        }

        public double Percentile
        {
            get;
            set;
        }

        [JsonProperty("xuid")]
        public string XboxUserId
        {
            get;
            set;
        }

        public string Gamertag
        {
            get;
            set;
        }
    }

    internal class LeaderboardResponse
    {
        [JsonProperty ("leaderboardInfo")]
        public LeaderboardInfo LeaderboardInfo { get;  set; }

        [JsonProperty ("pagingInfo")]
        public PagingInfo PagingInfo { get; set; }

        [JsonProperty("userList")]
        public IList<LeaderboardRowResponse> Rows { get; set; }
    }
}