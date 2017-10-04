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
            SocialManagerPresenceTitleRecord_c cTitleRecord = Marshal.PtrToStructure<SocialManagerPresenceTitleRecord_c>(titleRecordPtr);
            IsTitleActive = Convert.ToBoolean(cTitleRecord.IsTitleActive);
            IsBroadcasting = Convert.ToBoolean(cTitleRecord.IsBroadcasting);
            Device = cTitleRecord.DeviceType;
            TitleId = cTitleRecord.TitleId;
            PresenceText = cTitleRecord.PresenceText;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialManagerPresenceTitleRecord_c
        {

            [MarshalAs(UnmanagedType.U1)]
            public byte IsTitleActive;

            [MarshalAs(UnmanagedType.U1)]
            public byte IsBroadcasting;

            [MarshalAs(UnmanagedType.U4)]
            public PresenceDeviceType DeviceType;

            [MarshalAs(UnmanagedType.U4)]
            public uint TitleId;

            [MarshalAs(UnmanagedType.LPStr)]
            public string PresenceText;
        }
    }
}
