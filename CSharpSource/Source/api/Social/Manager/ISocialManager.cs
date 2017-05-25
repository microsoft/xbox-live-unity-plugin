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
        IList<XboxLiveUser> LocalUsers { get; }

        Task AddLocalUser(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel = SocialManagerExtraDetailLevel.None);

        void RemoveLocalUser(XboxLiveUser user);

        XboxSocialUserGroup CreateSocialUserGroupFromList(XboxLiveUser user, List<ulong> userIds);

        XboxSocialUserGroup CreateSocialUserGroupFromFilters(XboxLiveUser user, PresenceFilter presenceFilter, RelationshipFilter relationshipFilter);

        IList<SocialEvent> DoWork();
        
    }
}