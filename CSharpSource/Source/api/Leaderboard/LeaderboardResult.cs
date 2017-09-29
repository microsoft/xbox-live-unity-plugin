// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

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

        internal LeaderboardResult(IntPtr leaderboardResultPtr)
        {
            LeaderboardResult_c cResult = Marshal.PtrToStructure<LeaderboardResult_c>(leaderboardResultPtr);

            TotalRowCount = cResult.TotalRowCount;

            Columns = new List<LeaderboardColumn>();
            if (cResult.ColumnsSize > 0)
            {
                IntPtr[] cColumns = new IntPtr[cResult.ColumnsSize];
                Marshal.Copy(cResult.Columns, cColumns, 0, cResult.ColumnsSize);
                for (int i = 0; i < cResult.ColumnsSize; i++)
                {
                    Columns.Add(new LeaderboardColumn(cColumns[i]));
                }
            }

            Rows = new List<LeaderboardRow>();
            if (cResult.RowsSize > 0)
            {
                IntPtr[] cRows = new IntPtr[cResult.RowsSize];
                Marshal.Copy(cResult.Rows, cRows, 0, cResult.RowsSize);
                for (int i = 0; i < cResult.RowsSize; i++)
                {
                    Rows.Add(new LeaderboardRow(cRows[i]));
                }
            }
        }

        public bool HasNext
        {
            get
            {
                // todo fix
                return false;
                // return this.NextQuery.HasNext;
            }
        }

        public IList<LeaderboardRow> Rows { get; internal set; }

        public IList<LeaderboardColumn> Columns { get; internal set; }

        public uint TotalRowCount { get; internal set; }

        public LeaderboardQuery NextQuery { get; internal set; }

        // todo move
        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardResult_c
        {
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 TotalRowCount;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Columns;

            [MarshalAs(UnmanagedType.U4)]
            public Int32 ColumnsSize;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Rows;

            [MarshalAs(UnmanagedType.U4)]
            public Int32 RowsSize;
        }
    }
}