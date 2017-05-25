namespace Microsoft.Xbox.Services.Shared.TitleStorage
{
    using Microsoft.Xbox.Services.TitleStorage;

    /// <summary>
    /// Sets or Gets the Properties needed for Downloading Blobs.
    /// </summary>
    public class BlobQueryProperties
    {
        /// <summary>
        /// Gets or Sets the ETag Match Condition. Possible values are: NotUsed, IfMatch, IfNotMatch.
        /// </summary>
        public TitleStorageETagMatchCondition ETagMatchCondition { get; set; }

        /// <summary>
        /// The preferred download block size in bytes for binary blobs. 
        /// Minimum Size is 1024 bytes. Maximum for downloads is 4 Megabytes.
        /// Default Value Size is 262144 bytes.
        /// </summary>
        public uint PreferredBlockSize { get; set; }

        /// <summary>
        /// [Optional] ConfigStorage filter string or JSONStorage json property name string to filter. 
        /// </summary>
        public string SelectQuery { get; set; }
    }
}
