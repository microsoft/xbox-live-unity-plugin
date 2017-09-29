// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Leaderboard;
    using static Microsoft.Xbox.Services.Statistics.Manager.StatsManager;

    public class LeaderboardResultEventArgs : StatEventArgs
    {
        public LeaderboardResult Result { get; private set; }

        public LeaderboardResultEventArgs(LeaderboardResult result)
        {
            this.Result = result;
        }

        internal LeaderboardResultEventArgs(IntPtr leaderboardResultEventArgsPtr)
        {
            LeaderboardResultEventArgs_c cArgs = Marshal.PtrToStructure<LeaderboardResultEventArgs_c>(leaderboardResultEventArgsPtr);
            Result = new LeaderboardResult(cArgs.Result);
        }
    }
}