// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using System;

    class LeaderboardResultImpl : ILeaderboardResultImpl
    {
        IntPtr m_leaderboardResultPtr;

        [DllImport(XboxLive.FlatCDllName)]
        private static extern bool LeaderboardResultHasNext(IntPtr leaderboard);
        public bool GetHasNext()
        {
            return LeaderboardResultHasNext(m_leaderboardResultPtr);
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT LeaderboardResultGetNextQuery(IntPtr leaderboard, IntPtr nextQuery, IntPtr errMessage);
        public LeaderboardQuery GetNextQueryImpl()
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            IntPtr cNextQuery = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));

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

        public LeaderboardResultImpl(IntPtr ptr)
        {
            m_leaderboardResultPtr = ptr;
        }
    }
}
