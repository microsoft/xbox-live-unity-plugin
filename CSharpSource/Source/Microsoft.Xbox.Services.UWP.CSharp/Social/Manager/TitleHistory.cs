// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    public partial class TitleHistory
    {
        internal TitleHistory(IntPtr titleHistoryPtr)
        {
            TITLE_HISTORY cTitleHistory = Marshal.PtrToStructure<TITLE_HISTORY>(titleHistoryPtr);
            HasUserPlayed = cTitleHistory.UserHasPlayed;

            LastTimeUserPlayed = DateTimeOffset.FromUnixTimeSeconds(cTitleHistory.LastTimeUserPlayed);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TITLE_HISTORY
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool UserHasPlayed;

            [MarshalAs(UnmanagedType.I8)]
            public long LastTimeUserPlayed;
        }
    }
}
