// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.TitleStorage
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.System;

    /// <summary>
    /// Metadata about a blob.
    /// </summary>
    public class TitleStorageBlobMetadata
    {
        internal IntPtr metadataPtr;

        /// <summary>
        /// Gets the number of bytes of the blob data.
        /// </summary>
        public ulong Length { get; private set; }

        /// <summary>
        /// [optional] Timestamp assigned by the client.
        /// </summary>
        public DateTimeOffset ClientTimeStamp { get; private set; }

        /// <summary>
        /// ETag for the file used in read and write requests.
        /// </summary>
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
        public string BlobPath { get; private set; }

        /// <summary>
        /// The service configuration ID of the title
        /// </summary>
        public string ServiceConfigurationId { get; private set; }

        /// <summary>
        /// The Xbox User ID of the player that this file belongs to.
        /// This value is null for Global and Session files.
        /// </summary>
        public string XboxUserId { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        /// <param name="serviceConfigurationId">The service configuration ID (SCID) of the title</param>
        /// <param name="storageType">Type of storage.</param>
        /// <param name="blobPath">Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").</param>
        /// <param name="blobType">Type of blob data. Possible values are: Binary, Json, and Config.</param>
        /// <param name="xboxUserId">The Xbox User ID of the title storage to enumerate. Ignored when dealing with GlobalStorage, so passing nullptr is acceptable in that case. (Optional)</param>
        /// <param name="displayName">[optional] Friendly display name to show in app UI.</param>
        /// <param name="eTag">ETag for the file used in read and write requests.</param>
        /// <param name="length">Length of the content of the blob</param>
        public TitleStorageBlobMetadata(string serviceConfigurationId, TitleStorageType storageType, string blobPath, TitleStorageBlobType blobType, string xboxUserId, string displayName, string eTag, ulong length)
        {
            if (string.IsNullOrEmpty(blobPath))
                throw new ArgumentNullException("blobPath");

            this.ServiceConfigurationId = serviceConfigurationId;
            this.StorageType = storageType;
            this.BlobType = blobType;
            this.BlobPath = blobPath;
            this.DisplayName = displayName;
            this.ETag = eTag;
            this.Length = length;
            this.XboxUserId = xboxUserId;

            CreateCMetadata();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        /// <param name="serviceConfigurationId">The service configuration ID (SCID) of the title</param>
        /// <param name="storageType">Type of storage.</param>
        /// <param name="blobPath">Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").</param>
        /// <param name="blobType">Type of blob data. Possible values are: Binary, Json, and Config.</param>
        /// <param name="xboxUserId">The Xbox User ID of the title storage to enumerate. Ignored when dealing with GlobalStorage, so passing nullptr is acceptable in that case. (Optional)</param>
        /// <param name="displayName">[optional] Friendly display name to show in app UI.</param>
        /// <param name="eTag">ETag for the file used in read and write requests.</param>
        public TitleStorageBlobMetadata(string serviceConfigurationId, TitleStorageType storageType, string blobPath, TitleStorageBlobType blobType, string xboxUserId, string displayName, string eTag)
        {
            if (string.IsNullOrEmpty(blobPath))
                throw new ArgumentNullException("blobPath");

            this.ServiceConfigurationId = serviceConfigurationId;
            this.StorageType = storageType;
            this.BlobType = blobType;
            this.BlobPath = blobPath;
            this.DisplayName = displayName;
            this.ETag = eTag;
            this.XboxUserId = xboxUserId;

            CreateCMetadata();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleStorageBlobMetadata"/> class.
        /// </summary>
        /// <param name="serviceConfigurationId">The service configuration ID (SCID) of the title</param>
        /// <param name="storageType">Type of storage.</param>
        /// <param name="blobPath">Blob path is a unique string that conforms to a SubPath\file format (example: "foo\bar\blob.txt").</param>
        /// <param name="blobType">Type of blob data. Possible values are: Binary, Json, and Config.</param>
        /// <param name="xboxUserId">The Xbox User ID of the title storage to enumerate. Ignored when dealing with GlobalStorage, so passing nullptr is acceptable in that case. (Optional)</param>
        public TitleStorageBlobMetadata(string serviceConfigurationId, TitleStorageType storageType, string blobPath, TitleStorageBlobType blobType, string xboxUserId)
        {
            if (string.IsNullOrEmpty(blobPath))
                throw new ArgumentNullException("blobPath");

            this.ServiceConfigurationId = serviceConfigurationId;
            this.StorageType = storageType;
            this.BlobType = blobType;
            this.BlobPath = blobPath;
            this.XboxUserId = xboxUserId;

            CreateCMetadata();
        }

        internal TitleStorageBlobMetadata(IntPtr structPtr)
        {
            this.metadataPtr = structPtr;
            this.Refresh();
        }

        ~TitleStorageBlobMetadata()
        {
            TitleStorageReleaseBlobMetadata(metadataPtr);
        }

        private void CreateCMetadata()
        {
            var scid = MarshalingHelpers.StringToHGlobalUtf8(this.ServiceConfigurationId);
            var path = MarshalingHelpers.StringToHGlobalUtf8(this.BlobPath);
            var xuid = MarshalingHelpers.StringToHGlobalUtf8(this.XboxUserId);
            var displayName = MarshalingHelpers.StringToHGlobalUtf8(this.DisplayName);
            var etag = MarshalingHelpers.StringToHGlobalUtf8(this.ETag);

            IntPtr clientTimePtr = IntPtr.Zero;
            if (this.ClientTimeStamp != null)
            {
                var clientTime = this.ClientTimeStamp.ToUnixTimeSeconds();
                clientTimePtr = Marshal.AllocHGlobal(MarshalingHelpers.SizeOf<UInt64>());
                Marshal.WriteInt64(clientTimePtr, clientTime);
            }

            TitleStorageCreateBlobMetadata(scid, StorageType, path, BlobType, xuid, displayName, etag, clientTimePtr, out metadataPtr);

            Marshal.FreeHGlobal(scid);
            Marshal.FreeHGlobal(path);
            Marshal.FreeHGlobal(xuid);
            Marshal.FreeHGlobal(displayName);
            Marshal.FreeHGlobal(etag);
            Marshal.FreeHGlobal(clientTimePtr);
        }

        internal void Refresh()
        {
            var CStruct = MarshalingHelpers.PtrToStructure<XSAPI_TITLE_STORAGE_BLOB_METADATA>(this.metadataPtr);

            this.ServiceConfigurationId = MarshalingHelpers.Utf8ToString(CStruct.serviceConfigurationId);
            this.BlobPath = MarshalingHelpers.Utf8ToString(CStruct.blobPath);
            this.BlobType = CStruct.blobType;
            this.StorageType = CStruct.storageType;
            this.DisplayName = MarshalingHelpers.Utf8ToString(CStruct.displayName);
            this.ETag = MarshalingHelpers.Utf8ToString(CStruct.ETag);
            this.Length = CStruct.length;
            this.XboxUserId = MarshalingHelpers.Utf8ToString(CStruct.xboxUserId);
            this.ClientTimeStamp = MarshalingHelpers.FromUnixTimeSeconds(CStruct.clientTimestamp);
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageCreateBlobMetadata(
            IntPtr serviceConfigurationId,
            TitleStorageType storageType,
            IntPtr blobPath,
            TitleStorageBlobType blobType,
            IntPtr xboxUserId,
            IntPtr displayName,
            IntPtr etag,
            IntPtr pClientTimestamp,
            out IntPtr ppMetadata);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageReleaseBlobMetadata(
            IntPtr pMetadata);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class XSAPI_TITLE_STORAGE_BLOB_METADATA
    {
        public IntPtr blobPath;
        public TitleStorageBlobType blobType;
        public TitleStorageType storageType;
        public IntPtr displayName;
        public IntPtr ETag;
        public Int64 clientTimestamp;
        public UInt64 length;
        public IntPtr serviceConfigurationId;
        public IntPtr xboxUserId;
    }
}
