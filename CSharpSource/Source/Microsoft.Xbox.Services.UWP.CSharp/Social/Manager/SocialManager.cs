// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Presence;
    using System;

    public partial class SocialManager : ISocialManager
    {
        private readonly List<XboxLiveUser> m_localUsers = new List<XboxLiveUser>();
        private readonly List<XboxSocialUserGroup> m_groups = new List<XboxSocialUserGroup>();

        public IList<XboxLiveUser> LocalUsers
        {
            get
            {
                return this.m_localUsers.AsReadOnly();
            }
        }

        public void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerAddLocalUser(user.Impl.XboxLiveUserPtr, extraDetailLevel, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            m_localUsers.Add(user);
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerRemoveLocalUser(user.Impl.XboxLiveUserPtr, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            m_localUsers.Remove(user);
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cGroupPtr = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerCreateSocialUserGroupFromFilters(user.Impl.XboxLiveUserPtr, presenceFilter, relationshipFilter, cGroupPtr, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Handles returned objects
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(Marshal.ReadIntPtr(cGroupPtr));
            Marshal.FreeHGlobal(cGroupPtr);
            m_groups.Add(socialUserGroup);

            return socialUserGroup;
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IList<string> xboxUserIdList)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (xboxUserIdList == null) throw new ArgumentNullException("xboxUserIdList");

            // Allocates memory for parameters
            List<IntPtr> userIdPtrs = new List<IntPtr>();
            for (int i = 0; i < xboxUserIdList.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalAnsi(xboxUserIdList[i]);
                userIdPtrs.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * xboxUserIdList.Count);
            Marshal.Copy(userIdPtrs.ToArray(), 0, cUserIds, xboxUserIdList.Count);

            // Allocates memory for returned objects
            IntPtr cGroupPtr = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerCreateSocialUserGroupFromList(user.Impl.XboxLiveUserPtr, cUserIds, (uint)xboxUserIdList.Count, cGroupPtr, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Cleans up parameters
            foreach (IntPtr ptr in userIdPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
            Marshal.FreeHGlobal(cUserIds);

            // Handles returned objects
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(Marshal.ReadIntPtr(cGroupPtr));
            Marshal.FreeHGlobal(cGroupPtr);
            m_groups.Add(socialUserGroup);

            return socialUserGroup;
        }

        public void UpdateSocialUserGroup(XboxSocialUserGroup socialGroup, IList<string> users)
        {
            if (socialGroup == null) throw new ArgumentNullException("socialGroup");
            if (users == null) throw new ArgumentNullException("users");

            // Allocates memory for parameters
            List<IntPtr> userIdPtrs = new List<IntPtr>();
            for (int i = 0; i < users.Count; i++)
            {
                IntPtr cXuid = Marshal.StringToHGlobalUni(users[i]);
                userIdPtrs.Add(cXuid);
            }
            IntPtr cUserIds = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * users.Count);
            Marshal.Copy(userIdPtrs.ToArray(), 0, cUserIds, users.Count);
            
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerUpdateSocialUserGroup(socialGroup.GetPtr(), cUserIds, (uint)users.Count, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringUni(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Cleans up parameters
            foreach (IntPtr ptr in userIdPtrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
            Marshal.FreeHGlobal(cUserIds);

            // Does local work
            socialGroup.Refresh();
        }

        public void DestroySocialUserGroup(XboxSocialUserGroup xboxSocialUserGroup)
        {
            if (xboxSocialUserGroup == null) throw new ArgumentNullException("xboxSocialUserGroup");
            
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerDestroySocialUserGroup(xboxSocialUserGroup.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
            
            // Does local work
            m_groups.Remove(xboxSocialUserGroup);
        }

        public IList<SocialEvent> DoWork()
        {

            // Allocates memory for returned objects
            IntPtr cEventsCount = Marshal.AllocHGlobal(Marshal.SizeOf<Int32>());

            // Invokes the c method
            IntPtr eventsPtr = SocialManagerDoWork(cEventsCount);
            
            // Does local work
            uint eventsCount = (uint)Marshal.ReadInt32(cEventsCount);
            Marshal.FreeHGlobal(cEventsCount);

            List<SocialEvent> events = new List<SocialEvent>();

            if (eventsCount > 0)
            {
                IntPtr[] cEvents = new IntPtr[eventsCount];
                Marshal.Copy(eventsPtr, cEvents, 0, (int)eventsCount);

                foreach (IntPtr cEvent in cEvents)
                {
                    events.Add(new SocialEvent(cEvent, m_groups));
                }

                // Refresh object
                foreach (XboxSocialUserGroup group in m_groups)
                {
                    if (group != null)
                    {
                        group.Refresh();
                    }
                }
                foreach (XboxLiveUser user in m_localUsers)
                {
                    user.Impl.UpdatePropertiesFromXboxLiveUserPtr();
                }
            }

            return events.AsReadOnly();
        }

        public void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerSetRichPresencePollingStatus(user.Impl.XboxLiveUserPtr, shouldEnablePolling, cErrMessage);
            
            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Does local work
            user.Impl.UpdatePropertiesFromXboxLiveUserPtr();
        }

        // Marshaling
        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerAddLocalUser(IntPtr user, SocialManagerExtraDetailLevel extraDetailLevel, IntPtr errMessage);
        
        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerRemoveLocalUser(IntPtr user, IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerCreateSocialUserGroupFromFilters(IntPtr user, PresenceFilter presenceDetailFilter, RelationshipFilter filter, IntPtr returnGroup, IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerCreateSocialUserGroupFromList(IntPtr group, IntPtr users, UInt32 usersCount, IntPtr returnGroup, IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerUpdateSocialUserGroup(IntPtr group, IntPtr users, UInt32 usersCount, IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerDestroySocialUserGroup(IntPtr group, IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr SocialManagerDoWork(IntPtr numOfEvents);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerSetRichPresencePollingStatus(IntPtr user, bool shouldEnablePolling, IntPtr errMessage);


        [StructLayout(LayoutKind.Sequential)]
        internal struct SOCIAL_USER_GROUP_LOADED_ARGS
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr SocialUserGroup;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XBOX_USER_ID_CONTAINER
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr XboxUserId;
        }
    }
}
