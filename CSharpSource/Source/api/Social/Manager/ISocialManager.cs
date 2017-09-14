// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xbox.Services.System;

namespace Microsoft.Xbox.Services.Social.Manager
{
    public interface ISocialManager
    {
        IReadOnlyList<XboxLiveUser> LocalUsers { get; }

        void AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel = SocialManagerExtraDetailLevel.NoExtraDetail);

        void RemoveLocalUser(XboxLiveUser user);

        XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, IReadOnlyList<string> xboxUserIdList);

        XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter);

        IReadOnlyList<SocialEvent> DoWork();

        void UpdateSocialUserGroup(XboxSocialUserGroup group, IReadOnlyList<string> users);

        void DestroySocialUserGroup(XboxSocialUserGroup group);

        void SetRichPresencePollingStatus(XboxLiveUser user, bool shouldEnablePolling);
    }
}