// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.ConnectedStorage
{
    public enum GameSaveStatus
    {
        Abort,
        InvalidContainerName,
        NoAccess,
        OutOfLocalStorage,
        UserCanceled,
        UpdateTooBig,
        QuotaExceeded,
        ProvidedBufferTooSmall,
        BlobNotFound,
        NoXboxLiveInfo,
        ContainerNotInSync,
        ContainerSyncFailed,
        UserHasNoXboxLiveInfo,
        ObjectExpired,
        Ok
    }
}

