// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/title_callable_ui_c.h"
#include "taskargs.h"

using namespace xbox::services::system;

struct show_profile_card_taskargs : public taskargs
{
    PCSTR targetXboxUserId;
};

struct check_gaming_privilege_taskargs : public taskargs_with_payload<bool>
{
    XSAPI_GAMING_PRIVILEGE privilege;
    std::string friendlyMessage;
};

HC_RESULT TCUIShowProfileCardUIExecute(
    _In_opt_ void* executionRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<show_profile_card_taskargs*>(executionRoutineContext);
    auto result = title_callable_ui::show_profile_card_ui(utils::to_utf16string(args->targetXboxUserId)).get();
    args->copy_xbox_live_result(result);

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
TCUIShowProfileCardUI(
    _In_ PCSTR targetXboxUserId,
    _In_ XSAPI_SHOW_PROFILE_CARD_UI_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto args = new show_profile_card_taskargs();
    args->targetXboxUserId = targetXboxUserId;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            TCUIShowProfileCardUIExecute,
            static_cast<void*>(args),
            utils::execute_completion_routine<show_profile_card_taskargs, XSAPI_SHOW_PROFILE_CARD_UI_COMPLETION_ROUTINE>,
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
    auto args = reinterpret_cast<check_gaming_privilege_taskargs*>(executionRoutineContext);
    auto result = title_callable_ui::check_gaming_privilege_silently((gaming_privilege)args->privilege);

    args->copy_xbox_live_result(result);
    args->completionRoutinePayload = result.payload();

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
TCUICheckGamingPrivilegeSilently(
    _In_ XSAPI_GAMING_PRIVILEGE privilege,
    _In_ XSAPI_CHECK_GAMING_PRIVILEGE_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto tcuiArgs = new check_gaming_privilege_taskargs();
    tcuiArgs->privilege = privilege;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            TCUICheckGamingPrivilegeSilentlyExecute,
            static_cast<void*>(tcuiArgs),
            utils::execute_completion_routine_with_payload<check_gaming_privilege_taskargs, XSAPI_CHECK_GAMING_PRIVILEGE_COMPLETION_ROUTINE>,
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
    auto args = reinterpret_cast<check_gaming_privilege_taskargs*>(executionRoutineContext);

    auto result = title_callable_ui::check_gaming_privilege_with_ui(
        (gaming_privilege)args->privilege,
        utils::to_utf16string(args->friendlyMessage)
        ).get();

    args->copy_xbox_live_result(result);
    args->completionRoutinePayload = result.payload();

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
TCUICheckGamingPrivilegeWithUI(
    _In_ XSAPI_GAMING_PRIVILEGE privilege,
    _In_ PCSTR friendlyMessage,
    _In_ XSAPI_CHECK_GAMING_PRIVILEGE_COMPLETION_ROUTINE completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto tcuiArgs = new check_gaming_privilege_taskargs();
    tcuiArgs->privilege = privilege;
    tcuiArgs->friendlyMessage = friendlyMessage;

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            TCUICheckGamingPrivilegeWithUIExecute,
            static_cast<void*>(tcuiArgs),
            utils::execute_completion_routine_with_payload<check_gaming_privilege_taskargs, XSAPI_CHECK_GAMING_PRIVILEGE_COMPLETION_ROUTINE>,
            static_cast<void*>(tcuiArgs),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()
