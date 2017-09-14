// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Presence;
    using Microsoft.Xbox.Services.System;

    // todo update this method
    public class MockSocialManager : ISocialManager
    {
        public IList<XboxLiveUser> LocalUsers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel = SocialManagerExtraDetailLevel.NoExtraDetail)
        {
            throw new NotImplementedException();
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter)
        {
            throw new NotImplementedException();
        }

        public XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IList<string> xboxUserIdList)
        {
            throw new NotImplementedException();
        }

        public void DestroySocialUserGroup(XboxSocialUserGroup group)
        {
            throw new NotImplementedException();
        }

        public IList<SocialEvent> DoWork()
        {
            throw new NotImplementedException();
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling)
        {
            throw new NotImplementedException();
        }

        public void UpdateSocialUserGroup(XboxSocialUserGroup group, IList<string> users)
        {
            throw new NotImplementedException();
        }
    }
}