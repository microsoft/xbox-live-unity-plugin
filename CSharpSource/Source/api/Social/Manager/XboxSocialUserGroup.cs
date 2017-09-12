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

    public class XboxSocialUserGroup : IEnumerable<XboxSocialUser>
    {
        // Pointer to the c object
        internal IntPtr mSocialUserGroupPtr;

        // Properties
        private XboxLiveUser mLocalUser;
        private readonly Dictionary<string, XboxSocialUser> mUsers = new Dictionary<string, XboxSocialUser>();
        private List<ulong> mTrackedUsers = new List<ulong>();

        int mVersion;

        internal XboxSocialUserGroup(IntPtr cSocialUserGroup)
        {
            mSocialUserGroupPtr = cSocialUserGroup;
            Refresh();
        }

        public SocialUserGroupType SocialUserGroupType { get; private set; }

        public PresenceFilter PresenceFilter { get; private set; }

        public RelationshipFilter RelationshipFilter { get; private set; }

        // todo: Was private in cpp, should remove? I think yes
        public uint TitleId { get; set; }

        public int Count
        {
            get
            {
                return this.mUsers.Count;
            }
        }

        public IList<XboxSocialUser> Users
        {
            get
            {
                if (mVersion != SocialManager.WorkDone)
                    Refresh();

                return this.mUsers.Values.ToList();
            }
        }

        public IList<ulong> UsersTrackedBySocialUserGroup
        {
            get
            {
                if (mVersion != SocialManager.WorkDone)
                    Refresh();

                return this.mTrackedUsers;
            }
        }

        public XboxSocialUser GetUser(string userId)
        {
            if (mVersion != SocialManager.WorkDone)
                Refresh();

            XboxSocialUser user;
            this.mUsers.TryGetValue(userId, out user);
            return user;
        }

        public IEnumerator<XboxSocialUser> GetEnumerator()
        {
            if (mVersion != SocialManager.WorkDone)
                Refresh();

            return this.mUsers.Values.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            if (mVersion != SocialManager.WorkDone)
                Refresh();

            return this.GetEnumerator();
        }

        // For perf consider if we need an init method and a refresh method seperate
        internal void Refresh()
        {
            mVersion = SocialManager.WorkDone;
            var cSocialUserGroup = Marshal.PtrToStructure<XboxSocialUserGroup_c>(mSocialUserGroupPtr);

            // Properties
            SocialUserGroupType = cSocialUserGroup.SocialUserGroupType;
            PresenceFilter = cSocialUserGroup.PresenceFilterOfGroup;
            RelationshipFilter = cSocialUserGroup.RelationshipFilterOfGroup;

            // Local User
            if (mLocalUser == null)
            {
                mLocalUser = new XboxLiveUser(cSocialUserGroup.LocalUser);
            }
            mLocalUser.Impl.UpdatePropertiesFromXboxLiveUser_c();

            // Users

            // todo: for perf consider not removing everthing, but updating certain things and deleting the rest
            mUsers.Clear();

            if (cSocialUserGroup.NumOfUsers > 0)
            {
                IntPtr[] cUsersArray = new IntPtr[cSocialUserGroup.NumOfUsers];

                Marshal.Copy(cSocialUserGroup.Users, cUsersArray, 0, cSocialUserGroup.NumOfUsers);


                for (int i = 0; i < cUsersArray.Count(); i++)
                {
                    var socialUser = new XboxSocialUser(cUsersArray[i]);
                    mUsers[socialUser.XboxUserId] = socialUser; 
                }
            }

            // Users Tracked

            // todo: for perf consider whether this list is static or dynamic
            mTrackedUsers.Clear();

            if (cSocialUserGroup.NumOfUsersTrackedBySocialUserGroup > 0)
            {
                IntPtr[] cTrackedUsers = new IntPtr[cSocialUserGroup.NumOfUsersTrackedBySocialUserGroup];

                Marshal.Copy(cSocialUserGroup.UsersTrackedBySocialUserGroup, cTrackedUsers, 0, cSocialUserGroup.NumOfUsersTrackedBySocialUserGroup);


                for (int i = 0; i < cTrackedUsers.Count(); i++)
                {
                    var cSocialUser = Marshal.PtrToStructure<XboxUserIdContainer_c>(cTrackedUsers[i]);
                    var xboxUserId = ulong.Parse(cSocialUser.XboxUserId);
                    mTrackedUsers.Add(xboxUserId);
                }
            }
        }
    }
}