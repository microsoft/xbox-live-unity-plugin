// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.TitleStorage
{
    using Microsoft.Xbox.Services.Shared.TitleStorage;

    using Newtonsoft.Json;

    /// <summary>
    /// Returns the amount of storage space allocated and used.
    /// </summary>
    public class TitleStorageQuota
    {
        /// <summary>
        /// Maximum number of bytes that can be used in title storage of type StorageType.
        /// Note that this is a soft limit and the used bytes may actually exceed this value.
        /// </summary>
        [JsonProperty(PropertyName = "quotaBytes")]
        public ulong QuotaBytes { get; set; }

        /// <summary>
        /// Number of bytes used in title storage of type StorageType.
        /// </summary>
        [JsonProperty(PropertyName = "usedBytes")]
        public ulong UsedBytes { get; set; }

        /// <summary>
        /// The TitleStorageType associated with the quota.
        /// </summary>
        public TitleStorageType StorageType { get; private set; }

        /// <summary>
        /// Deserializes the Title Storage Quota from JSON
        /// </summary>
        /// <param name="json">The JSON String to be deserialized</param>
        /// <param name="storageType">The Title Storage Type</param>
        /// <returns>Title storage quota with the amount of storage space allocated and used.</returns>
        public static TitleStorageQuota Deserialize(
            string json, 
            TitleStorageType storageType)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var quotaInfoResult = JsonSerialization.FromJson<QuotaInfoResult>(json);
                var quotaInfo = quotaInfoResult.QuotaInfo;
                quotaInfo.StorageType = storageType;
                return quotaInfo;
            }
            return null;
        }
    }
}
