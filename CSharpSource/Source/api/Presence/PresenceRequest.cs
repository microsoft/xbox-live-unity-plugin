using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xbox.Services.Presence;

namespace Microsoft.Xbox.Services.Shared.Presence
{
    public class PresenceTitleRequest
    {
        public bool IsUserActiveInTitle { get; set; }
        public PresenceData PresenceData { get; set; }

        public PresenceTitleRequest(bool isUserActiveInTitle, PresenceData presenceData)
        {
            this.IsUserActiveInTitle = isUserActiveInTitle;
            this.PresenceData = presenceData;
        }
    }
}
