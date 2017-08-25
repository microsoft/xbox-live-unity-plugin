// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once

template<typename T>
struct xbl_args
{
    T result;
    virtual ~xbl_args() {}
};

struct xbl_args_tcui_show_profile_card_ui : public xbl_args<XboxLiveResult>
{
    PCSTR_T targetXboxUserId;

    string_t resultErrorMsg;
};

struct xbl_args_tcui_check_gaming_privilege : public xbl_args<TCUICheckGamingPrivilegeResult>
{
    GAMING_PRIVILEGE privilege;
    string_t resultErrorMsg;
    string_t friendlyMessage;
};

struct xbl_args_xbox_live_user_sign_in : public xbl_args<SignInResult>
{
    xbl_args_xbox_live_user_sign_in(
        _In_ XboxLiveUser* user,
        _In_ Platform::Object^ coreDispatcher,
        _In_opt_ bool signInSilently = false
        );

    XboxLiveUser* user;
    bool signInSilently;
    Platform::Object^ coreDispatcher;

    string_t resultErrorMsg;
};

struct xbl_args_xbox_live_user_get_token_and_signature : public xbl_args<TokenAndSignatureResult>
{
    xbl_args_xbox_live_user_get_token_and_signature(
        _In_ XboxLiveUser* user,
        _In_ PCSTR_T httpMethod,
        _In_ PCSTR_T url,
        _In_ PCSTR_T headers,
        _In_ PCSTR_T requestBodyString
        );

    XboxLiveUser* user;
    PCSTR_T httpMethod;
    PCSTR_T url;
    PCSTR_T headers;
    PCSTR_T requestBodyString;

    string_t token;
    string_t signature;
    string_t xboxUserId;
    string_t gamertag;
    string_t xboxUserHash;
    string_t ageGroup;
    string_t privileges;
    string_t webAccountId;

    string_t resultErrorMsg;
};

struct xbl_args_xbox_live_user_refresh_token : public xbl_args<XboxLiveResult>
{
    string_t resultErrorMsg;
};

template<typename T, typename T2>
void xbl_execute_callback_fn(
    _In_opt_ void* writeResultsRoutineContext,
    _In_ HC_TASK_HANDLE taskHandle,
    _In_opt_ void* completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    auto args = reinterpret_cast<T*>(writeResultsRoutineContext);
      
    T2 completeFn = (T2)completionRoutine;
    if (completeFn != nullptr)
    {
        completeFn(args->result, completionRoutineContext);
    }
    delete args;
}