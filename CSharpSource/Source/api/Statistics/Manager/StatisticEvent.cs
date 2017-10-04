// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    public partial class StatisticEvent
    {
        public StatisticEventType EventType { get; private set; }
        public StatisticEventArgs EventArgs { get; private set; }
        public XboxLiveUser User { get; private set; }
        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        
        // Used for mock services
        internal StatisticEvent(StatisticEventType type, XboxLiveUser user, StatisticEventArgs args)
        {
            EventType = type;
            EventArgs = args;
            User = user;
        }
    }
}