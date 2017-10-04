// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class StatisticEvent
    {
        internal StatisticEvent(IntPtr statEventPtr)
        {
            StatisticEvent_c cStatEvent = Marshal.PtrToStructure<StatisticEvent_c>(statEventPtr);

            EventType = cStatEvent.EventType;

            try
            {
                EventArgs = new LeaderboardResultEventArgs(cStatEvent.EventArgs);
            }
            catch (Exception)
            {
                // not LeaderboardResultEventArgs
            }

            User = new XboxLiveUser(cStatEvent.LocalUser);

            ErrorCode = cStatEvent.ErrorCode;
            ErrorMessage = cStatEvent.ErrorMessage;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StatisticEvent_c
        {
            [MarshalAs(UnmanagedType.U4)]
            public StatisticEventType EventType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr EventArgs;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr LocalUser;

            [MarshalAs(UnmanagedType.I4)]
            public int ErrorCode;

            [MarshalAs(UnmanagedType.LPStr)]
            public string ErrorMessage;
        }
    }
}
