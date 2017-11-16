// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    public partial class LeaderboardRow
    {
        internal LeaderboardRow(IntPtr leaderboardRowPtr)
        {
            LEADERBOARD_ROW cRow = Marshal.PtrToStructure<LEADERBOARD_ROW>(leaderboardRowPtr);
            Rank = cRow.Rank;
            Percentile = cRow.Percentile;
            XboxUserId = MarshalingHelpers.Utf8ToString(cRow.XboxUserId);
            Gamertag = MarshalingHelpers.Utf8ToString(cRow.Gamertag);

            Values = new List<string>();
            if (cRow.ColumnValuesSize > 0)
            {
                IntPtr[] cValues = new IntPtr[cRow.ColumnValuesSize];
                Marshal.Copy(cRow.ColumnValues, cValues, 0, cRow.ColumnValuesSize);
                for (int i = 0; i < cRow.ColumnValuesSize; i++)
                {
                    Values.Add(Marshal.PtrToStringAnsi(cValues[i]));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LEADERBOARD_ROW
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Gamertag;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr XboxUserId;

            [MarshalAs(UnmanagedType.R8)]
            public double Percentile;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 Rank;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr ColumnValues;

            [MarshalAs(UnmanagedType.I4)]
            public Int32 ColumnValuesSize;
        }
    }
}
