// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "leaderboard_helper_c.h"

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToMe(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ bool skipResultToMe
)
{
    VerifyGlobalXsapiInit();

    leaderboardQuery->pImpl->m_cppQuery.set_skip_result_to_me(skipResultToMe);
}
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToRank(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 skipResultToRank
)
{
    VerifyGlobalXsapiInit();

    leaderboardQuery->pImpl->m_cppQuery.set_skip_result_to_rank(skipResultToRank);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetMaxItems(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 maxItems
)
{
    VerifyGlobalXsapiInit();

    leaderboardQuery->pImpl->m_cppQuery.set_max_items(maxItems);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetOrder(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ SORT_ORDER order
)
{
    VerifyGlobalXsapiInit();

    leaderboardQuery->pImpl->m_cppQuery.set_order(static_cast<sort_order>(order));
}