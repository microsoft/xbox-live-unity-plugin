// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.ConnectedStorage
{
    using global::System.Collections.Generic;

    public class StorageContainerQueryResult
    {
        public GameSaveStatus Status { get; set; }

        public List<StorageContainerInfo> Value { get; set; }
    }
}