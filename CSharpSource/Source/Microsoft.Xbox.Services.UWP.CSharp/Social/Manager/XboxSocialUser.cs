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

        [StructLayout(LayoutKind.Sequential)]
        internal struct XboxSocialUser_c
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string XboxUserId;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsFavorite;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsFollowingUser;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsFollowedByCaller;

            [MarshalAs(UnmanagedType.LPStr)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string RealName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string DisplayPicUrlRaw;

            [MarshalAs(UnmanagedType.U1)]
            public byte UseAvatar;

            [MarshalAs(UnmanagedType.LPStr)]
            public string Gamerscore;

            [MarshalAs(UnmanagedType.LPStr)]
            public string Gamertag;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PresenceRecord;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr TitleHistory;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PreferredColor;
        }
    }
}
