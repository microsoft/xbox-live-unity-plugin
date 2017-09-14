// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Presence;
    public partial class SocialManagerPresenceTitleRecord
    {
        internal SocialManagerPresenceTitleRecord(IntPtr titleRecordPtr)
        {
            SocialManager.SocialManagerPresenceTitleRecord_c cTitleRecord = Marshal.PtrToStructure<SocialManager.SocialManagerPresenceTitleRecord_c>(titleRecordPtr);
            IsTitleActive = Convert.ToBoolean(cTitleRecord.IsTitleActive);
            IsBroadcasting = Convert.ToBoolean(cTitleRecord.IsBroadcasting);
            Device = cTitleRecord.DeviceType;
            TitleId = cTitleRecord.TitleId;
            PresenceText = cTitleRecord.PresenceText;
        }
    }
}
