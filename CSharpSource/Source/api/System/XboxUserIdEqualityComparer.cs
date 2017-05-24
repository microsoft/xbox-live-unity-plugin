// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System.Collections.Generic;

    using Microsoft.Xbox.Services.Social.Manager;

    /// <summary>
    /// An equality comparer that only uses the XUID property.
    /// </summary>
    /// <remarks>
    /// This is used in a variety of places in order to perform dictionary lookups efficiently.
    /// </remarks>
    public sealed class XboxUserIdEqualityComparer : IEqualityComparer<XboxLiveUser>
    {
        public bool Equals(XboxLiveUser x, XboxLiveUser y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.XboxUserId, y.XboxUserId);
        }

        public int GetHashCode(XboxLiveUser obj)
        {
            return obj.XboxUserId.GetHashCode();
        }
    }

    /// <summary>
    /// An equality comparer that only uses the XUID property.
    /// </summary>
    /// <remarks>
    /// This is used in a variety of places in order to perform dictionary lookups efficiently.
    /// </remarks>
    public sealed class XboxSocialUserIdEqualityComparer : IEqualityComparer<XboxSocialUser>
    {
        private static XboxSocialUserIdEqualityComparer instance;
        private static readonly object instanceLock = new object();

        public static XboxSocialUserIdEqualityComparer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new XboxSocialUserIdEqualityComparer();
                        }
                    }
                }
                return instance;
            }
        }

        public bool Equals(XboxSocialUser x, XboxSocialUser y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.XboxUserId, y.XboxUserId);
        }

        public int GetHashCode(XboxSocialUser obj)
        {
            return obj.XboxUserId.GetHashCode();
        }
    }
}