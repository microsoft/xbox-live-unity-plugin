namespace Microsoft.Xbox.Services.Shared.TitleStorage
{
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.TitleStorage;

    /// <summary>
    /// Services that manage title storage.
    /// </summary>
    internal interface ITitleStorageService
    {
        /// <summary>
        /// Gets title storage quota information for the specified service configuration and storage type.
        /// For user storage types (TrustedPlatform and Json) the request will be made for the calling user's
        /// Xbox user Id.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="storageType">Type of the storage type</param>
        /// <returns>An instance of the <see cref="TitleStorageQuota"/> class with the amount of storage space allocated and used.</returns>
        Task<TitleStorageQuota> GetQuotaAsync(XboxLiveUser user, TitleStorageType storageType);

        /// <summary>
        /// Gets a list of blob metadata objects under a given path for the specified service configuration, storage type and storage ID.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="storageType">The storage type to get blob metadata objects for.</param>
        /// <param name="blobPath">(Optional) The root path to enumerate.  Results will be for blobs contained in this path and all subpaths.</param>
        /// <param name="skipItems">(Optional) The number of items to skip before returning results. (Optional)</param>
        /// <param name="maxItems">(Optional) The maximum number of items to return.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadataResult"/> class containing the list of enumerated blob metadata objects.</returns>
        Task<TitleStorageBlobMetadataResult> GetBlobMetadataAsync(XboxLiveUser user, TitleStorageType storageType, string blobPath, uint skipItems, uint maxItems);

        /// <summary>
        /// Deletes a blob from title storage.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to delete.</param>
        /// <param name="blobQueryProperties">An instance of the <see cref="BlobQueryProperties"/> class with properties of the download query.</param>
        /// <returns>An empty task.</returns>
        Task DeleteBlobAsync(XboxLiveUser user, TitleStorageBlobMetadata blobMetadata, BlobQueryProperties blobQueryProperties);

        /// <summary>
        /// Downloads blob data from title storage.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to download.</param>
        /// <param name="blobQueryProperties">An instance of the <see cref="BlobQueryProperties"/> class with properties of the download query.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobResult"/> containing the blob content and an updated
        /// <see cref="TitleStorageBlobMetadata"/> object.</returns>
        Task<TitleStorageBlobResult> DownloadBlobAsync(XboxLiveUser user, TitleStorageBlobMetadata blobMetadata, BlobQueryProperties blobQueryProperties);

        /// <summary>
        /// Upload blob data to title storage.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to upload.</param>
        /// <param name="blobBuffer">The Blob content to be uploaded.</param>
        /// <param name="blobQueryProperties">An instance of the <see cref="BlobQueryProperties"/> class with properties of the upload query.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadata"/> class with updated ETag and Length Properties.</returns>
        Task<TitleStorageBlobMetadata> UploadBlobAsync(XboxLiveUser user, TitleStorageBlobMetadata blobMetadata, List<byte> blobBuffer, BlobQueryProperties blobQueryProperties);

    }
}
