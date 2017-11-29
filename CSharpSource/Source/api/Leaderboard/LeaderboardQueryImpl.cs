// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Runtime.InteropServices;

    class LeaderboardQueryImpl : ILeaderboardQueryImpl
    {
        IntPtr m_leaderboardQueryPtr;
        public IntPtr GetPtr() { return m_leaderboardQueryPtr; }
        
        // MaxItems
        uint m_maxItems;
        public uint GetMaxItems()
        {
            return m_maxItems;
        }
        public void SetMaxItems(uint maxItems)
        {
            m_maxItems = maxItems;
            LeaderboardQuerySetMaxItems(m_leaderboardQueryPtr, m_maxItems);
        }
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetMaxItems(IntPtr leaderboardQuery, UInt32 maxItems);

        // Order
        SortOrder m_order;
        public SortOrder GetOrder()
        {
            return m_order;
        }
        public void SetOrder(SortOrder order)
        {
            m_order = order;
            LeaderboardQuerySetOrder(m_leaderboardQueryPtr, m_order);
        }
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetOrder(IntPtr leaderboardQuery, SortOrder order);

        // SkipResultToMe
        bool m_skipResultToMe;
        public bool GetSkipResultToMe()
        {
            return m_skipResultToMe;
        }
        public void SetSkipResultToMe(bool skipResultToMe)
        {
            m_skipResultToMe = skipResultToMe;
            LeaderboardQuerySetSkipResultToMe(m_leaderboardQueryPtr, m_skipResultToMe);
        }
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetSkipResultToMe(IntPtr leaderboardQuery, bool skipResultToMe);

        // SkipResultToRank
        uint m_skipResultToRank;
        public uint GetSkipResultToRank()
        {
            return m_skipResultToRank;
        }
        public void SetSkipResultToRank(uint skipResultToRank)
        {
            m_skipResultToRank = skipResultToRank;
            LeaderboardQuerySetSkipResultToRank(m_leaderboardQueryPtr, skipResultToRank);
        }
        [DllImport(XboxLive.FlatCDllName)]
        private static extern void LeaderboardQuerySetSkipResultToRank(IntPtr leaderboardQuery, UInt32 setSkipResultToRank);
        
        public LeaderboardQueryImpl(IntPtr ptr, LeaderboardQuery query)
        {
            m_leaderboardQueryPtr = ptr;

            LEADERBOARD_QUERY cLeaderboardQuery = (LEADERBOARD_QUERY)Marshal.PtrToStructure(m_leaderboardQueryPtr, typeof(LEADERBOARD_QUERY));
            m_skipResultToMe = cLeaderboardQuery.SkipResultToMe;
            m_skipResultToRank = cLeaderboardQuery.SkipResultToRank;
            m_maxItems = cLeaderboardQuery.MaxItems;
            m_order = cLeaderboardQuery.Order;
            query.StatName = MarshalingHelpers.Utf8ToString(cLeaderboardQuery.StatName);
            query.SocialGroup = MarshalingHelpers.Utf8ToString(cLeaderboardQuery.SocialGroup);
            query.HasNext = cLeaderboardQuery.HasNext;
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
