// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using static SocialManager;

    public class XboxSocialUserGroup : IEnumerable<XboxSocialUser>
    {
        // Pointer to the c object
        private IntPtr m_socialUserGroupPtr;

        // Properties
        private readonly Dictionary<string, XboxSocialUser> m_users = new Dictionary<string, XboxSocialUser>();
        private List<string> m_trackedUsers = new List<string>();

        internal XboxSocialUserGroup(IntPtr socialUserGroupPtr)
        {
            m_socialUserGroupPtr = socialUserGroupPtr;
            Refresh();
        }

        public XboxLiveUser LocalUser { get; private set; }

        public SocialUserGroupType SocialUserGroupType { get; private set; }

        public PresenceFilter PresenceFilterOfGroup { get; private set; }

        public RelationshipFilter RelationshipFilterOfGroup { get; private set; }
        

        public int Count
        {
            get
            {
                return this.m_users.Count;
            }
        }

        public IReadOnlyList<XboxSocialUser> Users
        {
            get
            {
                return this.m_users.Values.ToList();
            }
        }

        public IReadOnlyList<string> UsersTrackedBySocialUserGroup
        {
            get
            {
                return this.m_trackedUsers;
            }
        }

        public IReadOnlyList<XboxSocialUser> GetUsersFromXboxUserIds(IReadOnlyList<string> xboxUserIds)
        {
            List<XboxSocialUser> users = new List<XboxSocialUser>();

            foreach (string xboxUserId in xboxUserIds)
            {
                if (m_users.ContainsKey(xboxUserId))
                {
                    users.Add(m_users[xboxUserId]);
                }
                else
                {
                    // todo handle error
                }
            }

            return users;
        }

        public IEnumerator<XboxSocialUser> GetEnumerator()
        {
            return this.m_users.Values.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal IntPtr GetPtr()
        {
            return m_socialUserGroupPtr;
        }

        // todo For perf consider if we need an init method and a refresh method seperate
        internal void Refresh()
        {
            var cSocialUserGroup = Marshal.PtrToStructure<XboxSocialUserGroup_c>(m_socialUserGroupPtr);

            // Properties
            SocialUserGroupType = cSocialUserGroup.SocialUserGroupType;
            PresenceFilterOfGroup = cSocialUserGroup.PresenceFilterOfGroup;
            RelationshipFilterOfGroup = cSocialUserGroup.RelationshipFilterOfGroup;

            // Local User
            if (LocalUser == null)
            {
                LocalUser = new XboxLiveUser(cSocialUserGroup.LocalUser);
            }
            LocalUser.Impl.UpdatePropertiesFromXboxLiveUser_c();

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
                    var cSocialUser = Marshal.PtrToStructure<XboxUserIdContainer_c>(cTrackedUsers[i]);
                    m_trackedUsers.Add(cSocialUser.XboxUserId);
                }
            }
        }
    }
}