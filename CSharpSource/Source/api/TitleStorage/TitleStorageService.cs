// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.TitleStorage
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Text;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Shared.TitleStorage;

    /// <summary>
    /// Services that manage title storage.
    /// </summary>
    public class TitleStorageService: ITitleStorageService
    {
        private static readonly Uri TitleStorageBaseUri = new Uri("https://titlestorage.xboxlive.com");

        private const string TitleStorageApiContract = "1";
        private const string ContentTypeHeaderValue = "application/octet-stream";
        private const string ETagHeaderName = "ETag";
        private const string IfMatchHeaderName = "If-Match";
        private const string IfNoneHeaderName = "If-None-Match";

        private const uint MaxUploadBlockSize = 4 * 1024 * 1024;
        private const uint MinUploadBlockSize = 1024;
        private const uint MinDownloadBlockSize = 1024;

        /// <summary>
        /// Gets title storage quota information for the specified service configuration and storage type.
        /// For user storage types (TrustedPlatform and Json) the request will be made for the calling user's
        /// Xbox user Id.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="storageType">Type of the storage type</param>
        /// <returns>An instance of the <see cref="TitleStorageQuota"/> class with the amount of storage space allocated and used.</returns>
        public Task<TitleStorageQuota> GetQuotaAsync(XboxLiveUser user, TitleStorageType storageType)
        {
            var subQuery = this.GetTitleStorageSubpath(user, storageType);
            var httpRequest = XboxLiveHttpRequest.Create(HttpMethod.Get, TitleStorageBaseUri.ToString(), subQuery);
            httpRequest.ContractVersion = TitleStorageApiContract;
            httpRequest.XboxLiveAPI = XboxLiveAPIName.GetQuota;

            return httpRequest.GetResponseWithAuth(user).ContinueWith(
                responseTask => this.HandleStorageQuoteResponse(responseTask, storageType));
        }

        /// <summary>
        /// Gets a list of blob metadata objects under a given path for the specified service configuration, storage type and storage ID.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="storageType">The storage type to get blob metadata objects for.</param>
        /// <param name="blobPath">(Optional) The root path to enumerate.  Results will be for blobs contained in this path and all subpaths.</param>
        /// <param name="skipItems">(Optional) The number of items to skip before returning results. (Optional)</param>
        /// <param name="maxItems">(Optional) The maximum number of items to return.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadataResult"/> class containing the list of enumerated blob metadata objects.</returns>
        public Task<TitleStorageBlobMetadataResult> GetBlobMetadataAsync(XboxLiveUser user, TitleStorageType storageType, string blobPath,uint skipItems = 0, uint maxItems = 0)
        {
            if (storageType == TitleStorageType.GlobalStorage && (user == null || !string.IsNullOrEmpty(user.XboxUserId)))
                throw new Exception("Global Storage Type with a non-empty xbox user id");
            
            return this.InternalGetBlobMetadata(user, storageType, blobPath, skipItems, maxItems);

        }

        /// <summary>
        /// Deletes a blob from title storage.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to delete.</param>
        /// <param name="blobQueryProperties">An instance of the <see cref="BlobQueryProperties"/> class with properties of the download query.</param>
        /// <returns>An empty task.</returns>
        public Task DeleteBlobAsync(XboxLiveUser user, TitleStorageBlobMetadata blobMetadata, BlobQueryProperties blobQueryProperties)
        {
            var eTagMatchCondition = blobQueryProperties.ETagMatchCondition == TitleStorageETagMatchCondition.IfMatch ? 
                TitleStorageETagMatchCondition.IfMatch : TitleStorageETagMatchCondition.NotUsed;
            var subPathAndQueryResult = this.GetTitleStorageBlobMetadataDownloadSubpath(user, blobMetadata, string.Empty);
            if (string.IsNullOrEmpty(subPathAndQueryResult))
            {
                return Task.FromResult<object>(null);
            }

            var httpRequest = XboxLiveHttpRequest.Create(HttpMethod.Delete, TitleStorageBaseUri.ToString(), subPathAndQueryResult);
            httpRequest.ContractVersion = TitleStorageApiContract;
            httpRequest.ContentType = ContentTypeHeaderValue;
            httpRequest.XboxLiveAPI = XboxLiveAPIName.DeleteBlob;

            SetEtagHeader(httpRequest, blobMetadata.ETag, eTagMatchCondition);
            return httpRequest.GetResponseWithAuth(user).ContinueWith(responseTask =>
            {
                if (responseTask.Result.ErrorCode != 0)
                {
                    throw new Exception("Invalid HTTP received on delete. Error Message: " + responseTask.Result.ErrorMessage);
                }
            });
        }

        /// <summary>
        /// Downloads blob data from title storage.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to download.</param>
        /// <param name="blobQueryProperties">An instance of the <see cref="BlobQueryProperties"/> class with properties of the download query.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobResult"/> containing the blob content and an updated
        /// <see cref="TitleStorageBlobMetadata"/> object.</returns>
        public Task<TitleStorageBlobResult> DownloadBlobAsync(
            XboxLiveUser user, 
            TitleStorageBlobMetadata blobMetadata, 
            BlobQueryProperties blobQueryProperties)
        {
            var blobBuffer = new List<byte>();

            var resultBlocBlobMetadata = new TitleStorageBlobMetadata(
                blobMetadata.StorageType, blobMetadata.BlobPath, 
                blobMetadata.BlobType, blobMetadata.DisplayName, blobMetadata.ETag);

            var preferredDownloadBlockSize = 
                blobQueryProperties.PreferredBlockSize < MinDownloadBlockSize ? 
                MinDownloadBlockSize : 
                blobQueryProperties.PreferredBlockSize;

            var isBinaryData = (blobMetadata.BlobType == TitleStorageBlobType.Binary);
            var isDownloading = true;
            uint startByte = 0;

            var subPathAndQueryResult = this.GetTitleStorageBlobMetadataDownloadSubpath(
                user, blobMetadata, blobQueryProperties.SelectQuery);

            while (isDownloading)
            {
                var httpRequest = XboxLiveHttpRequest.Create(HttpMethod.Get, TitleStorageBaseUri.ToString(), subPathAndQueryResult);
                httpRequest.ContractVersion = TitleStorageApiContract;
                httpRequest.ContentType = ContentTypeHeaderValue;
                httpRequest.LongHttpCall = true;
                httpRequest.ResponseBodyType = HttpCallResponseBodyType.VectorBody;

                SetEtagHeader(httpRequest, blobMetadata.ETag, blobQueryProperties.ETagMatchCondition);

                if (isBinaryData)
                {
                    httpRequest.SetRangeHeader(startByte, startByte + preferredDownloadBlockSize);
                }

                httpRequest.XboxLiveAPI = XboxLiveAPIName.DownloadBlob;
                httpRequest.GetResponseWithAuth(user).ContinueWith(responseTask =>
                {
                    var response = responseTask.Result;
                    if (response.ErrorCode == 0)
                    {
                        var responseVector = response.ResponseBodyVector;
                        if (responseVector.Length > 0)
                        {
                            blobBuffer.AddRange(responseVector);
                        }

                        startByte += (uint) responseVector.Length;
                        if (!isBinaryData || responseVector.Length < preferredDownloadBlockSize)
                        {
                            isDownloading = false;
                            resultBlocBlobMetadata.SetBlobMetadataProperties((uint)(responseVector.Length), response.ETag);
                        }
                    }
                }).Wait();
            }

            var resultBlobMetadataResult = new TitleStorageBlobResult(resultBlocBlobMetadata, blobBuffer.ToArray());
            return Task.FromResult(resultBlobMetadataResult);
        }

        /// <summary>
        /// Upload blob data to title storage.
        /// </summary>
        /// <param name="user">The Xbox User of the title storage to enumerate. Ignored when enumerating GlobalStorage.</param>
        /// <param name="blobMetadata">The blob metadata for the title storage blob to upload.</param>
        /// <param name="blobBuffer">The Blob content to be uploaded.</param>
        /// <param name="blobQueryProperties">An instance of the <see cref="BlobQueryProperties"/> class with properties of the upload query.</param>
        /// <returns>An instance of the <see cref="TitleStorageBlobMetadata"/> class with updated ETag and Length Properties.</returns>
        public Task<TitleStorageBlobMetadata> UploadBlobAsync(XboxLiveUser user, TitleStorageBlobMetadata blobMetadata, List<byte> blobBuffer, BlobQueryProperties blobQueryProperties)
        {
            if(blobBuffer == null) 
                throw new Exception("Blob buffer is null");

            if(blobBuffer.Count == 0)
                throw new Exception("Blob Buffer is empty");

            var preferredUploadBlockSize = blobQueryProperties.PreferredBlockSize < MinUploadBlockSize ? MinUploadBlockSize : blobQueryProperties.PreferredBlockSize;
            preferredUploadBlockSize = blobQueryProperties.PreferredBlockSize > MaxUploadBlockSize ? MaxUploadBlockSize : preferredUploadBlockSize;

            var resultBlocBlobMetadata = new TitleStorageBlobMetadata(blobMetadata.StorageType, blobMetadata.BlobPath,
               blobMetadata.BlobType, blobMetadata.DisplayName, blobMetadata.ETag);

            var isBinaryData = (blobMetadata.BlobType == TitleStorageBlobType.Binary);

            var dataChunk = new List<byte>();
            uint start = 0;
            var continuationToken = string.Empty;

            while (start < blobBuffer.Count)
            {
                dataChunk.Clear();
                bool isFinalBlock;
                if (isBinaryData)
                {
                    var count = (uint)(blobBuffer.Count) - start;
                    if (count > preferredUploadBlockSize)
                        count = preferredUploadBlockSize;

                    for (var index = 0; index < count; index++)
                    {
                        dataChunk.Add(blobBuffer[(int)(index + start)]);
                    }
                    
                    start += count;
                    isFinalBlock = start == blobBuffer.Count;
                }
                else
                {
                    dataChunk = blobBuffer;
                    start = (uint)(dataChunk.Count);
                    isFinalBlock = true;
                }

                var subpathAndQueryResult = this.GetTitleStorageBlobMetadataUploadSubpath(user, blobMetadata, continuationToken, isFinalBlock);
                var httpRequest = XboxLiveHttpRequest.Create(HttpMethod.Put, TitleStorageBaseUri.ToString(), subpathAndQueryResult);
                httpRequest.ContractVersion = TitleStorageApiContract;
                httpRequest.ContentType = ContentTypeHeaderValue;
                httpRequest.LongHttpCall = true;

                SetEtagHeader(httpRequest, blobMetadata.ETag, blobQueryProperties.ETagMatchCondition);
                var encoding = Encoding.UTF8;
                httpRequest.RequestBody = encoding.GetString(dataChunk.ToArray());
                httpRequest.XboxLiveAPI = XboxLiveAPIName.UploadBlob;
                httpRequest.RetryAllowed = false;

                var localIsFinalBlock = isFinalBlock;
                httpRequest.GetResponseWithAuth(user).ContinueWith(responseTask =>
                {
                    var json = responseTask.Result.ResponseBodyString;
                    continuationToken = string.Empty;
                    if (responseTask.Result.ErrorCode == 0 && !string.IsNullOrEmpty(json))
                    {
                        var pagingInfo = JsonSerialization.FromJson<PagingInfo>(json);
                        continuationToken = pagingInfo.ContinuationToken;
                    }

                    if (responseTask.Result.ErrorCode == 0 && localIsFinalBlock)
                    {
                        resultBlocBlobMetadata.SetBlobMetadataProperties((uint)(blobBuffer.Count), responseTask.Result.ETag);
                    }
                }).Wait();
            }
            return Task.FromResult(resultBlocBlobMetadata);
        }
        
        internal TitleStorageQuota HandleStorageQuoteResponse(
            Task<XboxLiveHttpResponse> responseTask, 
            TitleStorageType storageType)
        {
            var response = responseTask.Result;
            return TitleStorageQuota.Deserialize(
                response.ResponseBodyString, 
                storageType);
        }

        internal Task<TitleStorageBlobMetadataResult> InternalGetBlobMetadata(
            XboxLiveUser user,
            TitleStorageType storageType, 
            string blobPath, 
            uint skipItems,
            uint maxItems,
            string continuationToken = "")
        {
            string subPathAndQueryResult = this.GetTitleStorageBlobMetadataSubpath(
                user,
                storageType, 
                blobPath, 
                skipItems, 
                maxItems,
                continuationToken);

            var httpRequest = XboxLiveHttpRequest.Create(HttpMethod.Get, TitleStorageBaseUri.ToString(), subPathAndQueryResult);
            httpRequest.ContractVersion = TitleStorageApiContract;
            httpRequest.XboxLiveAPI = XboxLiveAPIName.GetBlobMetadata;
            return httpRequest.GetResponseWithAuth(user)
                .ContinueWith(
                responseTask => this.HandleBlobMetadataResult(user, responseTask, storageType, blobPath));
        }
        
        internal TitleStorageBlobMetadataResult HandleBlobMetadataResult(
            XboxLiveUser user,
            Task<XboxLiveHttpResponse> responseTask,
            TitleStorageType storageType,
            string blobPath)
        {
            var response = responseTask.Result;
            return TitleStorageBlobMetadataResult.Deserialize(
                response.ResponseBodyString, 
                storageType, 
                user,
                blobPath);
        }

        internal string GetTitleStorageSubpath(XboxLiveUser user, TitleStorageType titleStorageType)
        {
            var pathBuilder = new StringBuilder();
            switch (titleStorageType)
            {
                case TitleStorageType.TrustedPlatform:
                case TitleStorageType.UniversalPlatform:
                    pathBuilder.AppendFormat(
                        "{0}/users/xuid({1})/scids/{2}",
                        titleStorageType.ToString().ToLowerInvariant(),
                        user.XboxUserId,
                        XboxLive.Instance.AppConfig.PrimaryServiceConfigId);
                    break;
                case TitleStorageType.GlobalStorage:
                    pathBuilder.AppendFormat(
                        "global/scids/{0}",
                        XboxLive.Instance.AppConfig.PrimaryServiceConfigId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("titleStorageType");
            }

            return pathBuilder.ToString();
        }

        internal string GetTitleStorageBlobMetadataSubpath(
            XboxLiveUser user,
            TitleStorageType storageType,
            string blobPath,
            uint skipItems,
            uint maxItems,
            string continuationToken)
        {
            var subPathBuilder = new StringBuilder();
            var path = this.GetTitleStorageSubpath(user, storageType);
            subPathBuilder.Append(path);

            subPathBuilder.Append("/data");
            if (!string.IsNullOrEmpty(blobPath))
            {
                subPathBuilder.Append("/");
                subPathBuilder.Append(Uri.EscapeUriString(blobPath));
            }

            AppendPagingInfo(subPathBuilder, skipItems, maxItems, continuationToken);

            return subPathBuilder.ToString();
        }

        internal string GetTitleStorageBlobMetadataDownloadSubpath(
            XboxLiveUser user,
            TitleStorageBlobMetadata blobMetadata, 
            string selectQuery)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.AppendFormat("{0}/data/{1},{2}", 
                this.GetTitleStorageSubpath(
                    user, 
                    blobMetadata.StorageType), 
                    blobMetadata.BlobPath, 
                    blobMetadata.BlobType.ToString().ToLowerInvariant());

            if (!string.IsNullOrEmpty(selectQuery))
            {
                switch (blobMetadata.BlobType)
                {
                    case TitleStorageBlobType.Config:
                        pathBuilder.AppendFormat("?customerSelector={0}", Uri.EscapeUriString(selectQuery));
                        break;
                    case TitleStorageBlobType.Json:
                        pathBuilder.AppendFormat("?select={0}", Uri.EscapeUriString(selectQuery));
                        break;
                }
            }
            return pathBuilder.ToString();
        }

        internal string GetTitleStorageBlobMetadataUploadSubpath(
            XboxLiveUser user, TitleStorageBlobMetadata blobMetadata, string continuationToken, bool finalBlock)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(this.GetTitleStorageBlobMetadataDownloadSubpath(user, blobMetadata, string.Empty));

            var parameterList = new Dictionary<string, string>();
            if (blobMetadata.ClientTimeStamp != null)
            {
                var clientTimestamp = blobMetadata.ClientTimeStamp.ToString();
                parameterList.Add("clientFileTime", Uri.EscapeUriString(clientTimestamp));
            }

            if (!string.IsNullOrEmpty(blobMetadata.DisplayName))
            {
                parameterList.Add("displayName", Uri.EscapeUriString(blobMetadata.DisplayName));
            }

            if (!string.IsNullOrEmpty(continuationToken))
            {
                parameterList.Add("continuationToken", Uri.EscapeUriString(continuationToken));
            }

            if (blobMetadata.BlobType == TitleStorageBlobType.Binary)
            {
                parameterList.Add("finalBlock", finalBlock.ToString());
            }

            pathBuilder.Append(XboxLiveHttpRequest.GetQueryFromParams(parameterList));
            return pathBuilder.ToString();
        }

        internal static void AppendPagingInfo(
            StringBuilder stringBuilder, uint skipItems, uint maxItems, string continuationToken)
        {
            var parameterList = new Dictionary<string, string>();
            if (maxItems > 0)
            {
                parameterList.Add("maxItems", Convert.ToString(maxItems));
            }

            if (string.IsNullOrEmpty(continuationToken))
            {
                if (skipItems > 0)
                {
                    parameterList.Add("skipItems", Convert.ToString(skipItems));
                }
            }
            else
            {
                parameterList.Add("continuationToken", continuationToken);
            }

            stringBuilder.Append(XboxLiveHttpRequest.GetQueryFromParams(parameterList));
        }


        internal static void SetEtagHeader(
            XboxLiveHttpRequest httpRequest, string eTag, TitleStorageETagMatchCondition eTagMatchCondition)
        {
            if (eTagMatchCondition != TitleStorageETagMatchCondition.NotUsed)
            {
                if (!string.IsNullOrEmpty(eTag))
                {
                    httpRequest.SetCustomHeader(ETagHeaderName, eTag);
                    var headerToUse = eTagMatchCondition == TitleStorageETagMatchCondition.IfMatch ? IfMatchHeaderName : IfNoneHeaderName;
                    httpRequest.SetCustomHeader(headerToUse, eTag);
                }
            }
        }
    }
}
