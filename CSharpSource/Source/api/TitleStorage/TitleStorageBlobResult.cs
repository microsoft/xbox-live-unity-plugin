// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.TitleStorage
{
    /// <summary>
    /// Blob data returned from the cloud.
    /// </summary>
    public class TitleStorageBlobResult
    {
        /// <summary>
        /// Updated TitleStorageBlobMetadata object following an upload or download.
        /// </summary>
        public TitleStorageBlobMetadata BlobMetadata { get; private set; }

        /// <summary>
        /// The contents of the title storage blob.
        /// </summary>
        public byte[] BlobBuffer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobResult"/> class.
        /// </summary>
        /// <param name="blobMetadata">Updated TitleStorageBlobMetadata object following an upload or download.</param>
        /// <param name="blobBuffer">The contents of the title storage blob.</param>
        public TitleStorageBlobResult(TitleStorageBlobMetadata blobMetadata, byte[] blobBuffer)
        {
            this.BlobMetadata = blobMetadata;
            this.BlobBuffer = blobBuffer;
        }
    }
}
