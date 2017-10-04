// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Leaderboard;

    public partial class LeaderboardResultEventArgs
    {
        internal LeaderboardResultEventArgs(IntPtr leaderboardResultEventArgsPtr)
        {
            LeaderboardResultEventArgs_c cArgs = Marshal.PtrToStructure<LeaderboardResultEventArgs_c>(leaderboardResultEventArgsPtr);
            Result = new LeaderboardResult(cArgs.Result);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LeaderboardResultEventArgs_c
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Result;
        }
    }
}
