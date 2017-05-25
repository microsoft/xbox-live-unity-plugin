// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    public class PreferredColor
    {
        public string TertiaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string PrimaryColor { get; set; }

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