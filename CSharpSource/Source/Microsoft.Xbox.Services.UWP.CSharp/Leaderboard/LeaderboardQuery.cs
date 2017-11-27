// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class LeaderboardQuery : ILeaderboardQuery
    {
        IntPtr m_leaderboardQueryPtr;
        internal IntPtr GetPtr() { return m_leaderboardQueryPtr; }

        /// <summary>
        /// Create a new query
        /// </summary>
        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr LeaderboardQueryCreate();
        public LeaderboardQuery()
        {
            m_leaderboardQueryPtr = LeaderboardQueryCreate();
        }

        internal LeaderboardQuery(IntPtr leaderboardQueryPtr)
        {
            m_leaderboardQueryPtr = leaderboardQueryPtr;
            LEADERBOARD_QUERY cLeaderboardQuery = Marshal.PtrToStructure<LEADERBOARD_QUERY>(leaderboardQueryPtr);
            SkipResultToMe = cLeaderboardQuery.SkipResultToMe;
            SkipResultToRank = cLeaderboardQuery.SkipResultToRank;
            MaxItems = cLeaderboardQuery.MaxItems;
            Order = cLeaderboardQuery.Order;
            StatName = MarshalingHelpers.Utf8ToString(cLeaderboardQuery.StatName);
            SocialGroup = MarshalingHelpers.Utf8ToString(cLeaderboardQuery.SocialGroup);
            HasNext = cLeaderboardQuery.HasNext;
        }


        uint m_maxItems;
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetMaxItems(IntPtr leaderboardQuery, UInt32 maxItems);
        public uint MaxItems
        {
            get
            {
                return m_maxItems;
            }

            set
            {
                m_maxItems = value;
                LeaderboardQuerySetMaxItems(m_leaderboardQueryPtr, m_maxItems);
            }
        }

        SortOrder m_order;
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetOrder(IntPtr leaderboardQuery, SortOrder order);
        public SortOrder Order
        {
            get
            {
                return m_order;
            }

            set
            {
                m_order = value;
                LeaderboardQuerySetOrder(m_leaderboardQueryPtr, m_order);
            }
        }

        bool m_skipResultToMe;
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetSkipResultToMe(IntPtr leaderboardQuery, bool skipResultToMe);
        public bool SkipResultToMe
        {
            get
            {
                return m_skipResultToMe;
            }

            set
            {
                m_skipResultToMe = value;
                LeaderboardQuerySetSkipResultToMe(m_leaderboardQueryPtr, m_skipResultToMe);
            }
        }

        uint m_skipResultToRank;
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetSkipResultToRank(IntPtr leaderboardQuery, UInt32 setSkipResultToRank);
        public uint SkipResultToRank
        {
            get
            {
                return m_skipResultToRank;
            }

            set
            {
                m_skipResultToRank = value;
                LeaderboardQuerySetSkipResultToRank(m_leaderboardQueryPtr, m_skipResultToRank);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LEADERBOARD_QUERY
        {
            [MarshalAs(UnmanagedType.Bool)]
            public bool SkipResultToMe;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 SkipResultToRank;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 MaxItems;

            [MarshalAs(UnmanagedType.U4)]
            public SortOrder Order;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr StatName;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr SocialGroup;

            [MarshalAs(UnmanagedType.Bool)]
            public bool HasNext;
        }
    }
}
