// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::Microsoft.Xbox.Services.System;
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class XboxLiveAppConfiguration
    {
        private static XboxLiveAppConfiguration Load()
        {
            IntPtr appConfigPtr;
            var result = GetXboxLiveAppConfigSingleton(out appConfigPtr);
            if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(result);
            }

            var appConfigStruct = Marshal.PtrToStructure<XSAPI_XBOX_LIVE_APP_CONFIG>(appConfigPtr);

            return new XboxLiveAppConfiguration
            {
                TitleId = appConfigStruct.titleId,
                Environment = MarshalingHelpers.Utf8ToString(appConfigStruct.environment),
                Sandbox = MarshalingHelpers.Utf8ToString(appConfigStruct.sandbox),
                ServiceConfigurationId = MarshalingHelpers.Utf8ToString(appConfigStruct.scid)
            };
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XSAPI_XBOX_LIVE_APP_CONFIG
        {
            public UInt32 titleId;
            public IntPtr scid;
            public IntPtr environment;
            public IntPtr sandbox;
        };

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT GetXboxLiveAppConfigSingleton(
            out IntPtr ppConfig);
    }
}