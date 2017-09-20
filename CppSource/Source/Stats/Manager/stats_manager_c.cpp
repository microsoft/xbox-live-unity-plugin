// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "../Leaderboard/leaderboard_helper_c.h"
#include "stats_manager_helper_c.h"
#include "user_impl_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerAddLocalUser(
    _In_ XboxLiveUser* user
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->add_local_user(user->pImpl->m_cppUser);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerRemoveLocalUser(
    _In_ XboxLiveUser* user
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->add_local_user(user->pImpl->m_cppUser);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerRequestFlushToService(
    _In_ XboxLiveUser* user,
    _In_ bool isHighPriority
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->request_flush_to_service(user->pImpl->m_cppUser, isHighPriority);
}

XSAPI_DLLEXPORT StatEvent** XBL_CALLING_CONV
StatsManagerDoWork(
    _Inout_ int64 *numOfEvents
)
{
    VerifyGlobalXsapiInit();


    auto cppEvents = stats_manager::get_singleton_instance()->do_work();
    *numOfEvents = cppEvents.size();

    auto cEvents = std::vector<StatEvent *>();
    for (auto cppEvent : cppEvents) {
        cEvents.push_back(CreateStatEventFromCpp(cppEvent));
    }

    return cEvents.data();
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerSetStatAsNumber(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ double statValue
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->set_stat_as_number(user->pImpl->m_cppUser, statName, statValue);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerSetStatAsInteger(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ int64 statValue
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->set_stat_as_integer(user->pImpl->m_cppUser, statName, statValue);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerSetStatAsString(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ PCSTR_T statValue
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->set_stat_as_string(user->pImpl->m_cppUser, statName, statValue);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerGetStatNames(
    _In_ XboxLiveUser* user,
    _Inout_ PCSTR_T* statNameList
)
{
    VerifyGlobalXsapiInit();

    auto cppStatNameList = std::vector<utility::string_t>();
    stats_manager::get_singleton_instance()->get_stat_names(user->pImpl->m_cppUser, cppStatNameList);
    
    auto cStatNameList = std::vector<PCSTR_T>();
    for (auto cppStatName : cppStatNameList) 
    {
        cStatNameList.push_back(cppStatName.c_str());
    }

    statNameList = cStatNameList.data();
}

XSAPI_DLLEXPORT StatValue* XBL_CALLING_CONV
StatsManagerGetStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName
)
{
    VerifyGlobalXsapiInit();

    auto cppStatValue = stats_manager::get_singleton_instance()->get_stat(user->pImpl->m_cppUser, statName);
    return CreateStatValueFromCpp(cppStatValue.payload());
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerDeleteStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->delete_stat(user->pImpl->m_cppUser, statName);
}

// todo _XSAPIIMP stats_manager();

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerGetLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ LeaderboardQuery* query
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->get_leaderboard(user->pImpl->m_cppUser, statName, query->pImpl->m_cppQuery);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
StatsManagerGetSocialLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ PCSTR_T socialGroup,
    _In_ LeaderboardQuery* query
)
{
    VerifyGlobalXsapiInit();

    stats_manager::get_singleton_instance()->get_social_leaderboard(user->pImpl->m_cppUser, statName, socialGroup, query->pImpl->m_cppQuery);
}