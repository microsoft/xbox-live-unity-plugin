// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "types_c.h"
#include "xsapi/errors_c.h"
#include "xsapi/leaderboard_c.h"
#include "xsapi/system_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

#if !XDK_API
struct StatEventImpl;
struct StatValueImpl;

typedef enum STAT_DATA_TYPE {
    UNDEFINED,
    NUMBER,
    STRING
} STAT_DATA_TYPE;

typedef enum STAT_EVENT_TYPE {
    LOCAL_USER_ADDED_STAT,
    LOCAL_USER_REMOVED_STAT,
    STAT_UPDATE_COMPLETE_STAT,
    GET_LEADERBOARD_COMPLETE_STAT
} STAT_EVENT_TYPE;

typedef struct StatValue
{
    PCSTR name;
    double asNumber;
    int64 asInteger;
    PCSTR asString;
    STAT_DATA_TYPE dataType;

    StatValueImpl* pImpl;
} StatValue;

typedef struct StatEventArgs
{
} StatEventArgs;

// todo implement
typedef struct LeaderboardResultEventArgs : StatEventArgs
{
    LeaderboardResult* result;
} LeaderboardResultEventArgs;

typedef struct StatEvent
{
    // todo eventArgs
    STAT_EVENT_TYPE eventType;
    StatEventArgs* eventArgs;
    XboxLiveUser* localUser;
    int32 errorCode;
    PCSTR errorMessage;

    StatEventImpl* pImpl;
} StatEvent;

typedef struct StatsManager
{
} StatsManager;

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerAddLocalUser(
    _In_ XboxLiveUser* user,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerRemoveLocalUser(
    _In_ XboxLiveUser* user,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerRequestFlushToService(
    _In_ XboxLiveUser* user,
    _In_ bool isHighPriority,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT StatEvent** XBL_CALLING_CONV
StatsManagerDoWork(
    _Inout_ int32 *numOfEvents
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatisticNumberData(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ double statValue,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatisticIntegerData(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ int64 statValue,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatisticStringData(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ PCSTR statValue,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetStatNames(
    _In_ XboxLiveUser* user,
    _Inout_ PCSTR** statNameList,
    _Inout_ int32* statNameListSize,
    _Inout_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _Out_ StatValue** statValue,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerDeleteStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _Out_ PCSTR* errMessage
);

// todo _XSAPIIMP stats_manager();

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ LeaderboardQuery* query,
    _Out_ PCSTR* errMessage
);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetSocialLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ PCSTR socialGroup,
    _In_ LeaderboardQuery* query,
    _Out_ PCSTR* errMessage
);
#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)