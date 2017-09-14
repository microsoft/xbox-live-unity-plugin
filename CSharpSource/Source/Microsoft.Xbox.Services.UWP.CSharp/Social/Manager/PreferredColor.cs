// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using static Microsoft.Xbox.Services.Social.Manager.SocialManager;

    public partial class PreferredColor
    {
        internal PreferredColor(IntPtr preferredColorPtr)
        {
            PreferredColor_c cPreferredColor = Marshal.PtrToStructure<PreferredColor_c>(preferredColorPtr);
            PrimaryColor = cPreferredColor.PrimaryColor;
            SecondaryColor = cPreferredColor.SecondaryColor;
            TertiaryColor = cPreferredColor.TertiaryColor;
        }
    }
}
