namespace Microsoft.Xbox.Services.ConnectedStorage
{
    using global::System.Collections.Generic;

    public class StorageContainerQueryResult
    {
        public GameSaveStatus Status { get; set; }

        public List<StorageContainerInfo> Value { get; set; }
    }
}