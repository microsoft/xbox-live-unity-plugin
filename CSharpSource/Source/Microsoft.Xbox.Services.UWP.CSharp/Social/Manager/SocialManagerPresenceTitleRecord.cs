// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Presence;
    public partial class SocialManagerPresenceTitleRecord
    {
        internal SocialManagerPresenceTitleRecord(IntPtr titleRecordPtr)
        {
            SOCIAL_MANAGER_PRESENCE_TITLE_RECORD cTitleRecord = Marshal.PtrToStructure<SOCIAL_MANAGER_PRESENCE_TITLE_RECORD>(titleRecordPtr);
            IsTitleActive = cTitleRecord.IsTitleActive;
            IsBroadcasting = cTitleRecord.IsBroadcasting;
            Device = cTitleRecord.DeviceType;
            TitleId = cTitleRecord.TitleId;
            PresenceText = MarshalingHelpers.Utf8ToString(cTitleRecord.PresenceText);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SOCIAL_MANAGER_PRESENCE_TITLE_RECORD
        {

            [MarshalAs(UnmanagedType.U1)]
            public bool IsTitleActive;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsBroadcasting;

            [MarshalAs(UnmanagedType.U4)]
            public PresenceDeviceType DeviceType;

            [MarshalAs(UnmanagedType.U4)]
            public uint TitleId;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PresenceText;
        }
    }
}
