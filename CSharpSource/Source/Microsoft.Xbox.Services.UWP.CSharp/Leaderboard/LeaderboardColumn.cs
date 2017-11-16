// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class LeaderboardColumn
    {
        internal LeaderboardColumn(IntPtr leaderboardColumnPtr)
        {
            LEADERBOARD_COLUMN cColumn = Marshal.PtrToStructure<LEADERBOARD_COLUMN>(leaderboardColumnPtr);

            StatisticType = cColumn.StatType;
            StatisticName = MarshalingHelpers.Utf8ToString(cColumn.StatName);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LEADERBOARD_COLUMN
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr StatName;

            [MarshalAs(UnmanagedType.I4)]
            public LeaderboardStatType StatType;
        }
    }
}
