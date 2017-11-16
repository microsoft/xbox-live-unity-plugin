// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class PreferredColor
    {
        internal PreferredColor(IntPtr preferredColorPtr)
        {
            PreferredColor_c cPreferredColor = Marshal.PtrToStructure<PreferredColor_c>(preferredColorPtr);
            PrimaryColor = MarshalingHelpers.Utf8ToString(cPreferredColor.PrimaryColor);
            SecondaryColor = MarshalingHelpers.Utf8ToString(cPreferredColor.SecondaryColor);
            TertiaryColor = MarshalingHelpers.Utf8ToString(cPreferredColor.TertiaryColor);
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct PreferredColor_c
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PrimaryColor;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr SecondaryColor;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr TertiaryColor;
        }
    }
}
