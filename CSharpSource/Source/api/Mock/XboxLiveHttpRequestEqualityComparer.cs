// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    public sealed class XboxLiveHttpRequestEqualityComparer : IEqualityComparer<XboxLiveHttpRequest>
    {
        public static List<string> IgnoredHeaders = new List<string>
        {
            "Authorization"
        };

        public bool Equals(XboxLiveHttpRequest x, XboxLiveHttpRequest y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Method, y.Method)
                && string.Equals(x.Url, y.Url);
        }

        private static bool CheckHeadersAreEqual(IDictionary<string, string> x, IDictionary<string, string> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }

            return x.Keys.Where(key => !IgnoredHeaders.Contains(key)).All(key => x[key] == y[key]);
        }

        public int GetHashCode(XboxLiveHttpRequest obj)
        {
            unchecked
            {
                var hashCode = obj.Method.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Url.GetHashCode();
                return hashCode;
            }
        }
    }
}