using Microsoft.Xbox.Services.Presence;
using System;

namespace Microsoft.Xbox.Services.Shared.Presence
{
    public class PresenceActivityData
    {
        public PresenceData PresenceData { get; set; }

        public MediaPresenceData MediaPresenceData { get; set; }

        public bool ShouldSerialize { get; set; }

        public PresenceActivityData() {

        }

        public PresenceActivityData(PresenceData presenceData, MediaPresenceData mediaPresenceData ) {

        }

        public string Serialize() {
            throw new NotImplementedException();
        }
    }
}