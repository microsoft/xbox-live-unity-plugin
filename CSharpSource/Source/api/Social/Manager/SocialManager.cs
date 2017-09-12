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

    public class SocialManager : ISocialManager
    {
        internal static int WorkDone;
        private static ISocialManager instance;

        private static readonly object instanceLock = new object();
        private readonly object syncRoot = new object();

        private readonly List<XboxLiveUser> localUsers = new List<XboxLiveUser>();
        private readonly Dictionary<XboxLiveUser, SocialGraph> userGraphs = new Dictionary<XboxLiveUser, SocialGraph>(new XboxUserIdEqualityComparer());
        private readonly Dictionary<XboxLiveUser, HashSet<WeakReference>> userGroupsMap = new Dictionary<XboxLiveUser, HashSet<WeakReference>>(new XboxUserIdEqualityComparer());

        private Queue<SocialEvent> eventQueue = new Queue<SocialEvent>();

        private List<XboxSocialUserGroup> mGroups;

        private SocialManager()
        {
            mGroups = new List<XboxSocialUserGroup>();
        }

        internal static ISocialManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = XboxLive.UseMockServices ? new MockSocialManager() : (ISocialManager)new SocialManager();
                        }
                    }
                }
                return instance;
            }
        }

        public IList<XboxLiveUser> LocalUsers
        {
            get
            {
                return this.localUsers.AsReadOnly();
            }
        }

        private delegate void SocialManagerAddLocalUser(IntPtr user, SocialManagerExtraDetailLevel extraDetailLevel);
        public void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            if (user == null) throw new ArgumentNullException("user");

            XboxLive.Instance.Invoke<SocialManagerAddLocalUser>(user.Impl.m_xboxLiveUser_c, extraDetailLevel);
        }

        private delegate void SocialManagerRemoveLocalUser(IntPtr user);
        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            
            XboxLive.Instance.Invoke<SocialManagerRemoveLocalUser>(user.Impl.m_xboxLiveUser_c);
        }
        
        private delegate IntPtr SocialManagerCreateSocialUserGroupFromFilters(IntPtr user, PresenceFilter presenceDetailFilter, RelationshipFilter filter);
        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr socialUserGroupPtr = XboxLive.Instance.Invoke<IntPtr, SocialManagerCreateSocialUserGroupFromFilters>(user.Impl.m_xboxLiveUser_c, presenceFilter, relationshipFilter);
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(socialUserGroupPtr);
            mGroups.Add(socialUserGroup);
            
            return socialUserGroup;
        }

        private delegate IntPtr SocialManagerCreateSocialUserGroupFromList(IntPtr group, IntPtr users, int size);
        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, List<string> userIds)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (userIds == null) throw new ArgumentNullException("userIds");

            List<IntPtr> userIdPtrs = new List<IntPtr>();
            for (int i = 0; i < userIds.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalUni(userIds[i]);
                userIdPtrs.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * userIds.Count);
            Marshal.Copy(userIdPtrs.ToArray(), 0, cUserIds, userIds.Count);

            IntPtr socialUserGroupPtr = XboxLive.Instance.Invoke<IntPtr, SocialManagerCreateSocialUserGroupFromList>(user.Impl.m_xboxLiveUser_c, cUserIds, userIds.Count);
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(socialUserGroupPtr);
            mGroups.Add(socialUserGroup);

            return socialUserGroup;
        }

        private delegate IntPtr SocialManagerUpdateSocialUserGroup(IntPtr group, IntPtr users, int size);
        public void UpdateSocialUserGroup(XboxSocialUserGroup group, List<string> users)
        {

            List<IntPtr> userIds = new List<IntPtr>();
            for (int i = 0; i < users.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalUni(users[i]);
                userIds.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * users.Count);
            Marshal.Copy(userIds.ToArray(), 0, cUserIds, users.Count);

            XboxLive.Instance.Invoke<SocialManagerUpdateSocialUserGroup>(group.mSocialUserGroupPtr, cUserIds, users.Count);
            group.Refresh();
        }

        private delegate IntPtr SocialManagerDestroySocialUserGroup(IntPtr group);
        public void DestroySocialUserGroup(XboxSocialUserGroup group)
        {
            mGroups.Remove(group);

            XboxLive.Instance.Invoke<SocialManagerDestroySocialUserGroup>(group.mSocialUserGroupPtr);
        }

        private delegate IntPtr SocialManagerDoWork(IntPtr numOfEvents);
        public IList<SocialEvent> DoWork()
        {
            IntPtr cNumOfEvents = Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            IntPtr eventsPtr = XboxLive.Instance.Invoke<IntPtr, SocialManagerDoWork>(cNumOfEvents);

            int numOfEvents = Marshal.ReadInt32(cNumOfEvents);
            Marshal.FreeHGlobal(cNumOfEvents);

            List<SocialEvent> events = new List<SocialEvent>();

            if (numOfEvents > 0)
            {
                WorkDone++;
                IntPtr[] cEvents = new IntPtr[numOfEvents];
                Marshal.Copy(eventsPtr, cEvents, 0, numOfEvents);
                
                foreach (IntPtr cEvent in cEvents)
                {
                    events.Add(new SocialEvent(cEvent, mGroups));
                }

                foreach (XboxSocialUserGroup group in mGroups)
                {
                    if (group != null)
                    {
                        group.Refresh();
                    }
                }
            }


            return events;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TitleHistory_c
        {
            [MarshalAs(UnmanagedType.U1)]
            public byte UserHasPlayed;

            // todo: m_lastTimeUserPlayed
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

            // todo: presence_device_type

            [MarshalAs(UnmanagedType.I4)]
            public int TitleId;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string PresenceText; 
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialManagerPresenceRecord_c
        {
            // todo: user_presence_state

            // todo: presenceTitleRecords

            [MarshalAs(UnmanagedType.SysInt)]
            public int NumOfPresenceTitleRecords; 

            // todo: is_user_playing_title
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

            // todo
            //[MarshalAs(UnmanagedType.SysInt)]
            //public IntPtr Error;

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