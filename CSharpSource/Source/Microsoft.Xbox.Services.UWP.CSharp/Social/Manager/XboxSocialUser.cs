// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class XboxSocialUser
    {
        // todo refresh xbox social users on do_work
        internal XboxSocialUser(IntPtr xboxSocialUserPtr)
        {
            SocialManager.XboxSocialUser_c cXboxSocialUser = Marshal.PtrToStructure<SocialManager.XboxSocialUser_c>(xboxSocialUserPtr);

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
