// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"

using namespace xbox::services::system;
using namespace xbox::httpclient;

void TCUIShowProfileCardUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_show_profile_card_ui*>(executionRoutineContext);
    auto result = title_callable_ui::show_profile_card_ui(string_t(args->targetXboxUserId)).get();

    args->resultErrorMsg = to_utf16string(result.err_message());
    args->result.errorCode = result.err().value();
    args->result.errorMessage = args->resultErrorMsg.c_str();

    HCTaskSetCompleted(taskHandle);
}


XSAPI_DLLEXPORT void XBL_CALLING_CONV
TCUIShowProfileCardUI(
    _In_ PCSTR_T targetXboxUserId,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    )
{
    VerifyGlobalXsapiInit();

    auto args = new xbl_args_tcui_show_profile_card_ui();
    args->targetXboxUserId = targetXboxUserId;

    HC_TASK_HANDLE taskHandle = HCTaskCreate(
        taskGroupId,
        TCUIShowProfileCardUIExecute, 
        static_cast<void*>(args),
        xbl_execute_callback_fn<xbl_args_tcui_show_profile_card_ui, TCUIShowProfileCardUICompletionRoutine>, 
        static_cast<void*>(args),
        static_cast<void*>(completionRoutine), 
        completionRoutineContext,
        true
        );
}

void TCUICheckGamingPrivilegeSilentlyExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_check_gaming_privilege*>(executionRoutineContext);
    auto result = title_callable_ui::check_gaming_privilege_silently((gaming_privilege)args->privilege);

    args->resultErrorMsg = to_utf16string(result.err_message());
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();
    args->result.hasPrivilege = result.payload();

    HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
TCUICheckGamingPrivilegeSilently(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    )
{
    VerifyGlobalXsapiInit();

    auto tcuiArgs = new xbl_args_tcui_check_gaming_privilege();
    tcuiArgs->privilege = privilege;

    HC_TASK_HANDLE taskHandle = HCTaskCreate(
        taskGroupId,
        TCUICheckGamingPrivilegeSilentlyExecute, 
        static_cast<void*>(tcuiArgs),
        xbl_execute_callback_fn<xbl_args_tcui_check_gaming_privilege, TCUICheckGamingPrivilegeCompletionRoutine>, 
        static_cast<void*>(tcuiArgs),
        static_cast<void*>(completionRoutine),
        completionRoutineContext,
        true
        );
}


void TCUICheckGamingPrivilegeWithUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )

{
    auto args = reinterpret_cast<xbl_args_tcui_check_gaming_privilege*>(executionRoutineContext);

    auto result = title_callable_ui::check_gaming_privilege_with_ui(
        (gaming_privilege)args->privilege,
        string_t(args->friendlyMessage)
        ).get();

    args->resultErrorMsg = to_utf16string(result.err_message());
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();
    args->result.hasPrivilege = result.payload();

    HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
TCUICheckGamingPrivilegeWithUI(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ PCSTR_T friendlyMessage,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    )
{
    VerifyGlobalXsapiInit();

    auto tcuiArgs = new xbl_args_tcui_check_gaming_privilege();
    tcuiArgs->privilege = privilege;
    tcuiArgs->friendlyMessage = friendlyMessage;

    HC_TASK_HANDLE taskHandle = HCTaskCreate(
        taskGroupId,
        TCUICheckGamingPrivilegeWithUIExecute, 
        static_cast<void*>(tcuiArgs),
        xbl_execute_callback_fn<xbl_args_tcui_check_gaming_privilege, TCUICheckGamingPrivilegeCompletionRoutine>, 
        static_cast<void*>(tcuiArgs),
        static_cast<void*>(completionRoutine), 
        completionRoutineContext,
        true
        );
}

