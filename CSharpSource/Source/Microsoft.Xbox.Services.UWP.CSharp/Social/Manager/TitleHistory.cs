﻿// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    public partial class TitleHistory
    {
        internal TitleHistory(IntPtr titleHistoryPtr)
        {
            TitleHistory_c cTitleHistory = Marshal.PtrToStructure<TitleHistory_c>(titleHistoryPtr);
            HasUserPlayed = Convert.ToBoolean(cTitleHistory.UserHasPlayed);

            // todo test
            LastTimeUserPlayed = DateTimeOffset.FromUnixTimeSeconds(cTitleHistory.LastTimeUserPlayed);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TitleHistory_c
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte UserHasPlayed;

        [MarshalAs(UnmanagedType.I8)]
        public long LastTimeUserPlayed;
    }
}