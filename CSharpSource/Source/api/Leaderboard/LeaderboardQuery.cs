// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Xbox.Services.Leaderboard
{
    public class LeaderboardQuery
    {
        IntPtr m_leaderboardQueryPtr;
        internal IntPtr GetPtr() { return m_leaderboardQueryPtr; }

        /// <summary>
        /// Create a new query
        /// </summary>

        private delegate IntPtr LeaderboardQueryCreate();
        public LeaderboardQuery()
        {
            m_leaderboardQueryPtr = XboxLive.Instance.Invoke<IntPtr, LeaderboardQueryCreate>();
        }

        /// <summary>
        /// Create a continuation query from an existing query combined with a continuation token.
        /// </summary>
        /// <param name="query">The query that this continuation query is based on.</param>
        /// <param name="continuationToken">The continuation token for the next request.</param>
        public LeaderboardQuery(LeaderboardQuery query, string continuationToken)
        {
            // todo needs XboxLive.Instance.Invoke<IntPtr, LeaderboardQueryCreate>();?

            this.StatName = query.StatName;
            this.SocialGroup = query.SocialGroup;
            this.MaxItems = query.MaxItems;
            this.Order = query.Order;
            this.SkipResultToRank = query.SkipResultToRank;
            this.SkipResultToMe = query.SkipResultToMe;
            this.ContinuationToken = continuationToken;
        }

        public string StatName { get; private set; }

        public string SocialGroup { get; private set; }

        uint m_maxItems;
        private delegate void LeaderboardQuerySetMaxItems(IntPtr leaderboardQuery, UInt32 maxItems);
        public uint MaxItems
        {
            get
            {
                return m_maxItems;
            }

            set
            {
                m_maxItems = value;
                XboxLive.Instance.Invoke<LeaderboardQuerySetMaxItems>(m_leaderboardQueryPtr, m_maxItems);
            }
        }

        SortOrder m_order;
        private delegate void LeaderboardQuerySetOrder(IntPtr leaderboardQuery, SortOrder order);
        public SortOrder Order
        {
            get
            {
                return m_order;
            }

            set
            {
                m_order = value;
                XboxLive.Instance.Invoke<LeaderboardQuerySetOrder>(m_leaderboardQueryPtr, m_order);
            }
        }

        bool m_skipResultToMe;
        private delegate void LeaderboardQuerySetSkipResultToMe(IntPtr leaderboardQuery, bool skipResultToMe);
        public bool SkipResultToMe
        {
            get
            {
                return m_skipResultToMe;
            }

            set
            {
                m_skipResultToMe = value;
                XboxLive.Instance.Invoke<LeaderboardQuerySetSkipResultToMe>(m_leaderboardQueryPtr, m_skipResultToMe);
            }
        }

        bool m_skipResultToRank;
        private delegate void LeaderboardQuerySetSkipResultToRank(IntPtr leaderboardQuery, UInt32 setSkipResultToRank);
        public bool SkipResultToRank
        {
            get
            {
                return m_skipResultToRank;
            }

            set
            {
                m_skipResultToRank = value;
                XboxLive.Instance.Invoke<LeaderboardQuerySetSkipResultToRank>(m_leaderboardQueryPtr, m_skipResultToRank);
            }
        }

        // todo update?
        internal string ContinuationToken { get; private set; }

        // todo update?
        public bool HasNext
        {
            get
            {
                return !string.IsNullOrEmpty(this.ContinuationToken);
            }
        }

        internal void Refresh()
        {

        }
    }
}