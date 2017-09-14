// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using Presence;

    public partial class SocialManagerPresenceRecord
    {
        IntPtr m_socialManagerPresenceRecordPtr;
        List<SocialManagerPresenceTitleRecord> m_titleRecords;

        public UserPresenceState UserState { get; private set; }
        public IList<SocialManagerPresenceTitleRecord> PresenceTitleRecords
        {
            get
            {
                return m_titleRecords.AsReadOnly();
            }
        }
    }
}