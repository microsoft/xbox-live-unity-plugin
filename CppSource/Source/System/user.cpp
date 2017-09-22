// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "user_impl.h"
#include "taskargs.h"

using namespace xbox::services;
using namespace xbox::services::system;

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserCreateFromSystemUser(
    _In_ Windows::System::User^ creationContext,
    _Out_ XboxLiveUser** ppUser
    ) XSAPI_NOEXCEPT
try
{
    if (ppUser == nullptr)
    {
        return XSAPI_E_INVALIDARG;
    }

    auto cUser = new XboxLiveUser();
    cUser->pImpl = new XboxLiveUserImpl(creationContext, cUser);

    *ppUser = cUser;

    return XSAPI_OK;
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserCreate(
    _Out_ XboxLiveUser** ppUser
    ) XSAPI_NOEXCEPT
try
{
    return XboxLiveUserCreateFromSystemUser(nullptr, ppUser);
}
CATCH_RETURN()

XSAPI_DLLEXPORT void XBL_CALLING_CONV
XboxLiveUserDelete(
    _In_ XboxLiveUser *user
    ) XSAPI_NOEXCEPT
try
{
    auto singleton = get_xsapi_singleton();
    std::lock_guard<std::mutex> lock(singleton->m_usersLock);
    
    singleton->m_signedInUsers.erase(user->pImpl->m_xboxUserId);
    delete user->pImpl;
    delete user;
}
CATCH_RETURN_WITH(;)

HC_RESULT XboxLiveUserSignInExecute(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    xbox_live_result<sign_in_result> result;
    auto args = reinterpret_cast<xbl_args_xbox_live_user_sign_in*>(context);
        
    if (args->coreDispatcher == nullptr)
    {
        if (args->signInSilently)
        {
            result = args->user->pImpl->m_cppUser->signin_silently().get();
        }
        else
        {
            result = args->user->pImpl->m_cppUser->signin().get();
        }
    }
    else
    {
        if (args->signInSilently)
        {
            result = args->user->pImpl->m_cppUser->signin_silently(args->coreDispatcher).get();
        }
        else
        {
            result = args->user->pImpl->m_cppUser->signin(args->coreDispatcher).get();
        }
    }

    args->resultErrorMsg = result.err_message();
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();
    
    if (!result.err())
    {
        args->result.payload.status = static_cast<SIGN_IN_STATUS>(result.payload().status());
        args->user->pImpl->Refresh();

        {
            auto singleton = get_xsapi_singleton();
            std::lock_guard<std::mutex> lock(singleton->m_usersLock);
            singleton->m_signedInUsers[args->user->pImpl->m_xboxUserId] = args->user;
        }
    }

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_RESULT XboxLiveUserSignInHelper(
    _Inout_ XboxLiveUser* user,
    _In_ Platform::Object^ coreDispatcher,
    _In_ bool signInSilently,
    _In_ SignInCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    )
{
    verify_global_init();

    auto args = new xbl_args_xbox_live_user_sign_in(user, coreDispatcher, signInSilently);

    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            XboxLiveUserSignInExecute,
            static_cast<void*>(args),
            xbl_execute_callback_fn<xbl_args_xbox_live_user_sign_in, SignInCompletionRoutine>,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));    
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserSignIn(
    _Inout_ XboxLiveUser* user,
    _In_ SignInCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    return XboxLiveUserSignInHelper(user, nullptr, false, completionRoutine, completionRoutineContext, taskGroupId);
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserSignInSilently(
    _Inout_ XboxLiveUser* user,
    _In_ SignInCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    return XboxLiveUserSignInHelper(user, nullptr, true, completionRoutine, completionRoutineContext, taskGroupId);
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserSignInWithCoreDispatcher(
    _Inout_ XboxLiveUser* user,
    _In_ Platform::Object^ coreDispatcher,
    _In_ SignInCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    return XboxLiveUserSignInHelper(user, coreDispatcher, false, completionRoutine, completionRoutineContext, taskGroupId);
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserSignInSilentlyWithCoreDispatcher(
    _Inout_ XboxLiveUser* user,
    _In_ Platform::Object^ coreDispatcher,
    _In_ SignInCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    return XboxLiveUserSignInHelper(user, coreDispatcher, true, completionRoutine, completionRoutineContext, taskGroupId);
}
CATCH_RETURN()

HC_RESULT XboxLiveUserGetTokenAndSignatureExecute(
    _In_opt_ void* context,
    _In_ HC_TASK_HANDLE taskHandle
    )
{
    auto args = reinterpret_cast<xbl_args_xbox_live_user_get_token_and_signature*>(context);    

    auto result = args->user->pImpl->m_cppUser->get_token_and_signature(
        utils::to_utf16string(args->httpMethod),
        utils::to_utf16string(args->url),
        utils::to_utf16string(args->headers),
        args->requestBodyString == nullptr ? string_t() : utils::to_utf16string(args->requestBodyString)
        ).get();

    args->resultErrorMsg = result.err_message();
    args->result.result.errorCode = result.err().value();
    args->result.result.errorMessage = args->resultErrorMsg.c_str();

    if (!result.err())
    {
        auto cppPayload = result.payload();
        TokenAndSignatureResultPayload& payload = args->result.payload;
        
        args->token = utils::to_utf8string(cppPayload.token());
        payload.token = args->token.data();

        args->signature = utils::to_utf8string(cppPayload.signature());
        payload.signature = args->signature.data();

        args->xboxUserId = utils::to_utf8string(cppPayload.xbox_user_id());
        payload.xboxUserId = args->xboxUserId.data();

        args->gamertag = utils::to_utf8string(cppPayload.gamertag());
        payload.gamertag = args->gamertag.data();

        args->xboxUserHash = utils::to_utf8string(cppPayload.xbox_user_hash());
        payload.xboxUserHash = args->xboxUserHash.data();

        args->ageGroup = utils::to_utf8string(cppPayload.age_group());
        payload.ageGroup = args->ageGroup.data();

        args->privileges = utils::to_utf8string(cppPayload.privileges());
        payload.privileges = args->privileges.data();

        args->webAccountId = utils::to_utf8string(cppPayload.web_account_id());
        payload.webAccountId = args->webAccountId.data();
    }

    return HCTaskSetCompleted(taskHandle);
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
XboxLiveUserGetTokenAndSignature(
    _Inout_ XboxLiveUser* user,
    _In_ PCSTR httpMethod,
    _In_ PCSTR url,
    _In_ PCSTR headers,
    _In_ PCSTR requestBodyString,
    _In_ GetTokenAndSignatureCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext,
    _In_ uint64_t taskGroupId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    auto args = new xbl_args_xbox_live_user_get_token_and_signature(
        user,
        httpMethod,
        url,
        headers,
        requestBodyString);
    
    return utils::xsapi_result_from_hc_result(
        HCTaskCreate(
            taskGroupId,
            XboxLiveUserGetTokenAndSignatureExecute,
            static_cast<void*>(args),
            xbl_execute_callback_fn<xbl_args_xbox_live_user_get_token_and_signature, GetTokenAndSignatureCompletionRoutine>,
            static_cast<void*>(args),
            static_cast<void*>(completionRoutine),
            completionRoutineContext,
            nullptr
        ));
}
CATCH_RETURN()

XSAPI_DLLEXPORT function_context XBL_CALLING_CONV
AddSignOutCompletedHandler(
    _In_ SignOutCompletedHandler signOutHandler
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    return xbox_live_user::add_sign_out_completed_handler(
        [signOutHandler](const xbox::services::system::sign_out_completed_event_args& args)
        {
            auto singleton = get_xsapi_singleton();
            std::lock_guard<std::mutex> lock(singleton->m_usersLock);
            
            auto iter = singleton->m_signedInUsers.find(utils::to_utf8string(args.user()->xbox_user_id()));
            if (iter != singleton->m_signedInUsers.end())
            {
                iter->second->pImpl->Refresh();
                signOutHandler(iter->second);
            }
        });
}
CATCH_RETURN_WITH(-1)

XSAPI_DLLEXPORT void XBL_CALLING_CONV
RemoveSignOutCompletedHandler(
    _In_ function_context context
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();
    xbox_live_user::remove_sign_out_completed_handler(context);
}
CATCH_RETURN_WITH(;)