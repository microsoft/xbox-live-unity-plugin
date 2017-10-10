// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "leaderboard_helper_c.h"
#include "taskargs.h"

// todo - move global variabes into a pimpl and store it in the xsapi singleton
std::vector<XSAPI_LEADERBOARD_QUERY *> m_queries;
xbox_live_result<leaderboard_query> cppLeaderboardQueryResult;

XSAPI_DLLEXPORT XSAPI_LEADERBOARD_QUERY* XBL_CALLING_CONV
LeaderboardQueryCreate(
)
{
    verify_global_init();

    auto query = new XSAPI_LEADERBOARD_QUERY();
    query->pImpl = new XSAPI_LEADERBOARD_QUERY_IMPL(leaderboard_query(), query);
    m_queries.push_back(query);

    return query;
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToMe(
    _In_ XSAPI_LEADERBOARD_QUERY* leaderboardQuery,
    _In_ bool skipResultToMe
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetSkipResultToMe(skipResultToMe);
}
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToRank(
    _In_ XSAPI_LEADERBOARD_QUERY* leaderboardQuery,
    _In_ uint32_t skipResultToRank
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetSkipResultToRank(skipResultToRank);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetMaxItems(
    _In_ XSAPI_LEADERBOARD_QUERY* leaderboardQuery,
    _In_ uint32_t maxItems
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetMaxItems(maxItems);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetOrder(
    _In_ XSAPI_LEADERBOARD_QUERY* leaderboardQuery,
    _In_ XSAPI_SORT_ORDER order
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetOrder(order);
}

XSAPI_DLLEXPORT bool XBL_CALLING_CONV
LeaderboardResultHasNext(
    _In_ XSAPI_LEADERBOARD_RESULT* leaderboardResult
)
{
    verify_global_init();

    return leaderboardResult->pImpl->m_cppLeaderboardResult.has_next();
}


#if !defined(XBOX_LIVE_CREATORS_SDK)
HC_RESULT LeaderboardResultGetNextExecute(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle
)
{
    auto args = reinterpret_cast<xbl_args_leaderboard_result_get_next*>(context);

    auto result = args->leaderboard->pImpl->m_cppLeaderboardResult.get_next(args->maxItems).get();
    
    args->resultErrorMsg = result.err_message();
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();

    if (!result.err())
    {
        auto cppPayload = result.payload();
        XSAPI_GET_NEXT_RESULT_PAYLOAD& payload = args->result.payload;

        auto leaderboardResult = CreateLeaderboardResultFromCpp(cppPayload);

        args->nextResult = leaderboardResult;
        payload.nextResult = args->nextResult;
    }

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
LeaderboardResultGetNext(
    _In_ XSAPI_LEADERBOARD_RESULT* leaderboardResult,
    _In_ uint32_t maxItems,
    _In_ GetNextCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
)
{
    verify_global_init();

    auto args = new xbl_args_leaderboard_result_get_next(
        leaderboardResult,
        maxItems
    );

    return utils::xsapi_result_from_hc_result(HCTaskCreate(
        taskGroupId,
        LeaderboardResultGetNextExecute,
        static_cast<void*>(args),
        xbl_execute_callback_fn<xbl_args_leaderboard_result_get_next, GetNextCompletionRoutine>,
        static_cast<void*>(args),
        static_cast<void*>(completionRoutine),
        completionRoutineContext,
        nullptr
    ));
}
#endif

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
LeaderboardResultGetNextQuery(
    _In_ XSAPI_LEADERBOARD_RESULT* leaderboardResult,
    _Out_ XSAPI_LEADERBOARD_QUERY** nextQuery,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    cppLeaderboardQueryResult = leaderboardResult->pImpl->m_cppLeaderboardResult.get_next_query();

    *nextQuery = CreateLeaderboardQueryFromCpp(cppLeaderboardQueryResult.payload());

    *errMessage = cppLeaderboardQueryResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result<leaderboard_query>(cppLeaderboardQueryResult);
}