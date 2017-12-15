// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.TitleStorage
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services.Shared.TitleStorage;
    using Microsoft.Xbox.Services.System;

    /// <summary>
    /// Services that manage title storage.
    /// </summary>
    public class TitleStorageService : ITitleStorageService
    {
        internal IntPtr pCXboxLiveContext;

        internal TitleStorageService(IntPtr pCXboxLiveContext)
        {
            this.pCXboxLiveContext = pCXboxLiveContext;
        }

        public const UInt32 DefaultUploadBlockSize = 256 * 1024;
        public const UInt32 DefaultDownloadBlockSize = 256 * 1024;

        /// <summary>
        /// Gets title storage quota information for the specified service configuration and storage type.
        /// For user storage types (TrustedPlatform and Json) the request will be made for the calling user's
        /// Xbox user Id.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="storageType">Type of the storage type</param>
        /// <returns>An instance of the <see cref="TitleStorageQuota"/> class with the amount of storage space allocated and used.</returns>
        public Task<TitleStorageQuota> GetQuotaAsync(string serviceConfigurationId, TitleStorageType storageType)
        {
            var tcs = new TaskCompletionSource<TitleStorageQuota>();
            Task.Run(() =>
            {
                var scid = MarshalingHelpers.StringToHGlobalUtf8(XboxLive.Instance.AppConfig.ServiceConfigurationId);

                int contextKey;
                var context = XsapiCallbackContext<object, TitleStorageQuota>.CreateContext(null, tcs, out contextKey);
                context.PointersToFree = new List<IntPtr> { scid };

                var xsapiResult = TitleStorageGetQuota(
                    this.pCXboxLiveContext, scid, storageType, GetQuotaComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void GetQuotaComplete(XSAPI_RESULT_INFO result, XSAPI_TITLE_STORAGE_QUOTA quota, IntPtr contextKey)
        {
            XsapiCallbackContext<object, TitleStorageQuota> context;
            if (XsapiCallbackContext<object, TitleStorageQuota>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.TaskCompletionSource.SetResult(new TitleStorageQuota(quota));
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }

                context.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of blob metadata objects under a given path for the specified service configuration, storage type and storage ID.
        /// </summary>
        /// <param name="serviceConfigurationId">The service configuration ID (SCID) of the title</param>
        /// <param name="storageType">The storage type to get blob metadata objects for.</param>
        /// <param name="blobPath">(Optional) The root path to enumerate.  Results will be for blobs contained in this path and all subpaths.</param>
        /// <param name="xboxUserId">The Xbox User ID of the title storage to enumerate. Pass nullptr when searching for GlobalStorage type data. (Optional)</param>
        /// <param name="skipItems">(Optional) The number of items to skip before returning results. (Optional)</param>
        /// <param name="maxItems">(Optional) The maximum number of items to return.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadataResult"/> class containing the list of enumerated blob metadata objects.</returns>
        public Task<TitleStorageBlobMetadataResult> GetBlobMetadataAsync(string serviceConfigurationId, TitleStorageType storageType, string blobPath, string xboxUserId, uint skipItems = 0, uint maxItems = 0)
        {
            var tcs = new TaskCompletionSource<TitleStorageBlobMetadataResult>();
            Task.Run(() =>
            {
                var scid = MarshalingHelpers.StringToHGlobalUtf8(serviceConfigurationId);
                var blobPathPtr = MarshalingHelpers.StringToHGlobalUtf8(blobPath);
                var xuidPtr = MarshalingHelpers.StringToHGlobalUtf8(xboxUserId);

                int contextKey;
                var context = XsapiCallbackContext<object, TitleStorageBlobMetadataResult>.CreateContext(null, tcs, out contextKey);
                context.PointersToFree = new List<IntPtr> { scid, blobPathPtr, xuidPtr };

                var xsapiResult = TitleStorageGetBlobMetadata(
                    this.pCXboxLiveContext, scid, storageType, blobPathPtr, xuidPtr, skipItems, maxItems, 
                    GetBlobMetadataComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        internal void GetBlobMetadataComplete(XSAPI_RESULT_INFO result, XSAPI_TITLE_STORAGE_BLOB_METADATA_RESULT payload, IntPtr contextKey)
        {
            XsapiCallbackContext<object, TitleStorageBlobMetadataResult> context;
            if (XsapiCallbackContext<object, TitleStorageBlobMetadataResult>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.TaskCompletionSource.SetResult(new TitleStorageBlobMetadataResult(payload, this));
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                context.Dispose();
            }
        }

        /// <summary>
        /// Deletes a blob from title storage.
        /// </summary>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to delete.</param>
        /// <param name="deleteOnlyIfEtagMatches">Specifies whether or not to have the delete operation check that the ETag matches before deleting the blob.</param>
        /// <returns>An empty task.</returns>
        public Task DeleteBlobAsync(TitleStorageBlobMetadata blobMetadata, bool deleteOnlyIfEtagMatches)
        {
            var tcs = new TaskCompletionSource<object>();
            Task.Run(() =>
            {
                int contextKey;
                XsapiCallbackContext<object, object>.CreateContext(this, tcs, out contextKey);

                var xsapiResult = TitleStorageDeleteBlob(this.pCXboxLiveContext, blobMetadata.metadataPtr, deleteOnlyIfEtagMatches,
                    DeleteBlobComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void DeleteBlobComplete(XSAPI_RESULT_INFO result, IntPtr contextKey)
        {
            XsapiCallbackContext<object, object> context;
            if (XsapiCallbackContext<object, object>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.TaskCompletionSource.SetResult(null);
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                context.Dispose();
            }
        }

        /// <summary>
        /// Downloads blob data from title storage.
        /// </summary>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to download.</param>
        /// <param name="etagMatchCondition">The ETag match condition used to determine if the blob should be downloaded.</param>
        /// <param name="selectQuery">A query string that contains a ConfigStorage filter string or JSONStorage json property name string to filter. (Optional)</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobResult"/> containing the blob content and an updated
        /// <see cref="TitleStorageBlobMetadata"/> object.</returns>
        public Task<TitleStorageBlobResult> DownloadBlobAsync(TitleStorageBlobMetadata blobMetadata, TitleStorageETagMatchCondition etagMatchCondition, string selectQuery)
        {
            return DownloadBlobAsync(blobMetadata, etagMatchCondition, selectQuery, TitleStorageService.DefaultDownloadBlockSize);
        }

        /// <summary>
        /// Downloads blob data from title storage.
        /// </summary>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to download.</param>
        /// <param name="etagMatchCondition">The ETag match condition used to determine if the blob should be downloaded.</param>
        /// <param name="selectQuery">A query string that contains a ConfigStorage filter string or JSONStorage json property name string to filter. (Optional)</param>
        /// <param name="preferredDownloadBlockSize">The preferred download block size in bytes for binary blobs. </param>
        /// <returns>An instance of the <see cref="TitleStorageBlobResult"/> containing the blob content and an updated
        /// <see cref="TitleStorageBlobMetadata"/> object.</returns>
        public Task<TitleStorageBlobResult> DownloadBlobAsync(TitleStorageBlobMetadata blobMetadata, TitleStorageETagMatchCondition etagMatchCondition, string selectQuery, uint preferredDownloadBlockSize)
        {
            var tcs = new TaskCompletionSource<TitleStorageBlobResult>();
            Task.Run(() =>
            {
                int contextKey;
                var context = XsapiCallbackContext<TitleStorageBlobMetadata, TitleStorageBlobResult>.CreateContext(blobMetadata, tcs, out contextKey);

                var buffer = Marshal.AllocHGlobal((int)blobMetadata.Length);
                var select = MarshalingHelpers.StringToHGlobalUtf8(selectQuery);
                var pPreferredDownloadBlockSize = GCHandle.Alloc(preferredDownloadBlockSize, GCHandleType.Pinned);

                context.PointersToFree = new List<IntPtr> { buffer, select };
                context.GCHandlesToFree = new List<GCHandle> { pPreferredDownloadBlockSize };

                var xsapiResult = TitleStorageDownloadBlob(
                    this.pCXboxLiveContext, blobMetadata.metadataPtr, buffer, (UInt32)blobMetadata.Length, etagMatchCondition, select, GCHandle.ToIntPtr(pPreferredDownloadBlockSize),
                    DownloadBlobComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void DownloadBlobComplete(XSAPI_RESULT_INFO result, XSAPI_TITLE_STORAGE_BLOB_RESULT blobResult, IntPtr contextKey)
        {
            XsapiCallbackContext<TitleStorageBlobMetadata, TitleStorageBlobResult> context;
            if (XsapiCallbackContext<TitleStorageBlobMetadata, TitleStorageBlobResult>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.Context.Refresh();

                    byte[] buffer = new byte[blobResult.cbBlobBuffer];
                    Marshal.Copy(blobResult.blobBuffer, buffer, 0, (int)blobResult.cbBlobBuffer);

                    context.TaskCompletionSource.SetResult(new TitleStorageBlobResult(context.Context, buffer));
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                context.Dispose();
            }
        }

        /// <summary>
        /// Upload blob data to title storage.
        /// </summary>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to upload.</param>
        /// <param name="blobBuffer">The Blob content to be uploaded.</param>
        /// <param name="etagMatchCondition">The ETag match condition used to determine if the blob data should be uploaded.</param>
        /// <param name="preferredDownloadBlockSize">The preferred upload block size in bytes for binary blobs. </param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadata"/> class with updated ETag and Length Properties.</returns>
        public Task<TitleStorageBlobMetadata> UploadBlobAsync(TitleStorageBlobMetadata blobMetadata, IReadOnlyList<byte> blobBuffer, TitleStorageETagMatchCondition etagMatchCondition, uint preferredUploadBlockSize)
        {
            var tcs = new TaskCompletionSource<TitleStorageBlobMetadata>();
            Task.Run(() =>
            {
                int contextKey;
                var context = XsapiCallbackContext<TitleStorageBlobMetadata, TitleStorageBlobMetadata>.CreateContext(blobMetadata, tcs, out contextKey);

                var buffer = Marshal.AllocHGlobal(blobBuffer.Count);
                Marshal.Copy(Enumerable.ToArray<byte>(blobBuffer), 0, buffer, blobBuffer.Count);
                context.PointersToFree = new List<IntPtr> { buffer };

                var xsapiResult = TitleStorageUploadBlob(
                    this.pCXboxLiveContext, blobMetadata.metadataPtr, buffer, (UInt32)blobBuffer.Count, etagMatchCondition, IntPtr.Zero, UploadBlobComplete, 
                    (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void UploadBlobComplete(XSAPI_RESULT_INFO result, IntPtr pBlobMetadata, IntPtr contextKey)
        {
            XsapiCallbackContext<TitleStorageBlobMetadata, TitleStorageBlobMetadata> context;
            if (XsapiCallbackContext<TitleStorageBlobMetadata, TitleStorageBlobMetadata>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.Context.Refresh();
                    context.TaskCompletionSource.SetResult(context.Context);
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                context.Dispose();
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_GET_QUOTA_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result, 
            XSAPI_TITLE_STORAGE_QUOTA quota, 
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageGetQuota(
            IntPtr xboxLiveContext, 
            IntPtr serviceConfigurationId,
            TitleStorageType storageType,
            XSAPI_GET_QUOTA_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void XSAPI_GET_BLOB_METADATA_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            XSAPI_TITLE_STORAGE_BLOB_METADATA_RESULT payload,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageGetBlobMetadata(
            IntPtr xboxLiveContext,
            IntPtr serviceConfigurationId,
            TitleStorageType storageType,
            IntPtr blobPath,
            IntPtr xboxUserId,
            UInt32 skipItems,
            UInt32 maxItems,
            XSAPI_GET_BLOB_METADATA_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageCreateBlobMetadata(
            IntPtr serviceConfigurationId,
            TitleStorageType storageType,
            IntPtr blobPath,
            TitleStorageBlobType blobType,
            IntPtr xboxUserId,
            IntPtr displayName,
            IntPtr etag,
            IntPtr ppBlobMetadata);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageReleaseBlobMetadata(
            IntPtr serviceConfigurationId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_DELETE_BLOB_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageDeleteBlob(
            IntPtr xboxLiveContext,
            IntPtr blobMetadataPointer,
            bool deleteOnlyIfEtagMatches,
            XSAPI_DELETE_BLOB_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_DOWNLOAD_BLOB_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            XSAPI_TITLE_STORAGE_BLOB_RESULT blobResult,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageDownloadBlob(
            IntPtr xboxLiveContext,
            IntPtr blobMetadataPointer,
            IntPtr blobBuffer,
            UInt32 cbBlobBuffer,
            TitleStorageETagMatchCondition etagMatchCondition,
            IntPtr selectQuery,
            IntPtr preferredDownloadBlockSize,
            XSAPI_DOWNLOAD_BLOB_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_UPLOAD_BLOB_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            IntPtr pBlobMetadata,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TitleStorageUploadBlob(
            IntPtr xboxLiveContext,
            IntPtr blobMetadataPointer,
            IntPtr blobBuffer,
            UInt32 cbBlobBuffer,
            TitleStorageETagMatchCondition etagMatchCondition,
            IntPtr preferredUploadBlockSize,
            XSAPI_UPLOAD_BLOB_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);
    }
}
