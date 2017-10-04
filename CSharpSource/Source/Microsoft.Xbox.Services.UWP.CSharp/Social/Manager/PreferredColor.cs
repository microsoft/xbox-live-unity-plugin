// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class PreferredColor
    {
        internal PreferredColor(IntPtr preferredColorPtr)
        {
            PreferredColor_c cPreferredColor = Marshal.PtrToStructure<PreferredColor_c>(preferredColorPtr);
            PrimaryColor = cPreferredColor.PrimaryColor;
            SecondaryColor = cPreferredColor.SecondaryColor;
            TertiaryColor = cPreferredColor.TertiaryColor;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct PreferredColor_c
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string PrimaryColor;

            [MarshalAs(UnmanagedType.LPStr)]
            public string SecondaryColor;

            [MarshalAs(UnmanagedType.LPStr)]
            public string TertiaryColor;
        }
    }
}
