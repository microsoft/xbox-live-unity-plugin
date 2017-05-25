// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.TitleStorage
{
    using global::System;

    using Newtonsoft.Json;

    /// <summary>
    /// Metadata about a blob.
    /// </summary>
    public class TitleStorageBlobMetadata
    {
        /// <summary>
        /// Gets the number of bytes of the blob data.
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public ulong Length { get; private set; }

        /// <summary>
        /// [optional] Timestamp assigned by the client.
        /// </summary>
        public DateTimeOffset? ClientTimeStamp { get; private set; }

        /// <summary>
        /// ETag for the file used in read and write requests.
        /// </summary>
        [JsonProperty(PropertyName = "etag")]
        public string ETag { get; private set; }

        /// <summary>
        /// [optional] Friendly display name to show in app UI.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Type of storage.
        /// </summary>
        public TitleStorageType StorageType { get; private set; }

        /// <summary>
        /// Type of blob data. Possible values are: Binary, Json, and Config.
        /// </summary>
        public TitleStorageBlobType BlobType { get; private set; }

        /// <summary>
        /// Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").
        /// </summary>
        [JsonProperty(PropertyName = "fileName")]
        public string BlobPath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        public TitleStorageBlobMetadata()
        {
            this.StorageType = TitleStorageType.TrustedPlatform;
            this.BlobType = TitleStorageBlobType.Unknown;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        /// <param name="storageType">Type of storage.</param>
        /// <param name="blobPath">Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").</param>
        /// <param name="blobType">Type of blob data. Possible values are: Binary, Json, and Config.</param>
        /// <param name="displayName">[optional] Friendly display name to show in app UI.</param>
        /// <param name="eTag">ETag for the file used in read and write requests.</param>
        /// <param name="length">Length of the content of the blob</param>
        public TitleStorageBlobMetadata(
            TitleStorageType storageType,
            string blobPath,
            TitleStorageBlobType blobType,
            string displayName,
            string eTag,
            ulong length)
        {

            if (string.IsNullOrEmpty(blobPath))
                throw new ArgumentNullException("blobPath");

            this.StorageType = storageType;
            this.BlobType = blobType;
            this.BlobPath = blobPath;
            this.DisplayName = displayName;
            this.ETag = eTag;
            this.Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        /// <param name="storageType">Type of storage.</param>
        /// <param name="blobPath">Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").</param>
        /// <param name="blobType">Type of blob data. Possible values are: Binary, Json, and Config.</param>
        /// <param name="displayName">[optional] Friendly display name to show in app UI.</param>
        /// <param name="eTag">ETag for the file used in read and write requests.</param>
        public TitleStorageBlobMetadata( 
            TitleStorageType storageType, 
            string blobPath, 
            TitleStorageBlobType blobType, 
            string displayName, 
            string eTag) {

            if (string.IsNullOrEmpty(blobPath))
                throw new ArgumentNullException("blobPath");

            this.StorageType = storageType;
            this.BlobType = blobType;
            this.BlobPath = blobPath;
            this.DisplayName = displayName;
            this.ETag = eTag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        /// <param name="storageType">Type of storage.</param>
        /// <param name="blobPath">Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").</param>
        /// <param name="blobType">Type of blob data. Possible values are: Binary, Json, and Config.</param>
        public TitleStorageBlobMetadata(
            TitleStorageType storageType, 
            string blobPath, 
            TitleStorageBlobType blobType) {

            if (string.IsNullOrEmpty(blobPath))
                throw new ArgumentNullException("blobPath");

            this.StorageType = storageType;
            this.BlobType = blobType;
            this.BlobPath = blobPath;

        }

        /// <summary>
        /// Sets the Length and Client File Time Properties
        /// </summary>
        /// <param name="length">The number of bytes of blob data</param>
        /// <param name= "eTag">ETag for the file used in read and write requests</param>
        /// <param name="clientFileTime">Timestamp assigned by the client</param>
        public void SetBlobMetadataProperties(uint length, string eTag = "", DateTimeOffset? clientFileTime = null)
        {
            this.Length = length;
            this.ClientTimeStamp = clientFileTime;
            this.ETag = eTag;
        }
    }
}
