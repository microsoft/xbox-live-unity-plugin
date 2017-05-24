// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using Microsoft.Xbox.Services.Presence;

    using Newtonsoft.Json;

    public class XboxSocialUser
    {
        [JsonProperty("xuid")]
        public ulong XboxUserId { get; set; }

        public string DisplayName { get; set; }

        public string RealName { get; set; }

        public string DisplayPicRaw { get; set; }

        public bool UseAvatar { get; set; }

        public string Gamertag { get; set; }

        public string Gamerscore { get; set; }

        public PreferredColor PreferredColor { get; set; }

        public bool IsFollowedByCaller { get; set; }

        public bool IsFollowingCaller { get; set; }

        public bool IsFavorite { get; set; }

        public UserPresenceState PresenceState { get; set; }

        public IList<SocialManagerPresenceTitleRecord> PresenceDetails { get; set; }

        public TitleHistory TitleHistory { get; set; }

        public bool IsUserPlayingTitle(uint titleId)
        {
            return this.PresenceDetails.Any(t => t.TitleId == titleId && t.IsTitleActive);
        }

        internal ChangeListType GetChanges(XboxSocialUser other)
        {
            ChangeListType changeType = ChangeListType.NoChange;

            if (!string.Equals(this.Gamertag, other.Gamertag, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(this.Gamerscore, other.Gamerscore, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(this.RealName, other.RealName, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(this.DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(this.DisplayPicRaw, other.DisplayPicRaw, StringComparison.OrdinalIgnoreCase)
                || this.UseAvatar != other.UseAvatar
                || !Equals(this.PreferredColor, other.PreferredColor)
                || !Equals(this.TitleHistory, other.TitleHistory))
            {
                changeType |= ChangeListType.ProfileChange;
            }

            if (this.IsFollowedByCaller != other.IsFollowedByCaller ||
                this.IsFollowingCaller != other.IsFollowingCaller ||
                this.IsFavorite != other.IsFavorite)
            {
                changeType |= ChangeListType.SocialRelationshipChange;
            }

            if ((this.PresenceState != other.PresenceState) ||
                (this.PresenceDetails != null && other.PresenceDetails != null &&
                 this.PresenceDetails.Count > 0 && other.PresenceDetails.Count > 0 &&
                 !this.PresenceDetails.All(record => other.PresenceDetails.Contains(record))))
            {
                changeType |= ChangeListType.PresenceChange;
            }

            return changeType;
        }
    }
}