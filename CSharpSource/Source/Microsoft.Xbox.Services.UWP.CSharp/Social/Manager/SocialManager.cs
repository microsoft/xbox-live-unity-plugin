// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using System;

    public partial class SocialManager : ISocialManager
    {
        private readonly Dictionary<IntPtr, XboxLiveUser> m_localUsers = new Dictionary<IntPtr, XboxLiveUser>();
        private readonly List<XboxSocialUserGroup> m_groups = new List<XboxSocialUserGroup>();

        internal XboxLiveUser GetUser(IntPtr userPtr)
        {
            if (m_localUsers.ContainsKey(userPtr))
            {
                return m_localUsers[userPtr];
            }

            throw new XboxException("User doesn't exist. Did you call AddLocalUser?");
        }

        public IList<XboxLiveUser> LocalUsers
        {
            get
            {
                return this.m_localUsers.Values.ToList().AsReadOnly();
            }
        }

        public void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerAddLocalUser(user.Impl.XboxLiveUserPtr, extraDetailLevel, out cErrMessage);

            // Handles error
            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
            
            m_localUsers[user.Impl.XboxLiveUserPtr] = user;
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerRemoveLocalUser(user.Impl.XboxLiveUserPtr, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
            
            if (m_localUsers.ContainsKey(user.Impl.XboxLiveUserPtr))
            {
                m_localUsers.Remove(user.Impl.XboxLiveUserPtr);
            }
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cGroupPtr;
            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerCreateSocialUserGroupFromFilters(user.Impl.XboxLiveUserPtr, presenceFilter, relationshipFilter, out cGroupPtr, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Handles returned objects
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(cGroupPtr);
            m_groups.Add(socialUserGroup);

            return socialUserGroup;
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IList<string> xboxUserIdList)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (xboxUserIdList == null) throw new ArgumentNullException("xboxUserIdList");

            // Allocates memory for parameters
            IntPtr cUserIds = MarshalingHelpers.StringListToHGlobalUtf8StringArray(xboxUserIdList);

            // Allocates memory for returned objects
            IntPtr cGroupPtr;
            IntPtr cErrMessage;

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerCreateSocialUserGroupFromList(user.Impl.XboxLiveUserPtr, cUserIds, (uint)xboxUserIdList.Count, out cGroupPtr, out cErrMessage);

            MarshalingHelpers.FreeHGlobalUtf8StringArray(cUserIds, xboxUserIdList.Count);
            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);

            }
            
            // Handles returned objects
            XboxSocialUserGroup socialUserGroup = new XboxSocialUserGroup(cGroupPtr);
            m_groups.Add(socialUserGroup);

            return socialUserGroup;
        }

        public void UpdateSocialUserGroup(XboxSocialUserGroup socialGroup, IList<string> users)
        {
            if (socialGroup == null) throw new ArgumentNullException("socialGroup");
            if (users == null) throw new ArgumentNullException("users");

            // Allocates memory for parameters
            IntPtr cUserIds = MarshalingHelpers.StringListToHGlobalUtf8StringArray(users);
            
            // Allocates memory for returned objects
            IntPtr cErrMessage;

            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerUpdateSocialUserGroup(socialGroup.GetPtr(), cUserIds, (uint)users.Count, out cErrMessage);

            MarshalingHelpers.FreeHGlobalUtf8StringArray(cUserIds, users.Count);
            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Does local work
            socialGroup.Refresh();
        }

        public void DestroySocialUserGroup(XboxSocialUserGroup xboxSocialUserGroup)
        {
            if (xboxSocialUserGroup == null) throw new ArgumentNullException("xboxSocialUserGroup");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerDestroySocialUserGroup(xboxSocialUserGroup.GetPtr(), out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Does local work
            m_groups.Remove(xboxSocialUserGroup);
        }

        public IList<SocialEvent> DoWork()
        {
            UInt32 eventsCount;
            // Invokes the c method
            IntPtr eventsPtr = SocialManagerDoWork(out eventsCount);

            // Does local work
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

                foreach (XboxLiveUser user in m_localUsers.Values)
                {
                    user.Impl.UpdatePropertiesFromXboxLiveUserPtr();
                }
            }

            return events.AsReadOnly();
        }

        public void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = SocialManagerSetRichPresencePollingStatus(user.Impl.XboxLiveUserPtr, shouldEnablePolling, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Does local work
            user.Impl.UpdatePropertiesFromXboxLiveUserPtr();
        }

        // Marshaling
        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerAddLocalUser(IntPtr user, SocialManagerExtraDetailLevel extraDetailLevel, out IntPtr errMessage);
        
        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerRemoveLocalUser(IntPtr user, out IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerCreateSocialUserGroupFromFilters(IntPtr user, PresenceFilter presenceDetailFilter, RelationshipFilter filter, out IntPtr returnGroup, out IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerCreateSocialUserGroupFromList(IntPtr group, IntPtr users, UInt32 usersCount, out IntPtr returnGroup, out IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerUpdateSocialUserGroup(IntPtr group, IntPtr users, UInt32 usersCount, out IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerDestroySocialUserGroup(IntPtr group, out IntPtr errMessage);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr SocialManagerDoWork(out UInt32 numOfEvents);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT SocialManagerSetRichPresencePollingStatus(IntPtr user, bool shouldEnablePolling, out IntPtr errMessage);


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
