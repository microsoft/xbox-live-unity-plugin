// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using Newtonsoft.Json;
    using global::System.IO;

    public partial class XboxLiveAppConfiguration
    {
        public string PublisherId { get; set; }

        public string PublisherDisplayName { get; set; }

        public string PackageIdentityName { get; set; }

        public string DisplayName { get; set; }

        public string AppId { get; set; }

        public string ProductFamilyName { get; set; }

        public bool XboxLiveCreatorsTitle { get; set; }

        private static XboxLiveAppConfiguration Load()
        {
            return XboxLiveAppConfiguration.Load(FileName);
        }

        public static XboxLiveAppConfiguration Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format("Unable to find Xbox Live app configuration file '{0}'.", path));
            }

            string content = File.ReadAllText(path);
            if (string.IsNullOrEmpty(content))
            {
                throw new XboxException(string.Format("Xbox Live app configeration file '{0}' was empty.", path));
            }

            return JsonConvert.DeserializeObject<XboxLiveAppConfiguration>(content);
        }
    }
}