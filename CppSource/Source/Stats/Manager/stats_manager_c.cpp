// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "../Leaderboard/leaderboard_helper_c.h"
#include "user_impl.h"
#include "stats_manager_helper_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::stats::manager;

// todo - move global variabes into a pimpl and store it in the xsapi singleton
XSAPI_STATS_MANAGER_VARS statsVars;

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerAddLocalUser(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->add_local_user(user->pImpl->cppUser());

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerRemoveLocalUser(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->remove_local_user(user->pImpl->cppUser());

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerRequestFlushToService(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ bool isHighPriority,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->request_flush_to_service(user->pImpl->cppUser(), isHighPriority);

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_STAT_EVENT** XBL_CALLING_CONV
StatsManagerDoWork(
    _Inout_ uint32_t* statEventsCount
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto cppEvents = stats_manager::get_singleton_instance()->do_work();

    statsVars.cEvents.clear();
    for (auto cppEvent : cppEvents) {
        statsVars.cEvents.push_back(CreateStatEventFromCpp(cppEvent));
    }
    *statEventsCount = (uint32_t)statsVars.cEvents.size();
    
    return statsVars.cEvents.data();
}
CATCH_RETURN_WITH(nullptr)

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerSetStatisticNumberData(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _In_ double statValue,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->set_stat_as_number(user->pImpl->cppUser(), utils::to_utf16string(statName), statValue);

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerSetStatisticIntegerData(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _In_ int64_t statValue,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->set_stat_as_integer(user->pImpl->cppUser(), utils::to_utf16string(statName), statValue);

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerSetStatisticStringData(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _In_ PCSTR statValue,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->set_stat_as_string(user->pImpl->cppUser(), utils::to_utf16string(statName), utils::to_utf16string(statValue));

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerGetStatNames(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _Inout_ PCSTR** statNameList,
    _Inout_ uint32_t* statNameListCount,
    _Inout_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppStatNameList.clear();
    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->get_stat_names(user->pImpl->cppUser(), statsVars.cppStatNameList);
    
    statsVars.cStatNameStringList.resize(statsVars.cppStatNameList.size());
    statsVars.cStatNameCharList.clear();
    for (size_t i = 0; i < statsVars.cppStatNameList.size(); i++)
    {
        auto name = utils::to_utf8string(statsVars.cppStatNameList.at(i));
        statsVars.cStatNameStringList[i] = name;
        statsVars.cStatNameCharList.push_back(statsVars.cStatNameStringList[i].c_str());
    }

    *statNameList = statsVars.cStatNameCharList.data();
    *statNameListCount = (uint32_t)statsVars.cStatNameCharList.size();

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerGetStat(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _Out_ XSAPI_STAT_VALUE** statValue,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppStatValueResult = stats_manager::get_singleton_instance()->get_stat(user->pImpl->cppUser(), utils::to_utf16string(statName));
    *statValue = CreateStatValueFromCpp(statsVars.cppStatValueResult.payload());

    *errMessage = statsVars.cppStatValueResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppStatValueResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerDeleteStat(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->delete_stat(user->pImpl->cppUser(), utils::to_utf16string(statName));

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerGetLeaderboard(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _In_ XSAPI_LEADERBOARD_QUERY* query,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->get_leaderboard(user->pImpl->cppUser(), utils::to_utf16string(statName), query->pImpl->cppQuery());

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
StatsManagerGetSocialLeaderboard(
    _In_ XSAPI_XBOX_LIVE_USER* user,
    _In_ PCSTR statName,
    _In_ PCSTR socialGroup,
    _In_ XSAPI_LEADERBOARD_QUERY* query,
    _Out_ PCSTR* errMessage
) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    statsVars.cppVoidResult = stats_manager::get_singleton_instance()->get_social_leaderboard(user->pImpl->cppUser(), utils::to_utf16string(statName), utils::to_utf16string(socialGroup), query->pImpl->cppQuery());

    *errMessage = statsVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(statsVars.cppVoidResult.err());
}
CATCH_RETURN()