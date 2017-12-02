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
                LocalUser = new XboxLiveUser(cSocialUserGroup.LocalUser);
            }
            LocalUser.Impl.UpdatePropertiesFromXboxLiveUserPtr();

            // Users

            // todo: for perf consider not removing everthing, but updating certain things and deleting the rest
            m_users.Clear();

            int usersSize = cSocialUserGroup.UsersSize.ToInt32();
            if (usersSize > 0)
            {
                IntPtr[] cUsersArray = new IntPtr[usersSize];

                Marshal.Copy(cSocialUserGroup.Users, cUsersArray, 0, usersSize);


                for (int i = 0; i < cUsersArray.Count(); i++)
                {
                    var socialUser = new XboxSocialUser(cUsersArray[i]);
                    m_users[socialUser.XboxUserId] = socialUser;
                }
            }

            // Users Tracked

            // todo: for perf consider whether this list is static or dynamic
            m_trackedUsers.Clear();

            int usersTrackedBySocialUserGroupSize = cSocialUserGroup.UsersTrackedBySocialUserGroupSize.ToInt32();
            if (usersTrackedBySocialUserGroupSize > 0)
            {
                IntPtr[] cTrackedUsers = new IntPtr[usersTrackedBySocialUserGroupSize];

                Marshal.Copy(cSocialUserGroup.UsersTrackedBySocialUserGroup, cTrackedUsers, 0, usersTrackedBySocialUserGroupSize);


                for (int i = 0; i < cTrackedUsers.Count(); i++)
                {
                    var cSocialUser = Marshal.PtrToStructure<SocialManager.XBOX_USER_ID_CONTAINER>(cTrackedUsers[i]);
                    string xuid = MarshalingHelpers.Utf8ToString(cSocialUser.XboxUserId);
                    m_trackedUsers.Add(xuid);
                }
            }
        }


        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr XboxSocialUserGroupGetUsersFromXboxUserIds(IntPtr group, IntPtr xboxUserIds, int xboxUserIdsSize, IntPtr usersSize);
        public IList<XboxSocialUser> GetUsersFromXboxUserIds(IList<string> xboxUserIds)
        {
            // Allocates memory for returned objects
            IntPtr cUsersSize = Marshal.AllocHGlobal(Marshal.SizeOf<Int32>());

            List<IntPtr> userIdPtrs = new List<IntPtr>();
            for (int i = 0; i < xboxUserIds.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalAnsi(xboxUserIds[i]);
                userIdPtrs.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * xboxUserIds.Count);
            Marshal.Copy(userIdPtrs.ToArray(), 0, cUserIds, xboxUserIds.Count);

            // Invokes the c method
            IntPtr cUsersPtr = XboxSocialUserGroupGetUsersFromXboxUserIds(m_socialUserGroupPtr, cUserIds, userIdPtrs.Count(), cUsersSize);

            // Does local work
            int usersSize = Marshal.ReadInt32(cUsersSize);
            Marshal.FreeHGlobal(cUsersSize);

            List<XboxSocialUser> users = new List<XboxSocialUser>();

            if (usersSize > 0)
            {
                IntPtr[] cUsers = new IntPtr[usersSize];
                Marshal.Copy(cUsersPtr, cUsers, 0, usersSize);

                foreach (IntPtr cUser in cUsers)
                {
                    users.Add(new XboxSocialUser(cUser));
                }
            }

            // Cleans up parameters
            foreach (IntPtr ptr in userIdPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
            Marshal.FreeHGlobal(cUserIds);

            return users.AsReadOnly();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XBOX_SOCIAL_USER_GROUP
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Users;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersSize;

            [MarshalAs(UnmanagedType.U4)]
            public SocialUserGroupType SocialUserGroupType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersTrackedBySocialUserGroup;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersTrackedBySocialUserGroupSize;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr LocalUser;

            [MarshalAs(UnmanagedType.U4)]
            public PresenceFilter PresenceFilterOfGroup;

            [MarshalAs(UnmanagedType.U4)]
            public RelationshipFilter RelationshipFilterOfGroup;
        }
    }
}
