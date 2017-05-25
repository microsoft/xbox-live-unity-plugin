// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    public enum SocialEventType
    {
        Unknown = 0,
        UsersAddedToSocialGraph,
        UsersRemovedFromSocialGraph,
        PresenceChanged,
        ProfilesChanged,
        SocialRelationshipsChanged,
        LocalUserAdded,
        LocalUserRemoved,
        SocialUserGroupLoaded,
        SocialUserGroupUpdated,
    }
}