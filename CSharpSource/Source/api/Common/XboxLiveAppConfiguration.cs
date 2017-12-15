// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System;
#if UNITY_EDITOR
    using global::Newtonsoft.Json;
#endif

    public partial class XboxLiveAppConfiguration
    {
        public const string FileName = "XboxServices.config";

        private static readonly object instanceLock = new object();
        private static XboxLiveAppConfiguration instance;

#if UNITY_EDITOR
        [JsonProperty("PrimaryServiceConfigId")]
#endif
        public string ServiceConfigurationId { get; set; }

        public uint TitleId { get; set; }

        public string Sandbox { get; set; }

        public string Environment { get; set; }

        public static XboxLiveAppConfiguration SingletonInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        try
                        {
                            if (instance == null)
                            {
                                instance = XboxLiveAppConfiguration.Load();
                            }
                        }
                        catch(Exception e)
                        {
                            // If we're unable to load the file for some reason, we can just use an empty file
                            // if mock data is enable.
                            if (XboxLive.UseMockServices)
                            {
                                return new XboxLiveAppConfiguration();
                            }
                            throw new XboxException(string.Format("Unable to find or load Xbox Live configuration.  Make sure a properly configured Xboxservices.config exists."), e);
                        }
                    }
                }
                return instance;
            }
        }
    }
}