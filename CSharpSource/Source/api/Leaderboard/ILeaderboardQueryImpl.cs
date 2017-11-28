using Microsoft.Xbox.Services.Leaderboard;
using System;

namespace Microsoft.Xbox.Services.Leaderboard
{
    internal interface ILeaderboardQueryImpl
    {
        IntPtr GetPtr();
        
        uint GetSkipResultToRank();
        void SetSkipResultToRank(uint skipResultToRank);

        bool GetSkipResultToMe();
        void SetSkipResultToMe(bool skipResultToMe);

        SortOrder GetOrder();
        void SetOrder(SortOrder order);

        uint GetMaxItems();
        void SetMaxItems(uint maxItems);
    }
}
