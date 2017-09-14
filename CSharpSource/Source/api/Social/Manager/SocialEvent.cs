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
                    if (cArgs.SocialUserGroup == group.GetPtr())
                    {
                        EventArgs = new SocialUserGroupLoadedEventArgs(group);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // Event args weren't SocialUserGroupLoadedArgs
            }

            List<string> usersAffected = new List<string>();
            if (cSocialEvent.NumOfUsersAffected > 0)
            {
                IntPtr[] cUsersAffected = new IntPtr[cSocialEvent.NumOfUsersAffected];
                Marshal.Copy(cSocialEvent.UsersAffected, cUsersAffected, 0, cSocialEvent.NumOfUsersAffected);
                foreach (IntPtr cXuidPtr in cUsersAffected)
                {
                    XboxUserIdContainer_c cXuid = Marshal.PtrToStructure<XboxUserIdContainer_c>(cXuidPtr);
                    usersAffected.Add(cXuid.XboxUserId);
                }
            }
            UsersAffected = usersAffected;
            
            ErrorCode = cSocialEvent.ErrorCode;
            ErrorMessge = cSocialEvent.ErrorMessage;
        }

        // todo do I need this method?
        internal SocialEvent(SocialEventType type, XboxLiveUser user, IReadOnlyList<string> usersAffected = null, XboxSocialUserGroup groupAffected = null, int errorCode = 0, string errorMessage = "")
        {
            this.EventType = type;
            this.User = user;
            this.UsersAffected = usersAffected == null ? new List<string>() : usersAffected;
            this.EventArgs = groupAffected == null ? null : new SocialUserGroupLoadedEventArgs(groupAffected);
            this.ErrorCode = errorCode;
            this.ErrorMessge = errorMessage;
        }

        public SocialEventType EventType { get; private set; }
        
        public SocialEventArgs EventArgs { get; private set; }

        public XboxLiveUser User { get; private set; }

        public IReadOnlyList<string> UsersAffected { get; private set; }
                
        public int ErrorCode { get; private set; }

        public string ErrorMessge { get; private set; }
    }
}