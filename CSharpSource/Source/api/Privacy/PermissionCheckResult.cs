// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 


namespace Microsoft.Xbox.Services.Privacy
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    public class PermissionCheckResult
    {
        public IList<PermissionDenyReason> Reasons { get; private set; }

        public string PermissionRequested { get; private set; }

        public bool IsAllowed { get; private set; }

        internal PermissionCheckResult(IntPtr permissionCheckResultStructPtr)
            : this(MarshalingHelpers.PtrToStructure<XSAPI_PRIVACY_PERMISSION_CHECK_RESULT>(permissionCheckResultStructPtr))
        {
        }

        internal PermissionCheckResult(XSAPI_PRIVACY_PERMISSION_CHECK_RESULT permissionCheckResultStruct)
        {
            IsAllowed = permissionCheckResultStruct.isAllowed;
            PermissionRequested = MarshalingHelpers.Utf8ToString(permissionCheckResultStruct.permissionRequested);

            Reasons = new List<PermissionDenyReason>((int)permissionCheckResultStruct.denyReasonsCount);

            int size = MarshalingHelpers.SizeOf<XSAPI_PRIVACY_PERMISSION_DENY_REASON>();
            IntPtr denyReasonPtr = permissionCheckResultStruct.denyReasons;

            for (ulong i = 0; i < permissionCheckResultStruct.denyReasonsCount; ++i)
            {
                Reasons.Add(new PermissionDenyReason(denyReasonPtr));
                denyReasonPtr = denyReasonPtr.Increment(size);
            }
            
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XSAPI_PRIVACY_PERMISSION_CHECK_RESULT
    {
        public bool isAllowed;
        public IntPtr permissionRequested;
        public IntPtr denyReasons;
        public UInt64 denyReasonsCount;
    }
}
