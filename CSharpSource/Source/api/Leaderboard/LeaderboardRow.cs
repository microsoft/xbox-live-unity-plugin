// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.Services.Leaderboard
{
    public class LeaderboardRow
    {
        internal LeaderboardRow(IntPtr leaderboardRowPtr)
        {
            LeaderboardRow_c cRow = Marshal.PtrToStructure<LeaderboardRow_c>(leaderboardRowPtr);
            Rank = cRow.Rank;
            Percentile = cRow.Percentile;
            XboxUserId = cRow.XboxUserId;
            Gamertag = cRow.Gamertag;

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

        //todo move
        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardRow_c
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Gamertag;

            [MarshalAs(UnmanagedType.LPStr)]
            public string XboxUserId;

            [MarshalAs(UnmanagedType.R8)]
            public double Percentile;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 Rank;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr ColumnValues;

            [MarshalAs(UnmanagedType.I4)]
            public Int32 ColumnValuesSize;
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
    }
}
