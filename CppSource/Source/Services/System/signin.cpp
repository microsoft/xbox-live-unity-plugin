// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/services.h"
#include "xsapi/types.h"
#include "xsapi/services_c.h"
#include "singleton.h"
#include "asyncop.h"

std::unique_ptr<xbox::services::system::xbox_live_user> s_user;

void 
xbl_signin_silently_execute(_In_ const std::shared_ptr<xsapi_async_info>& info)
{
    auto args = std::dynamic_pointer_cast<xbl_args_xbl_signin_silently>(info->args);
    s_user = std::make_unique<xbox::services::system::xbox_live_user>();
    auto result = s_user->signin_silently().get();

    xbl_signin_silently_result ret;
    ret.errorCode = result.err().value();
    ret.signInResultCode = result.payload().status();
    
    args->m_result = ret;
    if (info->completionRoutine != nullptr)
    {
        xbl_signin_silently_completion_routine callbackFn = static_cast<xbl_signin_silently_completion_routine>(info->completionRoutine);
        callbackFn(info->completionRoutineContext, ret);
    }
}

XSAPI_ASYNC_HANDLE
XSAPI_CALL
xbl_signin_silently(
    _In_ void* completionRoutineContext,
    _In_ xbl_signin_silently_completion_routine completionRoutine
    )
{
    std::shared_ptr<xsapi_async_info> info = std::make_shared<xsapi_async_info>();
    info->executeRoutine = (xbl_async_op_execute_routine)xbl_signin_silently_execute;
    info->resultRoutine = (xbl_async_op_result_routine)xbl_asyncop_write_result<xbl_signin_silently_result, xbl_args_xbl_signin_silently>;
    info->args = std::make_shared<xbl_args_xbl_signin_silently>();
    xbl_asyncop_set_info_in_new_handle(info, completionRoutineContext, static_cast<void*>(completionRoutine));
    return info->handleId;
}
