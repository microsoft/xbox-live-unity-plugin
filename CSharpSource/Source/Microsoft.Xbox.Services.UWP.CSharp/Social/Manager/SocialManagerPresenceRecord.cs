// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using Presence;

    public partial class SocialManagerPresenceRecord : ISocialManagerPresenceRecord, IEquatable<SocialManagerPresenceRecord>
    {
        [DllImport(XboxLive.FlatCDllName)]
        private static extern bool SocialManagerPresenceRecordIsUserPlayingTitle(IntPtr socialManagerPresenceRecord, uint titleId);

        public bool IsUserPlayingTitle(uint titleId)
        {
            return SocialManagerPresenceRecordIsUserPlayingTitle(m_socialManagerPresenceRecordPtr, titleId);
        }
    }
}
