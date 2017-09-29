// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using static Microsoft.Xbox.Services.Statistics.Manager.StatsManager;

    public class StatEvent
    {
        public StatEventType EventType { get; private set; }
        public StatEventArgs EventArgs { get; private set; }
        public XboxLiveUser User { get; private set; }
        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public StatEvent(StatEventType eventType, XboxLiveUser user, StatEventArgs args)
        {
            this.EventType = eventType;
            this.User = user;
            this.EventArgs = args;
        }

        internal StatEvent(IntPtr statEventPtr)
        {
            StatEvent_c cStatEvent = Marshal.PtrToStructure<StatEvent_c>(statEventPtr);

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
    }
}