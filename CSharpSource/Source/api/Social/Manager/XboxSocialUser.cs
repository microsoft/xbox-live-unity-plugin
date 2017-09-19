// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{    
    public partial class XboxSocialUser
    {
        public string XboxUserId { get; set; }

        public string DisplayName { get; set; }

        public string RealName { get; set; }

        public string DisplayPicRaw { get; set; }

        public bool UseAvatar { get; set; }

        public string Gamertag { get; set; }

        public string Gamerscore { get; set; }

        public PreferredColor PreferredColor { get; set; }

        public bool IsFollowedByCaller { get; set; }

        public bool IsFollowingUser { get; set; }

        public bool IsFavorite { get; set; }

        public SocialManagerPresenceRecord PresenceRecord { get; set; }

        public TitleHistory TitleHistory { get; set; }

        // Used for mock services
        internal XboxSocialUser()
        {

        }
    }
}