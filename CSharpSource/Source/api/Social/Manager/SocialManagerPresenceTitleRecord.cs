// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Presence;
    using static SocialManager;

    public class SocialManagerPresenceTitleRecord : IEquatable<SocialManagerPresenceTitleRecord>
    {
        public SocialManagerPresenceTitleRecord()
        {
        }

        public SocialManagerPresenceTitleRecord(PresenceDeviceType device, PresenceTitleRecord titleRecord)
        {
            this.Device = device;
            this.TitleId = titleRecord.TitleId;
            this.IsBroadcasting = titleRecord.BroadcastRecord.StartTime != DateTimeOffset.MinValue;
            this.IsTitleActive = titleRecord.IsTitleActive;
            this.PresenceText = titleRecord.Presence;
        }

        internal SocialManagerPresenceTitleRecord(IntPtr titleRecordPtr)
        {
            SocialManagerPresenceTitleRecord_c cTitleRecord = Marshal.PtrToStructure<SocialManagerPresenceTitleRecord_c>(titleRecordPtr);
            IsTitleActive = Convert.ToBoolean(cTitleRecord.IsTitleActive);
            IsBroadcasting = Convert.ToBoolean(cTitleRecord.IsBroadcasting);
            Device = cTitleRecord.DeviceType;
            TitleId = cTitleRecord.TitleId;
            PresenceText = cTitleRecord.PresenceText;
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