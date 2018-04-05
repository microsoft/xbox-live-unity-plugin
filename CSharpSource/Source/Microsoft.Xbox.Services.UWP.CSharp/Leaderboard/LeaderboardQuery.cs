// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class LeaderboardQuery
    {
        /// <summary>
        /// Create a new query
        /// </summary>
        [DllImport(XboxLive.FlatCDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr LeaderboardQueryCreate();
        public LeaderboardQuery()
        {
            IntPtr ptr = LeaderboardQueryCreate();
            pImpl = new LeaderboardQueryImpl(ptr, this);
        }
    }
}
