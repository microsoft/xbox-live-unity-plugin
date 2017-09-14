// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;

    public class SocialUserGroupLoadedEventArgs : SocialEventArgs
    {
        internal SocialUserGroupLoadedEventArgs(XboxSocialUserGroup group)
        {
            SocialUserGroup = group;
        }

        public XboxSocialUserGroup SocialUserGroup { get; private set; }
    }
}