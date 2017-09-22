// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/title_callable_ui_c.h"
#include "taskargs.h"

using namespace xbox::services::system;

HC_RESULT TCUIShowProfileCardUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_show_profile_card_ui*>(executionRoutineContext);
    auto result = title_callable_ui::show_profile_card_ui(utils::to_utf16string(args->targetXboxUserId)).get();

    args->resultErrorMsg = result.err_message();
    args->result.errorCode = result.err().value();
    args->result.errorMessage = args->resultErrorMsg.c_str();

    return HCTaskSetCompleted(taskHandle);
}


XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
TCUIShowProfileCardUI(
    _In_ PCSTR targetXboxUserId,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto args = new xbl_args_tcui_show_profile_card_ui();
    args->targetXboxUserId = targetXboxUserId;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            TCUIShowProfileCardUIExecute,
            static_cast<void*>(args),
            xbl_execute_callback_fn<xbl_args_tcui_show_profile_card_ui, TCUIShowProfileCardUICompletionRoutine>,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()

HC_RESULT TCUICheckGamingPrivilegeSilentlyExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_check_gaming_privilege*>(executionRoutineContext);
    auto result = title_callable_ui::check_gaming_privilege_silently((gaming_privilege)args->privilege);

    args->resultErrorMsg = result.err_message();
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();
    args->result.hasPrivilege = result.payload();

    return HCTaskSetCompleted(taskHandle);    
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
TCUICheckGamingPrivilegeSilently(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto tcuiArgs = new xbl_args_tcui_check_gaming_privilege();
    tcuiArgs->privilege = privilege;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            TCUICheckGamingPrivilegeSilentlyExecute,
            static_cast<void*>(tcuiArgs),
            xbl_execute_callback_fn<xbl_args_tcui_check_gaming_privilege, TCUICheckGamingPrivilegeCompletionRoutine>,
            static_cast<void*>(tcuiArgs),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()


HC_RESULT TCUICheckGamingPrivilegeWithUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_tcui_check_gaming_privilege*>(executionRoutineContext);

    auto result = title_callable_ui::check_gaming_privilege_with_ui(
        (gaming_privilege)args->privilege,
        utils::to_utf16string(args->friendlyMessage)
        ).get();

    args->resultErrorMsg = result.err_message();
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();
    args->result.hasPrivilege = result.payload();

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
TCUICheckGamingPrivilegeWithUI(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ PCSTR friendlyMessage,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto tcuiArgs = new xbl_args_tcui_check_gaming_privilege();
    tcuiArgs->privilege = privilege;
    tcuiArgs->friendlyMessage = friendlyMessage;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            TCUICheckGamingPrivilegeWithUIExecute,
            static_cast<void*>(tcuiArgs),
            xbl_execute_callback_fn<xbl_args_tcui_check_gaming_privilege, TCUICheckGamingPrivilegeCompletionRoutine>,
            static_cast<void*>(tcuiArgs),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()
