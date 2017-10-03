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
struct LeaderboardColumnImpl;
struct LeaderboardRowImpl;
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
    PCSTR statName;
    LEADERBOARD_STAT_TYPE statType;

    LeaderboardColumnImpl* pImpl;
} LeaderboardColumn;

typedef struct LeaderboardRow
{
    PCSTR gamertag;
    PCSTR xboxUserId;
    double percentile;
    uint32 rank;
    PCSTR* columnValues;
    uint32 columnValuesSize;

    LeaderboardRowImpl* pImpl;
} LeaderboardRow;

typedef struct LeaderboardQuery
{
    bool skipResultToMe;
    uint32 skipResultToRank;
    uint32 maxItems;
    SORT_ORDER order;
    PCSTR statName;
    PCSTR socialGroup;
    bool hasNext;

    LeaderboardQueryImpl* pImpl;
} LeaderboardQuery;

XSAPI_DLLEXPORT LeaderboardQuery* XBL_CALLING_CONV
LeaderboardQueryCreate(
);
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
    uint32 columnsSize;
    LeaderboardRow** rows;
    uint32 rowsSize;

    LeaderboardResultImpl* pImpl;
} LeaderboardResult;

XSAPI_DLLEXPORT bool XBL_CALLING_CONV
LeaderboardResultHasNext(
    _In_ LeaderboardResult* leaderboardResult
);

#if !defined(XBOX_LIVE_CREATORS_SDK)
typedef struct GetNextResultPayload
{
    LeaderboardResult* nextResult;
} GetNextResultPayload;

typedef struct GetNextResult
{
    XboxLiveResult result;
    GetNextResultPayload payload;
} GetNextResult;

typedef void(*GetNextCompletionRoutine)(
    _In_ GetNextResult result,
    _In_opt_ void* context
    );

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
LeaderboardResultGetNext(
    _In_ LeaderboardResult* leaderboardResult,
    _In_ uint32 maxItems,
    _In_ GetNextCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
);
#endif

XSAPI_DLLEXPORT int XBL_CALLING_CONV
LeaderboardResultGetNextQuery(
    _In_ LeaderboardResult* leaderboardResult,
    _In_ uint32 maxItems,
    _Out_ LeaderboardQuery** nextQuery,
    _Out_ PCSTR* errMessage
);

typedef struct LeaderboardService
{
    // todo implement
} LeaderboardService;
#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)