// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/stats_manager_c.h"
#include "../Leaderboard/leaderboard_helper_c.h"
#include "user_impl.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

struct StatValueImpl
{
    StatValueImpl(
        _In_ stat_value creationContext,
        _In_ StatValue *cStatValue
    ) : m_cStatValue(cStatValue), m_cppStatValue(creationContext)
    {
        m_name = utils::to_utf8string(m_cppStatValue.name());
        m_cStatValue->name = m_name.c_str();

        m_asNumber = m_cppStatValue.as_number();
        m_cStatValue->asNumber = m_asNumber;

        m_asInteger = m_cppStatValue.as_integer();
        m_cStatValue->asInteger = m_cppStatValue.as_integer();

        m_asString = utils::to_utf8string(m_cppStatValue.as_string());
        m_cStatValue->asString = m_asString.c_str();

        m_dataType = static_cast<STAT_DATA_TYPE>(m_cppStatValue.data_type());
        m_cStatValue->dataType = m_dataType;
    }

    std::string m_name;
    double m_asNumber;
    int64 m_asInteger;
    std::string m_asString;
    STAT_DATA_TYPE m_dataType;

    stat_value m_cppStatValue;
    StatValue *m_cStatValue;
};

StatValue *CreateStatValueFromCpp(
    _In_ stat_value cppStatValue
)
{
    auto cStatValue = new StatValue();
    cStatValue->pImpl = new StatValueImpl(cppStatValue, cStatValue);

    return cStatValue;
}

struct LeaderboardResultEventArgsImpl {
    LeaderboardResultEventArgsImpl(
        _In_ std::shared_ptr<leaderboard_result_event_args> creationContext,
        _In_ LeaderboardResultEventArgs* cEventArgs
    ) : m_cEventArgs(cEventArgs), m_cppEventArgs(creationContext)
    {
        auto result = m_cppEventArgs->result();
        
        m_result = CreateLeaderboardResultFromCpp(result.payload());
        m_cEventArgs->result = m_result;
    }

    LeaderboardResult* m_result;

    std::shared_ptr<leaderboard_result_event_args> m_cppEventArgs;
    LeaderboardResultEventArgs* m_cEventArgs;
};

LeaderboardResultEventArgs* CreateLeaderboardResultEventArgs(
    _In_ std::shared_ptr<leaderboard_result_event_args> cppArgs
)
{
    auto cppResult = cppArgs->result();
    auto args = new LeaderboardResultEventArgs();
    args->pImpl = new LeaderboardResultEventArgsImpl(cppArgs, args);
    return args;
}

struct StatEventImpl 
{
    StatEventImpl(
        _In_ stat_event creationContext,
        _In_ StatEvent *cStatEvent
    ) : m_cStatEvent(cStatEvent), m_cppStatEvent(creationContext)
    {
        m_eventType = static_cast<STAT_EVENT_TYPE>(m_cppStatEvent.event_type());
        m_cStatEvent->eventType = m_eventType;

        // todo event args

        m_args = nullptr;
        if (m_cppStatEvent.event_args()) {
            try
            {
                auto cppEventArgs = std::dynamic_pointer_cast<leaderboard_result_event_args>(m_cppStatEvent.event_args());
                m_args = CreateLeaderboardResultEventArgs(cppEventArgs);
            }
            catch (const std::exception&)
            {
                // not leaderboard_result_event_args
            }
        }
        m_cStatEvent->eventArgs = m_args;

        m_localUser = new XboxLiveUser();
        m_localUser->pImpl = new XboxLiveUserImpl(m_cppStatEvent.local_user(), m_localUser);
        m_cStatEvent->localUser = m_localUser;

        m_errorInfo = m_cppStatEvent.error_info();

        m_errorCode = m_errorInfo.err();
        m_cStatEvent->errorCode = m_errorCode.value();

        m_errorMessage = m_errorInfo.err_message();
        m_cStatEvent->errorMessage = m_errorMessage.c_str();
    }

    STAT_EVENT_TYPE m_eventType;
    StatEventArgs *m_args;
    XboxLiveUser *m_localUser;
    xbox_live_result<void> m_errorInfo;
    std::error_code m_errorCode;
    std::string m_errorMessage;

    stat_event m_cppStatEvent;
    StatEvent *m_cStatEvent;
};

StatEvent *CreateStatEventFromCpp(
    _In_ stat_event cppEvent
) 
{
    auto cEvent = new StatEvent();
    cEvent->pImpl = new StatEventImpl(cppEvent, cEvent);

    return cEvent;
}