// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Leaderboard;

    public class LeaderboardResultEventArgs : StatisticEventArgs
    {
        public LeaderboardResult Result { get; private set; }

        internal LeaderboardResultEventArgs(IntPtr leaderboardResultEventArgsPtr)
        {
            LEADERBOARD_RESULT_EVENT_ARGS cArgs = MarshalingHelpers.PtrToStructure<LEADERBOARD_RESULT_EVENT_ARGS>(leaderboardResultEventArgsPtr);
            Result = new LeaderboardResult(cArgs.Result);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LEADERBOARD_RESULT_EVENT_ARGS
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Result;
        }

        // Used for mock services
        internal LeaderboardResultEventArgs(LeaderboardResult result)
        {
            Result = result;
        }
    }
}