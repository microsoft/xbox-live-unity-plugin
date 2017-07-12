using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xbox.Services.Shared.Presence
{
    public class UserPresenceWriterStatus
    {
        public bool ShouldRemove { get; set; }

        public float HeartBeatIntervalInMins { get; set; }
    }
}
