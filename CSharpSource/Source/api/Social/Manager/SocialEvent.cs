// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using static SocialManager;

    public class SocialEvent
    {
        internal SocialEvent(IntPtr socialEventPtr, IList<XboxSocialUserGroup> groups)
        {
            SocialEvent_c cSocialEvent = Marshal.PtrToStructure<SocialEvent_c>(socialEventPtr);
            EventType = cSocialEvent.EventType;

            User = new XboxLiveUser(cSocialEvent.User);

            try
            {
                SocialUserGroupLoadedArgs_c cArgs = Marshal.PtrToStructure<SocialUserGroupLoadedArgs_c>(cSocialEvent.EventArgs);

                foreach (XboxSocialUserGroup group in groups)
                {
                    if (cArgs.SocialUserGroup == group.mSocialUserGroupPtr)
                    {
                        GroupAffected = group;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }

            UsersAffected = new List<string>();
            if (cSocialEvent.NumOfUsersAffected > 0)
            {
                IntPtr[] cUsersAffected = new IntPtr[cSocialEvent.NumOfUsersAffected];
                Marshal.Copy(cSocialEvent.UsersAffected, cUsersAffected, 0, cSocialEvent.NumOfUsersAffected);
                foreach (IntPtr cXuidPtr in cUsersAffected)
                {
                    XboxUserIdContainer_c cXuid = Marshal.PtrToStructure<XboxUserIdContainer_c>(cXuidPtr);
                    UsersAffected.Add(cXuid.XboxUserId);
                }
            }
            

            if (!string.IsNullOrEmpty(cSocialEvent.ErrorMessage))
            {
                Exception = new Exception(cSocialEvent.ErrorMessage);
            }
        }

        public SocialEvent(SocialEventType type, XboxLiveUser user, IList<string> usersAffected = null, XboxSocialUserGroup groupAffected = null, Exception exception = null)
        {
            this.EventType = type;
            this.User = user;
            this.UsersAffected = usersAffected == null ? new List<string>() : usersAffected;
            this.GroupAffected = groupAffected;
            this.Exception = exception;
        }

        public SocialEventType EventType { get; private set; }

        public XboxLiveUser User { get; private set; }

        public IList<string> UsersAffected { get; private set; }

        public XboxSocialUserGroup GroupAffected { get; private set; }

        public Exception Exception { get; private set; }
    }
}