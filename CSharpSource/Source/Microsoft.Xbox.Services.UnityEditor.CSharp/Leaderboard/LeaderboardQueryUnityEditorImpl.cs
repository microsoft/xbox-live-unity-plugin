
using System;

namespace Microsoft.Xbox.Services.Leaderboard
{
    class LeaderboardQueryUnityEditorImpl : ILeaderboardQueryImpl
    {
        public IntPtr GetPtr() { return IntPtr.Zero; }
        
        uint m_maxItems;
        public uint GetMaxItems()
        {
            return m_maxItems;
        }
        public void SetMaxItems(uint maxItems)
        {
            m_maxItems = maxItems;
        }

        SortOrder m_order;
        public SortOrder GetOrder()
        {
            return m_order;
        }
        public void SetOrder(SortOrder order)
        {
            m_order = order;
        }

        bool m_skipResultToMe;
        public bool GetSkipResultToMe()
        {
            return m_skipResultToMe;
        }
        public void SetSkipResultToMe(bool skipResultToMe)
        {
            m_skipResultToMe = skipResultToMe;
        }

        uint m_skipResultToRank;
        public uint GetSkipResultToRank()
        {
            return m_skipResultToRank;
        }
        public void SetSkipResultToRank(uint skipResultToRank)
        {
            m_skipResultToRank = skipResultToRank;
        }
    }
}
