// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::System.IO;

    public partial class XboxLiveAppConfiguration
    {
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

            return JsonSerialization.FromJson<XboxLiveAppConfiguration>(content);
        }
    }
}