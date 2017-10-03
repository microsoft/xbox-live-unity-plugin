// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using static Microsoft.Xbox.Services.Statistics.Manager.StatisticManager;

    public class StatisticEvent
    {
        public StatisticEventType EventType { get; private set; }
        public StatisticEventArgs EventArgs { get; private set; }
        public XboxLiveUser User { get; private set; }
        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        
        internal StatisticEvent(IntPtr statEventPtr)
        {
            StatEvent_c cStatEvent = Marshal.PtrToStructure<StatEvent_c>(statEventPtr);

            EventType = cStatEvent.EventType;

            try
            {
                EventArgs = new LeaderboardResultEventArgs(cStatEvent.EventArgs);
            }
            catch (Exception e)
            {
                // not LeaderboardResultEventArgs
            }

            User = new XboxLiveUser(cStatEvent.LocalUser);

            ErrorCode = cStatEvent.ErrorCode;
            ErrorMessage = cStatEvent.ErrorMessage;
        }

        // Used for mock services
        internal StatisticEvent(StatisticEventType type, XboxLiveUser user, StatisticEventArgs args)
        {
            EventType = type;
            EventArgs = args;
            User = user;
        }
    }
}