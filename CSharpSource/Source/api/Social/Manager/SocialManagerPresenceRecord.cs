// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
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

        internal SocialManagerPresenceRecord(IntPtr socialManagerPresenceRecordPtr)
        {
            this.m_socialManagerPresenceRecordPtr = socialManagerPresenceRecordPtr;

            SOCIAL_MANAGER_PRESENCE_RECORD cPresenceRecord = MarshalingHelpers.PtrToStructure<SOCIAL_MANAGER_PRESENCE_RECORD>(socialManagerPresenceRecordPtr);
            UserState = cPresenceRecord.UserState;

            m_titleRecords = new List<SocialManagerPresenceTitleRecord>();
            if (cPresenceRecord.PresenceTitleRecordsCount > 0)
            {
                IntPtr[] cTitleRecords = new IntPtr[cPresenceRecord.PresenceTitleRecordsCount];
                Marshal.Copy(cPresenceRecord.PresenceTitleRecords, cTitleRecords, 0, (int)cPresenceRecord.PresenceTitleRecordsCount);
                foreach (IntPtr cTitleRecord in cTitleRecords)
                {
                    m_titleRecords.Add(new SocialManagerPresenceTitleRecord(cTitleRecord));
                }
            }
        }

        public bool Equals(SocialManagerPresenceRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            bool listsAreEqual = PresenceTitleRecords.Count == other.PresenceTitleRecords.Count;
            for (int i = 0; i < PresenceTitleRecords.Count && listsAreEqual; i++)
            {
                listsAreEqual = listsAreEqual && PresenceTitleRecords[i].Equals(other.PresenceTitleRecords[i]);
            }

            return listsAreEqual && this.UserState == other.UserState;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((SocialManagerPresenceRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = UserState.GetHashCode();
                hashCode = (hashCode * 397) ^ (m_titleRecords.GetHashCode());
                return hashCode;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SOCIAL_MANAGER_PRESENCE_RECORD
        {
            [MarshalAs(UnmanagedType.U4)]
            public UserPresenceState UserState;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr PresenceTitleRecords;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 PresenceTitleRecordsCount;
        }

        // Used for mock services
        internal SocialManagerPresenceRecord(UserPresenceState state, IList<SocialManagerPresenceTitleRecord> records)
        {
            UserState = state;
            m_titleRecords = records == null ? new List<SocialManagerPresenceTitleRecord>() : (List<SocialManagerPresenceTitleRecord>)records;
        }
    }
}