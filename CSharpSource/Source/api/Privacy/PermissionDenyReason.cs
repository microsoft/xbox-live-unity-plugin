// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.Privacy
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public class PermissionDenyReason
    {
        public string Reason { get; private set; }

        public string RestrictedSetting { get; private set; }

        internal PermissionDenyReason(IntPtr structPtr)
        {
            var permissionDenyReasonStruct = MarshalingHelpers.PtrToStructure<XSAPI_PRIVACY_PERMISSION_DENY_REASON>(structPtr);
            Reason = MarshalingHelpers.Utf8ToString(permissionDenyReasonStruct.reason);
            RestrictedSetting = MarshalingHelpers.Utf8ToString(permissionDenyReasonStruct.restrictedSetting);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XSAPI_PRIVACY_PERMISSION_DENY_REASON
    {
        public IntPtr reason;
        public IntPtr restrictedSetting;
    }
}
