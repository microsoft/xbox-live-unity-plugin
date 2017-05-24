// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;

    public class SocialEvent
    {
        public SocialEvent(SocialEventType type, XboxLiveUser user, IList<ulong> usersAffected = null, XboxSocialUserGroup groupAffected = null, Exception exception = null)
        {
            this.EventType = type;
            this.User = user;
            this.UsersAffected = usersAffected == null ? new List<ulong>() : usersAffected;
            this.GroupAffected = groupAffected;
            this.Exception = exception;
        }

        public SocialEventType EventType { get; private set; }

        public XboxLiveUser User { get; private set; }

        public IList<ulong> UsersAffected { get; private set; }

        public XboxSocialUserGroup GroupAffected { get; private set; }

        public Exception Exception { get; private set; }
    }
}