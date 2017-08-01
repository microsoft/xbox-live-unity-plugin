// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once

struct xbl_args
{
    virtual ~xbl_args() {}
};


struct xbl_args_tcui_show_profile_card_ui : public xbl_args
{
    PCSTR_T targetXboxUserId;
    std::wstring resultErrorMsg;
    XboxLiveResult m_result;
};

struct xbl_args_tcui_check_gaming_privilege : public xbl_args
{
    GAMING_PRIVILEGE privilege;
    std::wstring resultErrorMsg;
    std::wstring friendlyMessage;
    TCUICheckGamingPrivilegeResult m_result;
};

template<typename T, typename T2> void
xbl_asyncop_write_result(_In_ HC_TASK_HANDLE taskHandle, _Out_ void* result)
{
    std::shared_ptr<xbl_args> xblArgs = GetTaskArgs(taskHandle);
    auto args = std::dynamic_pointer_cast<T2>(xblArgs);
    *(T*)result = args->m_result;
}

template<typename T1, typename T2, typename T3>
void xbl_execute_callback_fn(
    _In_opt_ void* writeResultsRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    T1 result;
    xbl_asyncop_write_result<T1, T2>(taskHandle, &result);
    T3 completeFn = (T3)taskHandle->completionRoutine;
    if (completeFn != nullptr)
    {
        completeFn(result, taskHandle->completionRoutineContext);
    }
    ClearTaskArgs(taskHandle);
}


void StoreTaskArgs(_In_ HC_TASK_HANDLE taskHandle, _In_ std::shared_ptr<xbl_args> args);
std::shared_ptr<xbl_args> GetTaskArgs(_In_ HC_TASK_HANDLE taskHandle);
void ClearTaskArgs(_In_ HC_TASK_HANDLE taskHandle);
