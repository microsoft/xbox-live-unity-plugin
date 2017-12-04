// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.Services.Leaderboard
{
    public class LeaderboardRow
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

        internal LeaderboardRow(IntPtr leaderboardRowPtr)
        {
            LEADERBOARD_ROW cRow = MarshalingHelpers.PtrToStructure<LEADERBOARD_ROW>(leaderboardRowPtr);
            Rank = cRow.Rank;
            Percentile = cRow.Percentile;
            XboxUserId = MarshalingHelpers.Utf8ToString(cRow.XboxUserId);
            Gamertag = MarshalingHelpers.Utf8ToString(cRow.Gamertag);

            Values = new List<string>();
            if (cRow.ColumnValuesCount > 0)
            {
                IntPtr[] cValues = new IntPtr[cRow.ColumnValuesCount];
                Marshal.Copy(cRow.ColumnValues, cValues, 0, (int)cRow.ColumnValuesCount);
                for (uint i = 0; i < cRow.ColumnValuesCount; i++)
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

            [MarshalAs(UnmanagedType.U4)]
            public uint ColumnValuesCount;
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
