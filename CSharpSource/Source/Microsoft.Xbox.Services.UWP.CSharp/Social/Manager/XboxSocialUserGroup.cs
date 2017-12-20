// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;

    public partial class XboxSocialUserGroup : IXboxSocialUserGroup
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
                var manager = (SocialManager)XboxLive.Instance.SocialManager;
                LocalUser = manager.GetUser(cSocialUserGroup.LocalUser);
            }
            LocalUser.Impl.UpdatePropertiesFromXboxLiveUserPtr();

            // Users

            // todo: for perf consider not removing everthing, but updating certain things and deleting the rest
            m_users.Clear();
            
            if (cSocialUserGroup.UsersCount > 0)
            {
                IntPtr[] cUsersArray = new IntPtr[cSocialUserGroup.UsersCount];

                Marshal.Copy(cSocialUserGroup.Users, cUsersArray, 0, (int)cSocialUserGroup.UsersCount);


                for (int i = 0; i < cUsersArray.Count(); i++)
                {
                    var socialUser = new XboxSocialUser(cUsersArray[i]);
                    m_users[socialUser.XboxUserId] = socialUser;
                }
            }

            // Users Tracked

            // todo: for perf consider whether this list is static or dynamic
            m_trackedUsers.Clear();
            
            if (cSocialUserGroup.UsersTrackedBySocialUserGroupCount > 0)
            {
                IntPtr[] cTrackedUsers = new IntPtr[cSocialUserGroup.UsersTrackedBySocialUserGroupCount];

                Marshal.Copy(cSocialUserGroup.UsersTrackedBySocialUserGroup, cTrackedUsers, 0, (int)cSocialUserGroup.UsersTrackedBySocialUserGroupCount);


                for (int i = 0; i < cTrackedUsers.Count(); i++)
                {
                    var cSocialUser = Marshal.PtrToStructure<SocialManager.XBOX_USER_ID_CONTAINER>(cTrackedUsers[i]);
                    string xuid = MarshalingHelpers.Utf8ToString(cSocialUser.XboxUserId);
                    m_trackedUsers.Add(xuid);
                }
            }
        }


        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr XboxSocialUserGroupGetUsersFromXboxUserIds(IntPtr group, IntPtr xboxUserIds, UInt32 xboxUserIdsCount, IntPtr usersSize);
        public IList<XboxSocialUser> GetUsersFromXboxUserIds(IList<string> xboxUserIds)
        {
            // Allocates memory for returned objects
            IntPtr cUsersCount = Marshal.AllocHGlobal(Marshal.SizeOf<Int32>());
            IntPtr cUserIds = MarshalingHelpers.StringListToHGlobalUtf8StringArray(xboxUserIds);

            // Invokes the c method
            IntPtr cUsersPtr = XboxSocialUserGroupGetUsersFromXboxUserIds(m_socialUserGroupPtr, cUserIds, (uint)xboxUserIds.Count, cUsersCount);

            // Does local work
            uint usersCount = (uint)Marshal.ReadInt32(cUsersCount);
            Marshal.FreeHGlobal(cUsersCount);

            List<XboxSocialUser> users = new List<XboxSocialUser>();

            if (usersCount > 0)
            {
                IntPtr[] cUsers = new IntPtr[usersCount];
                Marshal.Copy(cUsersPtr, cUsers, 0, (int)usersCount);

                foreach (IntPtr cUser in cUsers)
                {
                    users.Add(new XboxSocialUser(cUser));
                }
            }

            // Cleans up parameters
            MarshalingHelpers.FreeHGlobalUtf8StringArray(cUserIds, xboxUserIds.Count());

            return users.AsReadOnly();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XBOX_SOCIAL_USER_GROUP
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Users;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 UsersCount;

            [MarshalAs(UnmanagedType.U4)]
            public SocialUserGroupType SocialUserGroupType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersTrackedBySocialUserGroup;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 UsersTrackedBySocialUserGroupCount;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr LocalUser;

            [MarshalAs(UnmanagedType.U4)]
            public PresenceFilter PresenceFilterOfGroup;

            [MarshalAs(UnmanagedType.U4)]
            public RelationshipFilter RelationshipFilterOfGroup;
        }
    }
}
