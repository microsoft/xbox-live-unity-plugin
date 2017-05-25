// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social
{
    using global::System;

    public class XboxUserProfile
    {
        public string XboxUserId { get; set; }

        public string Gamertag { get; set; }

        public string Gamerscore { get; set; }

        public Uri GameDisplayPictureResizeUri { get; set; }

        public string GameDisplayName { get; set; }

        public Uri ApplicationDisplayPictureResizeUri { get; set; }

        public string ApplicationDisplayName { get; set; }
    }
}