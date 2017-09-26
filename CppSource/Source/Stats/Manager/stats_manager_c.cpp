// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "../Leaderboard/leaderboard_helper_c.h"
#include "user_impl.h"
#include "stats_manager_helper_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerAddLocalUser(
    _In_ XboxLiveUser* user,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->add_local_user(user->pImpl->m_cppUser);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerRemoveLocalUser(
    _In_ XboxLiveUser* user,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->remove_local_user(user->pImpl->m_cppUser);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerRequestFlushToService(
    _In_ XboxLiveUser* user,
    _In_ bool isHighPriority,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->request_flush_to_service(user->pImpl->m_cppUser, isHighPriority);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

std::vector<StatEvent *> cEvents;
XSAPI_DLLEXPORT StatEvent** XBL_CALLING_CONV
StatsManagerDoWork(
    _Inout_ int32 *numOfEvents
)
{
    verify_global_init();

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
    _In_ PCSTR statName,
    _In_ double statValue,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->set_stat_as_number(user->pImpl->m_cppUser, utils::to_utf16string(statName), statValue);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatAsInteger(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ int64 statValue,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->set_stat_as_integer(user->pImpl->m_cppUser, utils::to_utf16string(statName), statValue);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerSetStatAsString(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ PCSTR statValue,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->set_stat_as_string(user->pImpl->m_cppUser, utils::to_utf16string(statName), utils::to_utf16string(statValue));

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

// todo move
std::vector<utility::string_t> cppStatNameList;
std::vector<std::string> cStatNameStringList;
std::vector<PCSTR> cStatNameCharList;
XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetStatNames(
    _In_ XboxLiveUser* user,
    _Inout_ PCSTR** statNameList,
    _Inout_ int32* statNameListSize,
    _Inout_ PCSTR* errMessage
)
{
    verify_global_init();

    cppStatNameList.clear();
    auto result = stats_manager::get_singleton_instance()->get_stat_names(user->pImpl->m_cppUser, cppStatNameList);
    
    cStatNameStringList.resize(cppStatNameList.size());
    cStatNameCharList.clear();
    const char* lastcstr;
    const char* lastdata;
    for (size_t i = 0; i < cppStatNameList.size(); i++)
    {
        auto name = utils::to_utf8string(cppStatNameList.at(i));
        cStatNameStringList[i] = name;
        cStatNameCharList.push_back(cStatNameStringList[i].c_str());
    }

    *statNameList = cStatNameCharList.data();
    *statNameListSize = cStatNameCharList.size();

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _Out_ StatValue** statValue,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->get_stat(user->pImpl->m_cppUser, utils::to_utf16string(statName));
    *statValue = CreateStatValueFromCpp(result.payload());

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerDeleteStat(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->delete_stat(user->pImpl->m_cppUser, utils::to_utf16string(statName));

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

// todo _XSAPIIMP stats_manager();

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ LeaderboardQuery* query,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->get_leaderboard(user->pImpl->m_cppUser, utils::to_utf16string(statName), query->pImpl->m_cppQuery);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
StatsManagerGetSocialLeaderboard(
    _In_ XboxLiveUser* user,
    _In_ PCSTR statName,
    _In_ PCSTR socialGroup,
    _In_ LeaderboardQuery* query,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    auto result = stats_manager::get_singleton_instance()->get_social_leaderboard(user->pImpl->m_cppUser, utils::to_utf16string(statName), utils::to_utf16string(socialGroup), query->pImpl->m_cppQuery);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}