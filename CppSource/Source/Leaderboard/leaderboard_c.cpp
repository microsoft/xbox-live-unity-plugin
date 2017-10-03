// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "leaderboard_helper_c.h"
#include "taskargs.h"

// todo move
std::vector<LeaderboardQuery *> m_queries;

XSAPI_DLLEXPORT LeaderboardQuery* XBL_CALLING_CONV
LeaderboardQueryCreate(
)
{
    verify_global_init();

    auto query = new LeaderboardQuery();
    query->pImpl = new LeaderboardQueryImpl(leaderboard_query(), query);
    m_queries.push_back(query);

    return query;
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToMe(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ bool skipResultToMe
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetSkipResultToMe(skipResultToMe);
}
XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetSkipResultToRank(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 skipResultToRank
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetSkipResultToRank(skipResultToRank);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetMaxItems(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ uint32 maxItems
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetMaxItems(maxItems);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
LeaderboardQuerySetOrder(
    _In_ LeaderboardQuery* leaderboardQuery,
    _In_ SORT_ORDER order
)
{
    verify_global_init();

    leaderboardQuery->pImpl->SetOrder(order);
}

XSAPI_DLLEXPORT bool XBL_CALLING_CONV
LeaderboardResultHasNext(
    _In_ LeaderboardResult* leaderboardResult
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
        GetNextResultPayload& payload = args->result.payload;

        auto leaderboardResult = CreateLeaderboardResultFromCpp(cppPayload);

        args->nextResult = leaderboardResult;
        payload.nextResult = args->nextResult;
    }

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
LeaderboardResultGetNext(
    _In_ LeaderboardResult* leaderboardResult,
    _In_ uint32 maxItems,
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

xbox_live_result<leaderboard_query> cppLeaderboardQueryResult;
XSAPI_DLLEXPORT int XBL_CALLING_CONV
LeaderboardResultGetNextQuery(
    _In_ LeaderboardResult* leaderboardResult,
    _In_ uint32 maxItems,
    _Out_ LeaderboardQuery** nextQuery,
    _Out_ PCSTR* errMessage
)
{
    verify_global_init();

    cppLeaderboardQueryResult = leaderboardResult->pImpl->m_cppLeaderboardResult.get_next_query();

    *nextQuery = CreateLeaderboardQueryFromCpp(cppLeaderboardQueryResult.payload());

    *errMessage = cppLeaderboardQueryResult.err_message().c_str();
    return cppLeaderboardQueryResult.err().value();
}