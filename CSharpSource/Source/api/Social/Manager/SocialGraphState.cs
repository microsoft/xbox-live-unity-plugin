// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;

    internal enum SocialGraphState
    {
        Normal,
        Diff,
        EventProcessing,
        Refresh
    }

    [Flags]
    internal enum ChangeListType
    {
        NoChange = 0x0,
        ProfileChange = 0x1,
        PresenceChange = 0x2,
        SocialRelationshipChange = 0x4,
        Change = 0x8
    }

    internal enum InternalSocialEventType
    {
        Unknown,
        UsersAdded,
        UsersChanged,
        UsersRemoved,
        PresenceChanged,
        DevicePresenceChanged,
        TitlePresenceChanged,
        ProfilesChanged,
        SocialRelationshipsChanged,
    }

    internal enum EventState
    {
        Read,
        ReadyToRead,
        Clear
    }
}