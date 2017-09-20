// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "types_c.h"
#include "xsapi/errors_c.h"
#include "xsapi/system_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

#if !XDK_API
struct LeaderboardQueryImpl;
struct LeaderboardResultImpl;

typedef enum LEADERBOARD_STAT_TYPE {
    STAT_UINT64,
    STAT_BOOLEAN,
    STAT_DOUBLE,
    STAT_STRING,
    STAT_DATETIME,
    STAT_OTHER
} LEADERBOARD_STAT_TYPE;

typedef enum SORT_ORDER {
    ASCENDING,
    DESCENDING
} SORT_ORDER;

typedef struct LeaderboardColumn
{
    PCSTR_T statName;
    LEADERBOARD_STAT_TYPE statType;
} LeaderboardColumn;

typedef struct LeaderboardRow
{
    PCSTR_T gamertag;
    PCSTR_T xboxUserId;
    double percentile;
    uint32 rank;
    PCSTR_T* columnValues;
} LeaderboardRow;

typedef struct LeaderboardQuery
{
    bool skipResultToMe;
    uint32 skipResultToRank;
    uint32 maxItems;
    SORT_ORDER order;
    PCSTR_T statName;
    PCSTR_T socialGroup;
    bool hasNext;

    LeaderboardQueryImpl* pImpl;
} LeaderboardQuery;

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToMe(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ bool skipResultToMe
);
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToRank(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 skipResultToRank
);
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetMaxItems(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 maxItems
);
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetOrder(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ SORT_ORDER order
);

typedef struct LeaderboardResult
{
    uint32 totalRowCount;
    LeaderboardColumn** columns;
    LeaderboardRow** rows;
    bool hasNext;

    LeaderboardResultImpl* pImpl;
} LeaderboardResult;

#if !defined(XBOX_LIVE_CREATORS_SDK)
XSAPI_DLLEXPORT LeaderboardResult XBL_CALLING_CONV
LeaderboardResultGetNext(
    _In_ uint32 maxItems
);
#endif
XSAPI_DLLEXPORT LeaderboardQuery XBL_CALLING_CONV
LeaderboardResultGetNextQuery(
    _In_ uint32 maxItems
);

typedef struct LeaderboardService
{
    // todo implement
} LeaderboardService;
#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)