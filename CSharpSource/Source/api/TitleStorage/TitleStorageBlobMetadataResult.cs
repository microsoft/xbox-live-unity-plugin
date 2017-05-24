// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.TitleStorage
{
    using Microsoft.Xbox.Services.Shared.TitleStorage;

    public class TitleStorageBlobMetadataResult
    {
        private static XboxLiveUser User;

        private TitleStorageType storageType;

        private string blobPath;

        private string continuationToken;

        /// <summary>
        /// List of <see cref="TitleStorageBlobMetadata"/> objects containing metadata of the included blobs.
        /// </summary>
        public List<TitleStorageBlobMetadata> Items { get; private set; }

        /// <summary>
        /// Indicates if there is additional data to retrieve from a GetNextAsync call
        /// </summary>
        public bool HasNext()
        {
            return string.IsNullOrEmpty(this.continuationToken);
        }

        /// <summary>
        /// Gets the metadata of the next group of blobs
        /// </summary>
        /// <param name="titleStorageService">Title Storage Service to retrieve next items</param>
        /// <param name="maxItems">[Optional] The maximum number of items the result can contain.  Pass 0 to attempt retrieving all items.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadataResult"/> class containing metadata of the included blobs.</returns>
        public Task<TitleStorageBlobMetadataResult> GetNextAsync(TitleStorageService titleStorageService, uint maxItems = 0)
        {
            if (string.IsNullOrEmpty(this.continuationToken))
            {
                throw new ArgumentNullException("continuationToken");
            }
            return titleStorageService.InternalGetBlobMetadata(
                TitleStorageBlobMetadataResult.User,
                this.storageType, 
                this.blobPath,  
                0, 
                maxItems, 
                this.continuationToken);
        }

        /// <summary>
        /// Deserializes the TitleStorageBlobMetadataResult from JSON
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="storageType">Type of title storage </param>
        /// <param name="xboxUser">The Xbox User of the player the files belongs to.</param>
        /// <param name="blobPath">The full path to to the blob.  examples: "gameconfig.json" and "user/settings/playerconfiguration.json".</param>
        /// <returns></returns>
        public static TitleStorageBlobMetadataResult Deserialize(
            string json,  
            TitleStorageType storageType,
            XboxLiveUser xboxUser,
            string blobPath)
        { 
            var titleStorageBlobMetadataResultInfo = JsonSerialization.FromJson<TitleStorageBlobMetadataResultInfo>(json);

            TitleStorageBlobMetadataResult.User = xboxUser;
            var titleMetadataResult = new TitleStorageBlobMetadataResult
                                      {
                                          Items = new List<TitleStorageBlobMetadata>(),
                                          storageType = storageType,
                                          blobPath = blobPath
                                      };
            foreach (var blob in titleStorageBlobMetadataResultInfo.Blobs)
            {
                var blobFileName = blob.BlobPath;

                // Since the blobPath returned from the service contains the type of the blob
                // such as (foo\bar\blob.txt,json)
                // It needs to be split out so the blobPath is only the first part and the
                // blob type is JSON.
                var nameParts = blobFileName.Split(',');
                blobFileName = nameParts[0];
                var blobFileType = (TitleStorageBlobType) Enum.Parse(typeof(TitleStorageBlobType), nameParts[1], true);

                var newTitleStorageBlobMetadata = new TitleStorageBlobMetadata(storageType, blobFileName, blobFileType, blob.DisplayName, blob.ETag);
                titleMetadataResult.Items.Add(newTitleStorageBlobMetadata);
            }

            if (titleStorageBlobMetadataResultInfo.PagingInfo != null)
            {
                titleMetadataResult.continuationToken = titleStorageBlobMetadataResultInfo.PagingInfo.ContinuationToken;
            }
            
            return titleMetadataResult;
        }
    }
}
