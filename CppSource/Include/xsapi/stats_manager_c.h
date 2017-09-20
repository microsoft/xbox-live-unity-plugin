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

typedef enum STAT_DATA_TYPE {
    UNDEFINED,
    NUMBER,
    STRING
} STAT_DATA_TYPE;

typedef enum STAT_EVENT_TYPE {
    LOCAL_USER_ADDED,
    LOCAL_USER_REMOVED,
    STAT_UPDATE_COMPLETE,
    GET_LEADERBOARD_COMPLETE
} STAT_EVENT_TYPE;

typedef struct StatValue
{
    PCSTR_T name;
    double asNumber;
    int64 asInteger;
    PCSTR_T asString;
    STAT_DATA_TYPE dataType;
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
    // todo error_info
    STAT_EVENT_TYPE eventType;
    StatEventArgs* eventArgs;
    XboxLiveUser* localUser;
} StatEvent;

typedef struct StatsManager
{
} StatsManager;

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerAddLocalUser(
    _In_ XboxLiveUser* user
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerRemoveLocalUser(
    _In_ XboxLiveUser* user
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerRequestFlushToService(
    _In_ XboxLiveUser* user,
    _In_ bool isHighPriority
);

XSAPI_DLLEXPORT StatEvent** XBL_CALLING_CONV
StatsManagerDoWork(
    _Inout_ int64 numOfEvents
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerSetStatAsNumber(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ double statValue
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerSetStatAsInteger(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ int64 statValue
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerSetStatAsString(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ PCSTR_T statValue
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerGetStatNames(
    _In_ XboxLiveUser* user,
    _Inout_ PCSTR_T* statNameList
);

XSAPI_DLLEXPORT StatValue* XBL_CALLING_CONV
StatsManagerGetStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerDeleteStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName
);

// todo _XSAPIIMP stats_manager();

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerGetLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ LeaderboardQuery* query
);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerGetSocialLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ PCSTR_T socialGroup,
    _In_ LeaderboardQuery* query
);
#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)