// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "../System/singleton.h"
#include "../System/asyncop.h"

struct xbl_args_tcui_show_profile_card_ui : public xbl_args
{
    PCSTR_T targetXboxUserId;
    Windows::System::User^ user;
    XboxLiveResult m_result;
};

void 
show_profile_card_ui_execute_routine(_In_ const std::shared_ptr<xsapi_async_info>& info)
{
    auto args = std::dynamic_pointer_cast<xbl_args_tcui_show_profile_card_ui>(info->args);
    auto result = xbox::services::system::title_callable_ui::show_profile_card_ui(string_t(args->targetXboxUserId)).get();

    XboxLiveResult ret;
    ret.errorCode = result.err().value();
    // ret.errorMessage = result.err_message().c_str();
    
    if (info->completionRoutine != nullptr)
    {
        TCUIShowProfileCardUICompletionRoutine callbackFn = static_cast<TCUIShowProfileCardUICompletionRoutine>(info->completionRoutine);
        callbackFn(info->completionRoutineContext, ret);
    }
}

_XSAPIIMP void XSAPI_CALL
TCUIShowProfileCardUI(
    _In_ PCSTR_T targetXboxUserId,
    _In_opt_ Windows::System::User^ user,
    _In_ void* completionRoutineContext,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine
    )
{
    std::shared_ptr<xsapi_async_info> info = std::make_shared<xsapi_async_info>();
    info->executeRoutine = (xbl_async_op_execute_routine)show_profile_card_ui_execute_routine;
    info->resultRoutine = (xbl_async_op_result_routine)xbl_asyncop_write_result<XboxLiveResult, xbl_args_tcui_show_profile_card_ui>;
    info->args = std::make_shared<xbl_args_tcui_show_profile_card_ui>();
    xbl_asyncop_set_info_in_new_handle(info, completionRoutineContext, static_cast<void*>(completionRoutine));
}
