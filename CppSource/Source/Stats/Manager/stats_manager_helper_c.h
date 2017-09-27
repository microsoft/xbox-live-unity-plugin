// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/stats_manager_c.h"
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
    // todo event args
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