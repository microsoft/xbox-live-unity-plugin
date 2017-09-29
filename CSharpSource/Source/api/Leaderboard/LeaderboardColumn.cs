// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Newtonsoft.Json;

    public class LeaderboardColumn
    {
        [JsonProperty("type")]
        public LeaderboardStatType StatisticType { get; set; }

        [JsonProperty("statName")]
        public string StatisticName { get; set; }

        internal LeaderboardColumn(IntPtr leaderboardColumnPtr)
        {
            LeaderboardColumn_c cColumn = Marshal.PtrToStructure<LeaderboardColumn_c>(leaderboardColumnPtr);

            StatisticType = cColumn.StatType;
            StatisticName = cColumn.StatName;
        }

        // todo move
        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardColumn_c
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string StatName;

            [MarshalAs(UnmanagedType.I4)]
            public LeaderboardStatType StatType;
        }
    }
}