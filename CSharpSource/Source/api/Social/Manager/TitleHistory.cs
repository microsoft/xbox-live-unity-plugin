// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;

    public class TitleHistory : IEquatable<TitleHistory>
    {
        public DateTimeOffset LastTimeUserPlayed { get; set; }

        public bool HasUserPlayed { get; set; }

        public bool Equals(TitleHistory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.LastTimeUserPlayed.Equals(other.LastTimeUserPlayed)
                   && this.HasUserPlayed == other.HasUserPlayed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((TitleHistory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.LastTimeUserPlayed.GetHashCode() * 397) ^ this.HasUserPlayed.GetHashCode();
            }
        }
    }
}