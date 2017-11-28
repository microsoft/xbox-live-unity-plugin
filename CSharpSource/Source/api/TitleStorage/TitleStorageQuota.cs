// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.TitleStorage
{
    using global::System;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// Returns the amount of storage space allocated and used.
    /// </summary>
    public class TitleStorageQuota
    {
        /// <summary>
        /// Maximum number of bytes that can be used in title storage of type StorageType.
        /// Note that this is a soft limit and the used bytes may actually exceed this value.
        /// </summary>
        public ulong QuotaBytes { get; set; }

        /// <summary>
        /// Number of bytes used in title storage of type StorageType.
        /// </summary>
        public ulong UsedBytes { get; set; }

        /// <summary>
        /// The TitleStorageType associated with the quota.
        /// </summary>
        public TitleStorageType StorageType { get; private set; }


        /// <summary>
        /// Initialze a TitleStorageQuota from the corresponding C object
        /// </summary>
        internal TitleStorageQuota(XSAPI_TITLE_STORAGE_QUOTA quotaStruct)
        {
            QuotaBytes = quotaStruct.QuotaBytes;
            UsedBytes = quotaStruct.UsedBytes;
            StorageType = quotaStruct.storageType;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XSAPI_TITLE_STORAGE_QUOTA
    {
        public IntPtr ServiceConfigurationId;
        public TitleStorageType storageType;
        public IntPtr XboxUserId;
        public UInt64 UsedBytes;
        public UInt64 QuotaBytes;
    }
}
