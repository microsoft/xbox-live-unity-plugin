// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "user_impl_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

StatValue *CreateStatValueFromCpp(
    _In_ stat_value cppStatValue
)
{
    auto cStatValue = new StatValue();

    cStatValue->name = cppStatValue.name().c_str();
    cStatValue->asNumber = cppStatValue.as_number();
    cStatValue->asInteger = cppStatValue.as_integer();
    cStatValue->asString = cppStatValue.as_string().c_str();
    cStatValue->dataType = static_cast<STAT_DATA_TYPE>(cppStatValue.data_type());

    return cStatValue;
}

StatEvent *CreateStatEventFromCpp(
    _In_ stat_event cppEvent
) 
{
    auto cEvent = new StatEvent();

    // todo cppEvent.error_info();
    // todo cppEvent.event_args();
    
    cEvent->eventType = static_cast<STAT_EVENT_TYPE>(cppEvent.event_type());

    auto localUser = new XboxLiveUser();
    localUser->pImpl = new XboxLiveUserImpl(cppEvent.local_user()->windows_system_user(), localUser);
    cEvent->localUser = localUser;

    return cEvent;
}