// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "leaderboard_helper_c.h"

// todo move
std::vector<LeaderboardQuery *> m_queries;

XSAPI_DLLEXPORT LeaderboardQuery* XBL_CALLING_CONV
LeaderboardQueryCreate(
)
{
    verify_global_init();

    auto query = new LeaderboardQuery();
    query->pImpl = new LeaderboardQueryImpl(leaderboard_query(), query);
    m_queries.push_back(query);

    return query;
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToMe(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ bool skipResultToMe
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetSkipResultToMe(skipResultToMe);
}
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToRank(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 skipResultToRank
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetSkipResultToRank(skipResultToRank);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetMaxItems(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 maxItems
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetMaxItems(maxItems);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetOrder(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ SORT_ORDER order
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetOrder(order);
}