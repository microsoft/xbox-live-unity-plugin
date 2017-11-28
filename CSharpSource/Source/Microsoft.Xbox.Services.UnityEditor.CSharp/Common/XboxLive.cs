// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{

    public partial class XboxLive
    {
        public static bool UseMockServices
        {
            get { return true; }
        }

        public static bool UseMockHttp
        {
            get { return false; }
        }
    }
}