// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Presence;

    public class SocialManagerPresenceTitleRecord : IEquatable<SocialManagerPresenceTitleRecord>
    {
        public SocialManagerPresenceTitleRecord()
        {
        }

        internal SocialManagerPresenceTitleRecord(IntPtr titleRecordPtr)
        {
            SOCIAL_MANAGER_PRESENCE_TITLE_RECORD cTitleRecord = (SOCIAL_MANAGER_PRESENCE_TITLE_RECORD)Marshal.PtrToStructure(titleRecordPtr, typeof(SOCIAL_MANAGER_PRESENCE_TITLE_RECORD));
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

        public uint TitleId { get; set; }

        public string PresenceText { get; set; }
        
        public bool IsBroadcasting { get; set; }

        public PresenceDeviceType Device { get; set; }
        
        public bool IsTitleActive { get; set; }

        public bool Equals(SocialManagerPresenceTitleRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.TitleId == other.TitleId
                   && this.IsTitleActive == other.IsTitleActive
                   && string.Equals(this.PresenceText, other.PresenceText)
                   && this.IsBroadcasting == other.IsBroadcasting
                   && this.Device == other.Device;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((SocialManagerPresenceTitleRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)this.TitleId;
                hashCode = (hashCode * 397) ^ this.IsTitleActive.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.PresenceText != null ? this.PresenceText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.IsBroadcasting.GetHashCode();
                return hashCode;
            }
        }
    }
}