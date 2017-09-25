// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "../Leaderboard/leaderboard_helper_c.h"
#include "stats_manager_helper_c.h"
#include "user_impl_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerAddLocalUser(
    _In_ XboxLiveUser* user,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->add_local_user(user->pImpl->m_cppUser);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerRemoveLocalUser(
    _In_ XboxLiveUser* user,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->remove_local_user(user->pImpl->m_cppUser);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerRequestFlushToService(
    _In_ XboxLiveUser* user,
    _In_ bool isHighPriority,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->request_flush_to_service(user->pImpl->m_cppUser, isHighPriority);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

std::vector<StatEvent *> cEvents;
XSAPI_DLLEXPORT StatEvent** XBL_CALLING_CONV
StatsManagerDoWork(
    _Inout_ int32 *numOfEvents
)
{
    VerifyGlobalXsapiInit();

    auto cppEvents = stats_manager::get_singleton_instance()->do_work();

    cEvents.clear();
    for (auto cppEvent : cppEvents) {
        cEvents.push_back(CreateStatEventFromCpp(cppEvent));
    }
    *numOfEvents = cEvents.size();

    return cEvents.data();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatAsNumber(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ double statValue,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->set_stat_as_number(user->pImpl->m_cppUser, statName, statValue);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatAsInteger(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ int64 statValue,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->set_stat_as_integer(user->pImpl->m_cppUser, statName, statValue);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatAsString(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ PCSTR_T statValue,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->set_stat_as_string(user->pImpl->m_cppUser, statName, statValue);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

// todo move
std::vector<utility::string_t> cppStatNameList;
std::vector<PCSTR_T> cStatNameList;
XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetStatNames(
    _In_ XboxLiveUser* user,
    _Inout_ PCSTR_T** statNameList,
    _Inout_ int32* statNameListSize,
    _Inout_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    cppStatNameList.clear();
    auto result = stats_manager::get_singleton_instance()->get_stat_names(user->pImpl->m_cppUser, cppStatNameList);
    
    cStatNameList.clear();
    for (size_t i = 0; i < cppStatNameList.size(); i++)
    {
        cStatNameList.push_back(cppStatNameList[i].c_str());
    }

    *statNameList = cStatNameList.data();
    *statNameListSize = cStatNameList.size();

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _Out_ StatValue** statValue,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->get_stat(user->pImpl->m_cppUser, statName);
    *statValue = CreateStatValueFromCpp(result.payload());

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerDeleteStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->delete_stat(user->pImpl->m_cppUser, statName);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

// todo _XSAPIIMP stats_manager();

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ LeaderboardQuery* query,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->get_leaderboard(user->pImpl->m_cppUser, statName, query->pImpl->m_cppQuery);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetSocialLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR_T statName,
    _In_ PCSTR_T socialGroup,
    _In_ LeaderboardQuery* query,
    _Out_ PCSTR_T* errMessage
)
{
    VerifyGlobalXsapiInit();

    auto result = stats_manager::get_singleton_instance()->get_social_leaderboard(user->pImpl->m_cppUser, statName, socialGroup, query->pImpl->m_cppQuery);

    *errMessage = std::wstring(result.err_message().begin(), result.err_message().end()).data();
    return result.err().value();
}