// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Runtime.InteropServices;

    internal enum XsapiResult
    {
        XSAPI_OK = 0,
        XSAPI_E_FAIL = -1,
        XSAPI_E_POINTER = -2,
        XSAPI_E_INVALIDARG = -3,
        XSAPI_E_OUTOFMEMORY = -4,
        XSAPI_E_BUFFERTOOSMALL = -5,
        XSAPI_E_NOTINITIALIZED = -6,
        XSAPI_E_FEATURENOTPRESENT = -7
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XboxLiveResult
    {
        [MarshalAsAttribute(UnmanagedType.I4)]
        public int errorCode;

        [MarshalAsAttribute(UnmanagedType.LPStr)]
        public String errorMessage;
    }
}
