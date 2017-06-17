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

    XboxLiveResult xboxLiveResult;
    xboxLiveResult.errorCode = result.err().value();
    std::wstring errMessage = std::wstring(result.err_message().begin(), result.err_message().end());
    xboxLiveResult.errorMessage = errMessage.c_str();
    
    if (info->completionRoutine != nullptr)
    {
        TCUIShowProfileCardUICompletionRoutine callbackFn = static_cast<TCUIShowProfileCardUICompletionRoutine>(info->completionRoutine);
        callbackFn(xboxLiveResult, info->completionRoutineContext);
    }
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUIShowProfileCardUI(
    _In_ PCSTR_T targetXboxUserId,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    std::shared_ptr<xsapi_async_info> info = std::make_shared<xsapi_async_info>();
    info->executeRoutine = (xbl_async_op_execute_routine)show_profile_card_ui_execute_routine;
    info->resultRoutine = (xbl_async_op_result_routine)xbl_asyncop_write_result<XboxLiveResult, xbl_args_tcui_show_profile_card_ui>;
    auto tcuiArgs = std::make_shared<xbl_args_tcui_show_profile_card_ui>();
    tcuiArgs->targetXboxUserId = targetXboxUserId;
    info->args = tcuiArgs;

    xbl_asyncop_set_info_in_new_handle(info, completionRoutineContext, static_cast<void*>(completionRoutine));
}
