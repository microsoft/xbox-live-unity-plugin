// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.TitleStorage
{
    /// <summary>Defines values used to indicate the ETag match condition used when downloading, uploading or deleting title storage data.</summary>
    public enum TitleStorageETagMatchCondition : int
    {
        /// <summary>There is no match condition.</summary>
        NotUsed,

        /// <summary>Perform the request if the Etag value specified matches the service value.</summary>
        IfMatch,

        /// <summary>Perform the request if the Etag value specified does not match the service value.</summary>
        IfNotMatch
    }

}
