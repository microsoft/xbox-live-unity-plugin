// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    public partial class XboxLiveAppConfiguration
    {
        public static XboxLiveAppConfiguration Load(string path)
        {
            return new XboxLiveAppConfiguration
            {
                PrimaryServiceConfigId = "00000000-0000-0000-0000-0000694f5acb",
                TitleId = 1766808267,
                Environment = string.Empty,
                Sandbox = "JDTDWX.0",
            };
        }
    }
}