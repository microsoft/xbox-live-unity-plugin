// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"

using namespace xbox::httpclient;

void TCUIShowProfileCardUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_show_profile_card_ui*>(executionRoutineContext);
    auto result = xbox::services::system::title_callable_ui::show_profile_card_ui(string_t(args->targetXboxUserId)).get();

    args->resultErrorMsg = to_utf16string(result.err_message());
    args->m_result.errorCode = result.err().value();
    args->m_result.errorMessage = args->resultErrorMsg.c_str();
    HCTaskSetCompleted(taskHandle);
}


XSAPI_DLLEXPORT void XBL_CALLING_CONV
TCUIShowProfileCardUI(
    _In_ PCSTR_T targetXboxUserId,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    VerifyGlobalXsapiInit();

    auto tcuiArgs = std::make_shared<xbl_args_tcui_show_profile_card_ui>();
    tcuiArgs->targetXboxUserId = targetXboxUserId;

    uint64_t taskGroupId = 0;
    HC_TASK_HANDLE taskHandle = HCTaskCreate(
        taskGroupId,
        TCUIShowProfileCardUIExecute, static_cast<void*>(tcuiArgs.get()),
        xbl_execute_callback_fn<XboxLiveResult, xbl_args_tcui_show_profile_card_ui, TCUIShowProfileCardUICompletionRoutine>, static_cast<void*>(tcuiArgs.get()),
        static_cast<void*>(completionRoutine), completionRoutineContext,
        true
        );
    StoreTaskArgs(taskHandle, tcuiArgs);
}

void TCUICheckGamingPrivilegeSilentlyExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_check_gaming_privilege*>(executionRoutineContext);
    auto result = xbox::services::system::title_callable_ui::check_gaming_privilege_silently((xbox::services::system::gaming_privilege)args->privilege);

    args->resultErrorMsg = to_utf16string(result.err_message());
    args->m_result.result.errorCode = result.err().value();
    args->m_result.result.errorMessage = args->resultErrorMsg.c_str();
    args->m_result.hasPrivilege = result.payload();
    HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
TCUICheckGamingPrivilegeSilently(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    VerifyGlobalXsapiInit();

    auto tcuiArgs = std::make_shared<xbl_args_tcui_check_gaming_privilege>();
    tcuiArgs->privilege = privilege;

    uint64_t taskGroupId = 0;
    HC_TASK_HANDLE taskHandle = HCTaskCreate(
        taskGroupId,
        TCUICheckGamingPrivilegeSilentlyExecute, static_cast<void*>(tcuiArgs.get()),
        xbl_execute_callback_fn<TCUICheckGamingPrivilegeResult, xbl_args_tcui_check_gaming_privilege, TCUICheckGamingPrivilegeCompletionRoutine>, static_cast<void*>(tcuiArgs.get()),
        static_cast<void*>(completionRoutine), completionRoutineContext,
        true
        );
    StoreTaskArgs(taskHandle, tcuiArgs);
}


void TCUICheckGamingPrivilegeWithUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_check_gaming_privilege*>(executionRoutineContext);
    auto result = xbox::services::system::title_callable_ui::check_gaming_privilege_with_ui(
        (xbox::services::system::gaming_privilege)args->privilege,
        string_t(args->friendlyMessage)
        ).get();

    args->resultErrorMsg = to_utf16string(result.err_message());
    args->m_result.result.errorCode = result.err().value();
    args->m_result.result.errorMessage = args->resultErrorMsg.c_str();
    args->m_result.hasPrivilege = result.payload();
    HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
TCUICheckGamingPrivilegeWithUI(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ PCSTR_T friendlyMessage,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    VerifyGlobalXsapiInit();

    auto tcuiArgs = std::make_shared<xbl_args_tcui_check_gaming_privilege>();
    tcuiArgs->privilege = privilege;
    tcuiArgs->friendlyMessage = friendlyMessage;

    uint64_t taskGroupId = 0;
    HC_TASK_HANDLE taskHandle = HCTaskCreate(
        taskGroupId,
        TCUICheckGamingPrivilegeWithUIExecute, static_cast<void*>(tcuiArgs.get()),
        xbl_execute_callback_fn<TCUICheckGamingPrivilegeResult, xbl_args_tcui_check_gaming_privilege, TCUICheckGamingPrivilegeCompletionRoutine>, static_cast<void*>(tcuiArgs.get()),
        static_cast<void*>(completionRoutine), completionRoutineContext,
        true
        );
    StoreTaskArgs(taskHandle, tcuiArgs);
}

