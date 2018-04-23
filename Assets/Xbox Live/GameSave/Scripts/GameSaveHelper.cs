// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if ENABLE_WINMD_SUPPORT
using Windows.Gaming.XboxLive.Storage;
using Windows.Storage.Streams;
#endif

namespace Microsoft.Xbox.Services.ConnectedStorage
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Text;

    using Microsoft.Xbox.Services.Client;

    using UnityEngine;

#if ENABLE_WINMD_SUPPORT
    using global::System.Runtime.InteropServices.WindowsRuntime;
#endif

    public class GameSaveHelper
    {

#if ENABLE_WINMD_SUPPORT
        private GameSaveProvider gameSaveProvider;
#endif

        private bool isInitializedInterntal;

        public IEnumerator Initialize(XboxLiveUser xboxLiveUser, Action<GameSaveStatus> resultCallBack)
        {
            yield return null;

            if (resultCallBack != null)
            {
#if ENABLE_WINMD_SUPPORT
                var configId = XboxLive.Instance.AppConfig.ServiceConfigurationId;
                var initTask = GameSaveProvider.GetForUserAsync(xboxLiveUser.WindowsSystemUser, configId).AsTask();
                if (initTask.Result.Status == GameSaveErrorStatus.Ok)
                {
                    this.gameSaveProvider = initTask.Result.Value;
                }

                var gameSaveStatus =
                    (GameSaveStatus)Enum.Parse(typeof(GameSaveStatus), initTask.Result.Status.ToString());
                resultCallBack(gameSaveStatus);
#else
                resultCallBack(GameSaveStatus.Ok);
#endif
                this.isInitializedInterntal = true;
            }
        }

        public IEnumerator SubmitUpdates(
            string containerName,
            Dictionary<string, byte[]> dataToSaveForBlobs,
            string[] blobsToDelete,
            Action<GameSaveStatus> resultCallBack,
            string displayName = "")
        {
            yield return null;
            if (resultCallBack != null)
            {
#if ENABLE_WINMD_SUPPORT
                if (this.gameSaveProvider != null)
                {
                    var container = this.gameSaveProvider.CreateContainer(containerName);
                    var displayNameToUse = (!string.IsNullOrEmpty(displayName)) ? displayName : containerName;
                    var saveBuffers = this.GenerateIBuffers(dataToSaveForBlobs);
                    container.SubmitUpdatesAsync(saveBuffers, blobsToDelete, displayNameToUse)
                        .AsTask()
                        .ContinueWith(
                            saveTask =>
                                {
                                    var gameSaveStatus =
                                        (GameSaveStatus)
                                        Enum.Parse(typeof(GameSaveStatus), saveTask.Result.Status.ToString());
                                    resultCallBack(gameSaveStatus);
                                });
                }
                else
                {
                    var errorMessage = "An Exception Occured: Game Save Provider hasn't been initialized yet. Initialize needs to be called first.";
                    ExceptionManager.Instance.ThrowException(
                           ExceptionSource.GameSaveManager,
                           ExceptionType.GameSaveProviderNotInitialized,
                           new Exception(errorMessage));
                }
#else
                resultCallBack(GameSaveStatus.Ok);
#endif
            }
        }

        public IEnumerator GetAsBytes(
            string containerName,
            string[] blobsToRead,
            Action<Dictionary<string, byte[]>> resultCallBack)
        {
            yield return null;
            if (resultCallBack != null)
            {
                var returnDictionary = new Dictionary<string, byte[]>();
#if ENABLE_WINMD_SUPPORT
                if (this.gameSaveProvider != null)
                {
                    var loadBuffers = this.LoadDataHelper(containerName, blobsToRead);
                    foreach (var loadedBuffer in loadBuffers)
                    {
                        returnDictionary.Add(loadedBuffer.Key, loadedBuffer.Value.ToArray());
                    }
                }
                else
                {
                    var errorMessage = "An Exception Occured: Game Save Provider hasn't been initialized yet. Initialize needs to be called first.";
                    ExceptionManager.Instance.ThrowException(
                           ExceptionSource.GameSaveManager,
                           ExceptionType.GameSaveProviderNotInitialized,
                           new Exception(errorMessage));
                }
#else
                var blobContent = "Fake Content";
                var blobContentBytes = Encoding.UTF8.GetBytes(blobContent);
                foreach (var blob in blobsToRead)
                {
                    returnDictionary.Add(blob, blobContentBytes);
                }
#endif
                resultCallBack(returnDictionary);
            }
        }

        public IEnumerator GetAsStrings(
            string containerName,
            string[] blobsToRead,
            Action<Dictionary<string, string>> resultCallBack)
        {
            yield return null;
            if (resultCallBack != null)
            {
                var returnDictionary = new Dictionary<string, string>();
#if ENABLE_WINMD_SUPPORT
                if (this.gameSaveProvider != null)
                {
                    var loadBuffers = this.LoadDataHelper(containerName, blobsToRead);
                    foreach (var loadedBuffer in loadBuffers)
                    {
                        var loadedBufferValue = loadedBuffer.Value;
                        var reader = DataReader.FromBuffer(loadedBufferValue);
                        var loadedData = reader.ReadString(loadedBufferValue.Length);
                        returnDictionary.Add(loadedBuffer.Key, loadedData);
                    }
                }
                else
                {
                    var errorMessage = "An Exception Occured: Game Save Provider hasn't been initialized yet. Initialize needs to be called first.";
                    ExceptionManager.Instance.ThrowException(
                           ExceptionSource.GameSaveManager,
                           ExceptionType.GameSaveProviderNotInitialized,
                           new Exception(errorMessage));
                }
#else
                var blobContent = "Fake Content";
                foreach (var blob in blobsToRead)
                {
                    returnDictionary.Add(blob, blobContent);
                }
#endif
                resultCallBack(returnDictionary);
            }
        }

        public IEnumerator DeleteContainer(string containerName, Action<GameSaveStatus> resultCallBack)
        {
            yield return null;
            if (resultCallBack != null)
            {
#if ENABLE_WINMD_SUPPORT
                if (this.gameSaveProvider != null)
                {
                    this.gameSaveProvider.DeleteContainerAsync(containerName).AsTask().ContinueWith(
                        deleteTask =>
                            {
                                var deleteStatus =
                                    (GameSaveStatus)
                                    Enum.Parse(typeof(GameSaveStatus), deleteTask.Result.Status.ToString());
                                resultCallBack(deleteStatus);
                            });
                }
                else
                {
                    var errorMessage = "An Exception Occured: Game Save Provider hasn't been initialized yet. Initialize needs to be called first.";
                    ExceptionManager.Instance.ThrowException(
                           ExceptionSource.GameSaveManager,
                           ExceptionType.GameSaveProviderNotInitialized,
                           new Exception(errorMessage));
                }
#else
                resultCallBack(GameSaveStatus.Ok);
#endif
            }
        }

        public IEnumerator GetContainerInfo(string prefix, Action<StorageContainerQueryResult> resultCallBack)
        {
            yield return null;

            if (resultCallBack != null)
            {
#if ENABLE_WINMD_SUPPORT
                if (this.gameSaveProvider != null)
                {
                    var query = this.gameSaveProvider.CreateContainerInfoQuery(prefix);
                    var resultList = new List<StorageContainerInfo>();
                    query.GetContainerInfoAsync().AsTask().ContinueWith(
                        queryTask =>
                            {
                                var gameSaveQueryResults = queryTask.Result;
                                foreach (var resultgameSaveContainerInfo in gameSaveQueryResults.Value)
                                {
                                    var containerInfo = new StorageContainerInfo
                                    {
                                        Name = resultgameSaveContainerInfo.Name,
                                        DisplayName = resultgameSaveContainerInfo.DisplayName,
                                        LastModifiedTime = resultgameSaveContainerInfo.LastModifiedTime,
                                        NeedsSync = resultgameSaveContainerInfo.NeedsSync,
                                        TotalSize = resultgameSaveContainerInfo.TotalSize
                                    };
                                    resultList.Add(containerInfo);
                                }

                                var status = (GameSaveStatus)Enum.Parse(typeof(GameSaveStatus), gameSaveQueryResults.Status.ToString());
                                var queryResult = new StorageContainerQueryResult
                                {
                                    Value = resultList,
                                    Status = status
                                };

                                resultCallBack(queryResult);
                            });
                }
                else
                {
                    var errorMessage = "An Exception Occured: Game Save Provider hasn't been initialized yet. Initialize needs to be called first.";
                    ExceptionManager.Instance.ThrowException(
                           ExceptionSource.GameSaveManager,
                           ExceptionType.GameSaveProviderNotInitialized,
                           new Exception(errorMessage));
                }
#else
                var resultList = new List<StorageContainerInfo>();
                var firstContainer = new StorageContainerInfo
                {
                    Name = "Container01",
                    DisplayName = "Display Name 01",
                    LastModifiedTime = DateTime.Now,
                    NeedsSync = false,
                    TotalSize = 1024
                };
                resultList.Add(firstContainer);
                var secondContainer = new StorageContainerInfo
                {
                    Name = "Container02",
                    DisplayName = "Display Name 02",
                    LastModifiedTime = DateTime.Now,
                    NeedsSync = false,
                    TotalSize = 2048
                };
                resultList.Add(secondContainer);
                var queryResult = new StorageContainerQueryResult
                {
                    Status = GameSaveStatus.Ok,
                    Value = resultList
                };
                resultCallBack(queryResult);
#endif
            }
        }

        public IEnumerator GetRemainingBytesInQuota(string containerName, Action<long> resultCallBack)
        {
            yield return null;
            if (resultCallBack != null)
            {
#if ENABLE_WINMD_SUPPORT
                if (this.gameSaveProvider != null)
                {
                    var quotaTask = this.gameSaveProvider.GetRemainingBytesInQuotaAsync().AsTask();
                    resultCallBack(quotaTask.Result);
                }
                else
                {
                    var errorMessage = "An Exception Occured: Game Save Provider hasn't been initialized yet. Initialize needs to be called first.";
                    ExceptionManager.Instance.ThrowException(
                           ExceptionSource.GameSaveManager,
                           ExceptionType.GameSaveProviderNotInitialized,
                           new Exception(errorMessage));
                }
#else
                resultCallBack(1024);
#endif
            }
        }

        public bool IsInitialized()
        {
            return this.isInitializedInterntal;
        }

#if ENABLE_WINMD_SUPPORT
        private Dictionary<string, IBuffer> LoadDataHelper(string containerName, string[] blobsToRead)
        {
            var container = this.gameSaveProvider.CreateContainer(containerName);
            var resultBuffers = new Dictionary<string, IBuffer>();
            GameSaveBlobGetResult loadResult = container.GetAsync(blobsToRead).AsTask().Result;
            if (loadResult.Status == GameSaveErrorStatus.Ok)
            {
                foreach (var blobName in blobsToRead)
                {
                    IBuffer loadedBuffer;
                    loadResult.Value.TryGetValue(blobName, out loadedBuffer);
                    if (loadedBuffer == null)
                    {
                        var errorMessage = string.Format("An Exception Occured: Didn't find expected blob \"{0}\" in the loaded data.", blobName);
                        ExceptionManager.Instance.ThrowException(
                               ExceptionSource.GameSaveManager,
                               ExceptionType.BlobNotFound,
                               new Exception(errorMessage));
                        return null;
                    }

                    resultBuffers.Add(blobName, loadedBuffer);
                }
                return resultBuffers;
            }
            else
            {
                var errorMessage = string.Format("An Exception Occured: Loading Data failed. Error Status: {0}", loadResult.Status);
                    ExceptionManager.Instance.ThrowException(
                               ExceptionSource.GameSaveManager,
                               ExceptionType.LoadingDataFailed,
                               new Exception(errorMessage));


                return null;
            }
        }

        private Dictionary<string, IBuffer> GenerateIBuffers(Dictionary<string, byte[]> dataToSaveForBlobs)
        {
            var saveBuffers = new Dictionary<string, IBuffer>();
            foreach (var blobDataPair in dataToSaveForBlobs)
            {
                var dataWriter = new DataWriter();
                dataWriter.WriteBytes(blobDataPair.Value);
                var dataBuffer = dataWriter.DetachBuffer();
                saveBuffers.Add(blobDataPair.Key, dataBuffer);
            }

            return saveBuffers;
        }
#endif
    }
}