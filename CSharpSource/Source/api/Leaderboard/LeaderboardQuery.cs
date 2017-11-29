// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xbox.Services.Leaderboard
{
    public partial class LeaderboardQuery
    {
        public string StatName { get; internal set; }
        public string SocialGroup { get; internal set; }
        public bool HasNext { get; internal set; }

        public uint SkipResultToRank
        {
            get
            {
                return pImpl.GetSkipResultToRank();
            }
            set
            {
                pImpl.SetSkipResultToRank(value);
            }
        }

        public bool SkipResultToMe
        {
            get
            {
                return pImpl.GetSkipResultToMe();
            }

            set
            {
                 pImpl.SetSkipResultToMe(value);
            }
        }

        public SortOrder Order
        {
            get
            {
                return pImpl.GetOrder();
            }

            set
            {
                pImpl.SetOrder(value);
            }
        }

        public uint MaxItems
        {
            get
            {
                return pImpl.GetMaxItems();
            }

            set
            {
                pImpl.SetMaxItems(value);
            }
        }

        ILeaderboardQueryImpl pImpl;
        internal IntPtr GetPtr() { return pImpl.GetPtr(); }
        
        internal LeaderboardQuery(IntPtr ptr)
        {
            pImpl = new LeaderboardQueryImpl(ptr, this);
        }

        // todo remove after removing leaderboard service
        internal LeaderboardQuery(LeaderboardQuery query, string continuation)
        {

        }
    }
}