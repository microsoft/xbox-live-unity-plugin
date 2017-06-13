// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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