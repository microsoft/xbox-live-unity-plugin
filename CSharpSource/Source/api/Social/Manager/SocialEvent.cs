// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;

    public partial class SocialEvent
    {
        // todo do I need this method?
        internal SocialEvent(SocialEventType type, XboxLiveUser user, IList<string> usersAffected = null, XboxSocialUserGroup groupAffected = null, int errorCode = 0, string errorMessage = "")
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

        public IList<string> UsersAffected { get; private set; }
                
        public int ErrorCode { get; private set; }

        public string ErrorMessge { get; private set; }
    }
}