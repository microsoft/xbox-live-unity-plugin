using System;

namespace Microsoft.Xbox.Services.ConnectedStorage
{
    public class StorageContainerInfo
    {

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public bool NeedsSync { get; set; }

        public DateTimeOffset LastModifiedTime { get; set; }

        public ulong TotalSize { get; set; }
    }
}