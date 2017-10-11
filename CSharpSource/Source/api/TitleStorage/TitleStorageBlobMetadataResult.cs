// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 


namespace Microsoft.Xbox.Services.TitleStorage
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services.System;

    public class TitleStorageBlobMetadataResult
    {
        private TitleStorageService titleStorageService;
        private List<TitleStorageBlobMetadata> items;
        private XSAPI_TITLE_STORAGE_BLOB_METADATA_RESULT metadataResultStruct;

        /// <summary>
        /// List of <see cref="TitleStorageBlobMetadata"/> objects containing metadata of the included blobs.
        /// </summary>
        public List<TitleStorageBlobMetadata> Items
        {
            get
            {
                if (items == null)
                {
                    items = new List<TitleStorageBlobMetadata>((int)metadataResultStruct.itemCount);

                    IntPtr metadataPtr = metadataResultStruct.items;
                    for (ulong i = 0; i < metadataResultStruct.itemCount; ++i)
                    {
                        items.Add(new TitleStorageBlobMetadata(metadataPtr));
#if DOTNET_3_5
                        metadataPtr = new IntPtr(metadataPtr.ToInt64() + Marshal.SizeOf(new XSAPI_TITLE_STORAGE_BLOB_METADATA()));
#else
                        metadataPtr = IntPtr.Add(metadataPtr, Marshal.SizeOf<XSAPI_TITLE_STORAGE_BLOB_METADATA>());
#endif
                    }
                }
                return items;
            }
        }

        /// <summary>
        /// Indicates if there is additional data to retrieve from a GetNextAsync call
        /// </summary>
        public bool HasNext()
        {
            return metadataResultStruct.hasNext;
        }

        /// <summary>
        /// Gets the metadata of the next group of blobs
        /// </summary>
        /// <param name="maxItems">[Optional] The maximum number of items the result can contain.  Pass 0 to attempt retrieving all items.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadataResult"/> class containing metadata of the included blobs.</returns>
        public Task<TitleStorageBlobMetadataResult> GetNextAsync(uint maxItems = 0)
        {
            var tcs = new TaskCompletionSource<TitleStorageBlobMetadataResult>();
            Task.Run(() =>
            {
                int contextKey;
                var context = XsapiCallbackContext<object, TitleStorageBlobMetadataResult>.CreateContext(null, tcs, out contextKey);

                var xsapiResult = TitleStorageBlobMetadataResultGetNext(metadataResultStruct, maxItems, titleStorageService.GetBlobMetadataComplete,
                    (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        internal TitleStorageBlobMetadataResult(XSAPI_TITLE_STORAGE_BLOB_METADATA_RESULT cObject, TitleStorageService service)
        {
            items = null;
            this.titleStorageService = service;
            this.metadataResultStruct = cObject;
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageBlobMetadataResultGetNext(
            XSAPI_TITLE_STORAGE_BLOB_METADATA_RESULT metadataResult,
            UInt32 maxItems,
            TitleStorageService.XSAPI_GET_BLOB_METADATA_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XSAPI_TITLE_STORAGE_BLOB_METADATA_RESULT
    {
        public IntPtr items;

        public UInt64 itemCount;

        [MarshalAsAttribute(UnmanagedType.U1)]
        public bool hasNext;
    }
}
