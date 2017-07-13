using Microsoft.Xbox.Services.Presence;
using System;

namespace Microsoft.Xbox.Services.Shared.Presence
{
    public class MediaPresenceData
    {
        public string MediaId { get; set; }

        public PresenceMediaIdType MediaIdType { get; set; }

        public bool ShouldSerialize { get; set; }

        public string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}