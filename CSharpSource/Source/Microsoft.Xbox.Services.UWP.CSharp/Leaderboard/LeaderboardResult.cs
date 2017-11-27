// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using System;

    public partial class LeaderboardResult : ILeaderboardResult
    {
        [DllImport(XboxLive.FlatCDllName)]
        private static extern bool LeaderboardResultHasNext(IntPtr leaderboard);
        public bool HasNext
        {
            get
            {
                return LeaderboardResultHasNext(m_leaderboardResultPtr);
            }
        }

        IntPtr m_leaderboardResultPtr;
        internal LeaderboardResult(IntPtr leaderboardResultPtr)
        {
            m_leaderboardResultPtr = leaderboardResultPtr;
            LEADERBOARD_RESULT cResult = Marshal.PtrToStructure<LEADERBOARD_RESULT>(leaderboardResultPtr);

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

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT LeaderboardResultGetNextQuery(IntPtr leaderboard, IntPtr nextQuery, IntPtr errMessage);
        public LeaderboardQuery GetNextQuery()
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cNextQuery = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = LeaderboardResultGetNextQuery(m_leaderboardResultPtr, cNextQuery, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Does local work
            LeaderboardQuery nextQuery = new LeaderboardQuery(Marshal.ReadIntPtr(cNextQuery));
            Marshal.FreeHGlobal(cNextQuery);

            return nextQuery;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct LEADERBOARD_RESULT
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
