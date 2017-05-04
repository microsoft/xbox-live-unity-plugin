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

