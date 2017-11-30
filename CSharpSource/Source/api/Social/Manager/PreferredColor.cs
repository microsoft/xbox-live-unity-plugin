// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.Services.Social.Manager
{
    public class PreferredColor
    {
        public string TertiaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string PrimaryColor { get; set; }

        public PreferredColor()
        {
        }

        internal PreferredColor(IntPtr preferredColorPtr)
        {
            PREFERRED_COLOR cPreferredColor = MarshalingHelpers.PtrToStructure<PREFERRED_COLOR>(preferredColorPtr);
            PrimaryColor = MarshalingHelpers.Utf8ToString(cPreferredColor.PrimaryColor);
            SecondaryColor = MarshalingHelpers.Utf8ToString(cPreferredColor.SecondaryColor);
            TertiaryColor = MarshalingHelpers.Utf8ToString(cPreferredColor.TertiaryColor);
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct PREFERRED_COLOR
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PrimaryColor;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr SecondaryColor;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr TertiaryColor;
        }

        protected bool Equals(PreferredColor other)
        {
            return string.Equals(this.TertiaryColor, other.TertiaryColor) 
                && string.Equals(this.SecondaryColor, other.SecondaryColor) 
                && string.Equals(this.PrimaryColor, other.PrimaryColor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return this.Equals((PreferredColor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.PrimaryColor != null ? this.PrimaryColor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.SecondaryColor != null ? this.SecondaryColor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.TertiaryColor != null ? this.TertiaryColor.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}