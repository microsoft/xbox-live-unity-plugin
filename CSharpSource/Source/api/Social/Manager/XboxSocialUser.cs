// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;

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

        public bool IsFollowingUser { get; set; }

        public bool IsFavorite { get; set; }

        public SocialManagerPresenceRecord PresenceRecord { get; set; }

        public TitleHistory TitleHistory { get; set; }
        
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
            IsFollowingUser = Convert.ToBoolean(cXboxSocialUser.IsFollowingUser);
            IsFavorite = Convert.ToBoolean(cXboxSocialUser.IsFavorite);

            PresenceRecord = new SocialManagerPresenceRecord(cXboxSocialUser.PresenceRecord);
            
            TitleHistory = new TitleHistory(cXboxSocialUser.TitleHistory);
        }
    }
}