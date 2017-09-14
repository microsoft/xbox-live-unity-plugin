// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;

    public interface ISocialManager
    {
        IList<XboxLiveUser> LocalUsers { get; }

        void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel = SocialManagerExtraDetailLevel.NoExtraDetail);

        void RemoveLocalUser(XboxLiveUser user);

        XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IList<string> xboxUserIdList);

        XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter);

        IList<SocialEvent> DoWork();

        void UpdateSocialUserGroup(XboxSocialUserGroup group, IList<string> users);

        void DestroySocialUserGroup(XboxSocialUserGroup group);

        void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling);
    }
}