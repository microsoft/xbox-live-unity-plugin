// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.System;
    using Presence;

    public class SocialManager : ISocialManager
    {
        private static ISocialManager m_instance;

        private static readonly object m_instanceLock = new object();
        
        private readonly List<XboxLiveUser> m_localUsers = new List<XboxLiveUser>();
        private readonly List<XboxSocialUserGroup> m_groups = new List<XboxSocialUserGroup>();

        private SocialManager()
        {
        }

        public static ISocialManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_instanceLock)
                    {
                        if (m_instance == null)
                        {
                            m_instance = XboxLive.UseMockServices ? (ISocialManager)new MockSocialManager() : (ISocialManager)new SocialManager();
                        }
                    }
                }
                return m_instance;
            }
        }

        public IReadOnlyList<XboxLiveUser> LocalUsers
        {
            get
            {
                return this.m_localUsers;
            }
        }

        public void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            if (user == null) throw new ArgumentNullException("user");

            XboxLive.Instance.Invoke<SocialManagerAddLocalUser>(user.Impl.GetPtr(), extraDetailLevel);
            m_localUsers.Add(user);
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            
            XboxLive.Instance.Invoke<SocialManagerRemoveLocalUser>(user.Impl.GetPtr());
            m_localUsers.Remove(user);
        }
        
        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr socialUserGroupPtr = XboxLive.Instance.Invoke<IntPtr, SocialManagerCreateSocialUserGroupFromFilters>(user.Impl.GetPtr(), presenceFilter, relationshipFilter);
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(socialUserGroupPtr);
            m_groups.Add(socialUserGroup);
            
            return socialUserGroup;
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IReadOnlyList<string> xboxUserIdList)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (xboxUserIdList == null) throw new ArgumentNullException("xboxUserIdList");

            List<IntPtr> userIdPtrs = new List<IntPtr>();
            for (int i = 0; i < xboxUserIdList.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalUni(xboxUserIdList[i]);
                userIdPtrs.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * xboxUserIdList.Count);
            Marshal.Copy(userIdPtrs.ToArray(), 0, cUserIds, xboxUserIdList.Count);

            IntPtr socialUserGroupPtr = XboxLive.Instance.Invoke<IntPtr, SocialManagerCreateSocialUserGroupFromList>(user.Impl.GetPtr(), cUserIds, xboxUserIdList.Count);
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(socialUserGroupPtr);
            m_groups.Add(socialUserGroup);

            return socialUserGroup;
        }

        public void UpdateSocialUserGroup(XboxSocialUserGroup socialGroup, IReadOnlyList<string> users)
        {
            if (socialGroup == null) throw new ArgumentNullException("socialGroup");
            if (users == null) throw new ArgumentNullException("users");

            List<IntPtr> userIds = new List<IntPtr>();
            for (int i = 0; i < users.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalUni(users[i]);
                userIds.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * users.Count);
            Marshal.Copy(userIds.ToArray(), 0, cUserIds, users.Count);

            XboxLive.Instance.Invoke<SocialManagerUpdateSocialUserGroup>(socialGroup.GetPtr(), cUserIds, users.Count);
            socialGroup.Refresh();
        }

        public void DestroySocialUserGroup(XboxSocialUserGroup xboxSocialUserGroup)
        {
            if (xboxSocialUserGroup == null) throw new ArgumentNullException("xboxSocialUserGroup");

            m_groups.Remove(xboxSocialUserGroup);

            XboxLive.Instance.Invoke<SocialManagerDestroySocialUserGroup>(xboxSocialUserGroup.GetPtr());
        }

        public IReadOnlyList<SocialEvent> DoWork()
        {
            IntPtr cNumOfEvents = Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            IntPtr eventsPtr = XboxLive.Instance.Invoke<IntPtr, SocialManagerDoWork>(cNumOfEvents);

            int numOfEvents = Marshal.ReadInt32(cNumOfEvents);
            Marshal.FreeHGlobal(cNumOfEvents);

            List<SocialEvent> events = new List<SocialEvent>();

            if (numOfEvents > 0)
            {
                IntPtr[] cEvents = new IntPtr[numOfEvents];
                Marshal.Copy(eventsPtr, cEvents, 0, numOfEvents);
                
                foreach (IntPtr cEvent in cEvents)
                {
                    events.Add(new SocialEvent(cEvent, m_groups));
                }

                // Update Objects
                foreach (XboxSocialUserGroup group in m_groups)
                {
                    if (group != null)
                    {
                        group.Refresh();
                    }
                }
                foreach (XboxLiveUser user in m_localUsers)
                {
                    user.Impl.UpdatePropertiesFromXboxLiveUser_c();
                }
            }
            
            return events;
        }

        public void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling)
        {
            if (user == null) throw new ArgumentNullException("user");

            XboxLive.Instance.Invoke<SocialManagerSetRichPresencePollingStatus>(user.Impl.GetPtr(), shouldEnablePolling);
            user.Impl.UpdatePropertiesFromXboxLiveUser_c();
        }

        // Marshalling

        private delegate void SocialManagerAddLocalUser(IntPtr user, SocialManagerExtraDetailLevel extraDetailLevel);
        private delegate void SocialManagerRemoveLocalUser(IntPtr user);
        private delegate IntPtr SocialManagerCreateSocialUserGroupFromFilters(IntPtr user, PresenceFilter presenceDetailFilter, RelationshipFilter filter);
        private delegate IntPtr SocialManagerCreateSocialUserGroupFromList(IntPtr group, IntPtr users, int size);
        private delegate IntPtr SocialManagerUpdateSocialUserGroup(IntPtr group, IntPtr users, int size);
        private delegate IntPtr SocialManagerDestroySocialUserGroup(IntPtr group);
        private delegate IntPtr SocialManagerDoWork(IntPtr numOfEvents);
        private delegate void SocialManagerSetRichPresencePollingStatus(IntPtr user, bool shouldEnablePolling);

        [StructLayout(LayoutKind.Sequential)]
        internal struct TitleHistory_c
        {
            [MarshalAs(UnmanagedType.U1)]
            public byte UserHasPlayed;

            [MarshalAs(UnmanagedType.I8)]
            public long LastTimeUserPlayed;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PreferredColor_c
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string PrimaryColor;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string SecondaryColor;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string TertiaryColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialManagerPresenceTitleRecord_c
        {

            [MarshalAs(UnmanagedType.U1)]
            public byte IsTitleActive;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsBroadcasting;

            [MarshalAs(UnmanagedType.U4)]
            public PresenceDeviceType DeviceType;

            [MarshalAs(UnmanagedType.U4)]
            public uint TitleId;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string PresenceText; 
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialManagerPresenceRecord_c
        {
            [MarshalAs(UnmanagedType.U4)]
            public UserPresenceState UserState;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PresenceTitleRecords;

            [MarshalAs(UnmanagedType.I4)]
            public int NumOfPresenceTitleRecords;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XboxSocialUser_c
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string XboxUserId;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsFavorite;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsFollowingUser;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsFollowedByCaller;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string RealName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string DisplayPicUrlRaw;

            [MarshalAs(UnmanagedType.U1)]
            public byte UseAvatar;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Gamerscore;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Gamertag;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PresenceRecord;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr TitleHistory;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PreferredColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialEvent_c
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr User;

            [MarshalAs(UnmanagedType.U4)]
            public SocialEventType EventType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersAffected;

            [MarshalAs(UnmanagedType.I4)]
            public int NumOfUsersAffected;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr EventArgs;
            
            [MarshalAs(UnmanagedType.I4)]
            public int ErrorCode;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ErrorMessage;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialUserGroupLoadedArgs_c
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr SocialUserGroup;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XboxUserIdContainer_c
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string XboxUserId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XboxSocialUserGroup_c
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