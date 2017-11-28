// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;
    using Presence;

    public partial class SocialManagerPresenceRecord : ISocialManagerPresenceRecord
    {
        public SocialManagerPresenceRecord()
        {
            m_titleRecords = new List<SocialManagerPresenceTitleRecord>();
        }

        public bool IsUserPlayingTitle(uint titleId)
        {
            return false;
        }
    }
}
