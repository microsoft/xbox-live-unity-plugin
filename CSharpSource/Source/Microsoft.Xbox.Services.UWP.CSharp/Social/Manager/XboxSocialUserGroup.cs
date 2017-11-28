// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;

    public partial class XboxSocialUserGroup
    {
        // Pointer to the c object
        private IntPtr m_socialUserGroupPtr;

        internal XboxSocialUserGroup(IntPtr socialUserGroupPtr)
        {
            m_socialUserGroupPtr = socialUserGroupPtr;
            Refresh();
        }

        internal IntPtr GetPtr()
        {
            return m_socialUserGroupPtr;
        }

        // todo For perf consider if we need an init method and a refresh method seperate
        internal void Refresh()
        {
            var cSocialUserGroup = Marshal.PtrToStructure<XBOX_SOCIAL_USER_GROUP>(m_socialUserGroupPtr);

            // Properties
            SocialUserGroupType = cSocialUserGroup.SocialUserGroupType;
            PresenceFilterOfGroup = cSocialUserGroup.PresenceFilterOfGroup;
            RelationshipFilterOfGroup = cSocialUserGroup.RelationshipFilterOfGroup;

            // Local User
            if (LocalUser == null)
            {
                LocalUser = new XboxLiveUser(cSocialUserGroup.LocalUser);
            }
            LocalUser.Impl.UpdatePropertiesFromXboxLiveUserPtr();

            // Users

            // todo: for perf consider not removing everthing, but updating certain things and deleting the rest
            m_users.Clear();

            if (cSocialUserGroup.NumOfUsers > 0)
            {
                IntPtr[] cUsersArray = new IntPtr[cSocialUserGroup.NumOfUsers];

                Marshal.Copy(cSocialUserGroup.Users, cUsersArray, 0, cSocialUserGroup.NumOfUsers);


                for (int i = 0; i < cUsersArray.Count(); i++)
                {
                    var socialUser = new XboxSocialUser(cUsersArray[i]);
                    m_users[socialUser.XboxUserId] = socialUser;
                }
            }

            // Users Tracked

            // todo: for perf consider whether this list is static or dynamic
            m_trackedUsers.Clear();

            if (cSocialUserGroup.NumOfUsersTrackedBySocialUserGroup > 0)
            {
                IntPtr[] cTrackedUsers = new IntPtr[cSocialUserGroup.NumOfUsersTrackedBySocialUserGroup];

                Marshal.Copy(cSocialUserGroup.UsersTrackedBySocialUserGroup, cTrackedUsers, 0, cSocialUserGroup.NumOfUsersTrackedBySocialUserGroup);


                for (int i = 0; i < cTrackedUsers.Count(); i++)
                {
                    var cSocialUser = Marshal.PtrToStructure<SocialManager.XBOX_USER_ID_CONTAINER>(cTrackedUsers[i]);
                    string xuid = MarshalingHelpers.Utf8ToString(cSocialUser.XboxUserId);
                    m_trackedUsers.Add(xuid);
                }
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct XBOX_SOCIAL_USER_GROUP
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Users;

            [MarshalAs(UnmanagedType.I4)]
            public int NumOfUsers;

            [MarshalAs(UnmanagedType.U4)]
            public SocialUserGroupType SocialUserGroupType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersTrackedBySocialUserGroup;

            [MarshalAs(UnmanagedType.I4)]
            public int NumOfUsersTrackedBySocialUserGroup;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr LocalUser;

            [MarshalAs(UnmanagedType.U4)]
            public PresenceFilter PresenceFilterOfGroup;

            [MarshalAs(UnmanagedType.U4)]
            public RelationshipFilter RelationshipFilterOfGroup;
        }
    }
}
