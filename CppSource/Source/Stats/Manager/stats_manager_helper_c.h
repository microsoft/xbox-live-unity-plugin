// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/stats_manager_c.h"
#include "../Leaderboard/leaderboard_helper_c.h"
#include "user_impl.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

struct XSAPI_STATS_MANAGER_VARS 
{
    public:
        xbox_live_result<void> cppVoidResult;
        xbox_live_result<stat_value> cppStatValueResult;

        std::vector<XSAPI_STAT_EVENT *> cEvents;

        std::vector<utility::string_t> cppStatNameList;
        std::vector<std::string> cStatNameStringList;
        std::vector<PCSTR> cStatNameCharList;
};

struct XSAPI_STAT_VALUE_IMPL
{
public:
    XSAPI_STAT_VALUE_IMPL(
        _In_ stat_value cppStatValue,
        _In_ XSAPI_STAT_VALUE *cStatValue
    );

    stat_value cppStatValue() const;

private:
    std::string m_name;
    std::string m_asString;

    stat_value m_cppStatValue;
    XSAPI_STAT_VALUE *m_cStatValue;
};

struct XSAPI_LEADERBOARD_RESULT_EVENT_ARGS_IMPL 
{
public:
    XSAPI_LEADERBOARD_RESULT_EVENT_ARGS_IMPL(
        _In_ std::shared_ptr<leaderboard_result_event_args> cppEventArgs,
        _In_ XSAPI_LEADERBOARD_RESULT_EVENT_ARGS* cEventArgs
    );

    std::shared_ptr<leaderboard_result_event_args> cppEventArgs() const;

private:
    XSAPI_LEADERBOARD_RESULT* m_result;

    std::shared_ptr<leaderboard_result_event_args> m_cppEventArgs;
    XSAPI_LEADERBOARD_RESULT_EVENT_ARGS* m_cEventArgs;
};

struct XSAPI_STAT_EVENT_IMPL 
{
public:
    XSAPI_STAT_EVENT_IMPL(
        _In_ stat_event cppStatEvent,
        _In_ XSAPI_STAT_EVENT *cStatEvent
    );

    stat_event cppStatEvent() const;

private:
    XSAPI_STAT_EVENT_TYPE m_eventType;
    XSAPI_STAT_EVENT_ARGS *m_args;
    XSAPI_XBOX_LIVE_USER *m_localUser;
    xbox_live_result<void> m_errorInfo;
    std::error_code m_errorCode;
    std::string m_errorMessage;

    stat_event m_cppStatEvent;
    XSAPI_STAT_EVENT *m_cStatEvent;
};

XSAPI_STAT_VALUE *CreateStatValueFromCpp(
    _In_ stat_value cppStatValue
);

XSAPI_LEADERBOARD_RESULT_EVENT_ARGS* CreateLeaderboardResultEventArgs(
    _In_ std::shared_ptr<leaderboard_result_event_args> cppArgs
);

XSAPI_STAT_EVENT *CreateStatEventFromCpp(
    _In_ stat_event cppEvent
);