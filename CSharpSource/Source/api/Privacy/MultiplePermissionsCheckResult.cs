// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.Privacy
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    public class MultiplePermissionsCheckResult
    {
        public IList<PermissionCheckResult> Items { get; private set; }

        public string XboxUserId { get; private set; }

        internal MultiplePermissionsCheckResult(IntPtr multiplePermissionsStructPtr)
        {
            var multiplePermissionsStruct = MarshalingHelpers.PtrToStructure<XSAPI_PRIVACY_MULTIPLE_PERMISSIONS_CHECK_RESULT>(multiplePermissionsStructPtr);

            XboxUserId = MarshalingHelpers.Utf8ToString(multiplePermissionsStruct.xboxUserId);

            Items = new List<PermissionCheckResult>((int)multiplePermissionsStruct.itemsCount);

            int size = MarshalingHelpers.SizeOf<XSAPI_PRIVACY_PERMISSION_CHECK_RESULT>();
            IntPtr permissionStructPtr = multiplePermissionsStruct.items;

            for (ulong i = 0; i < multiplePermissionsStruct.itemsCount; ++i)
            {
                var permissionCheckResultStruct = MarshalingHelpers.PtrToStructure<XSAPI_PRIVACY_PERMISSION_CHECK_RESULT>(permissionStructPtr);
                Items.Add(new PermissionCheckResult(permissionCheckResultStruct));
                permissionStructPtr = permissionStructPtr.Increment(size);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XSAPI_PRIVACY_MULTIPLE_PERMISSIONS_CHECK_RESULT
    {
        public IntPtr xboxUserId;
        public IntPtr items;
        public UInt64 itemsCount;
    }
}
