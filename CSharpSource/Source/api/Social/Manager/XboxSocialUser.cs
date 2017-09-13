// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Presence;
    
    using static SocialManager;

    public class XboxSocialUser
    {
        public string XboxUserId { get; set; }

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

        internal XboxSocialUser()
        {

        }

        // todo refresh xbox social users on do_work
        internal XboxSocialUser(IntPtr xboxSocialUserPtr)
        {
            XboxSocialUser_c cXboxSocialUser = Marshal.PtrToStructure<XboxSocialUser_c>(xboxSocialUserPtr);

            XboxUserId = cXboxSocialUser.XboxUserId;
            DisplayName = cXboxSocialUser.DisplayName;
            RealName = cXboxSocialUser.RealName;
            DisplayPicRaw = cXboxSocialUser.DisplayPicUrlRaw;
            UseAvatar = Convert.ToBoolean(cXboxSocialUser.UseAvatar);
            Gamertag = cXboxSocialUser.Gamertag;
            Gamerscore = cXboxSocialUser.Gamerscore;
            PreferredColor = new PreferredColor(cXboxSocialUser.PreferredColor);
            IsFollowedByCaller = Convert.ToBoolean(cXboxSocialUser.IsFollowedByCaller);
            IsFollowingCaller = Convert.ToBoolean(cXboxSocialUser.IsFollowingUser);
            IsFavorite = Convert.ToBoolean(cXboxSocialUser.IsFavorite);

            SocialManagerPresenceRecord_c cPresenceRecord = Marshal.PtrToStructure<SocialManagerPresenceRecord_c>(cXboxSocialUser.PresenceRecord);
            PresenceState = cPresenceRecord.UserState;

            List<SocialManagerPresenceTitleRecord> titleRecords = new List<SocialManagerPresenceTitleRecord>();
            IntPtr[] cTitleRecords = new IntPtr[cPresenceRecord.NumOfPresenceTitleRecords];
            Marshal.Copy(cPresenceRecord.PresenceTitleRecords, cTitleRecords, 0, cPresenceRecord.NumOfPresenceTitleRecords);
            foreach (IntPtr cTitleRecord in cTitleRecords)
            {
                titleRecords.Add(new SocialManagerPresenceTitleRecord(cTitleRecord));
            }
            PresenceDetails = titleRecords;
            
            // todo: TitleHistory = new TitleHistory(cXboxSocialUser.TitleHistory);
        }
    }
}