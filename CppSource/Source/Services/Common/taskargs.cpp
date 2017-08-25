// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#include "pch.h"
#include "taskargs.h"

xbl_args_xbox_live_user_sign_in::xbl_args_xbox_live_user_sign_in(
    _In_ XboxLiveUser* _user,
    _In_ Platform::Object^ _coreDispatcher,
    _In_opt_ bool _signInSilently
    )
    : user(_user),
    coreDispatcher(_coreDispatcher),
    signInSilently(_signInSilently)
{
}

xbl_args_xbox_live_user_get_token_and_signature::xbl_args_xbox_live_user_get_token_and_signature(
    _In_ XboxLiveUser* _user,
    _In_ PCSTR_T _httpMethod,
    _In_ PCSTR_T _url,
    _In_ PCSTR_T _headers,
    _In_ PCSTR_T _requestBodyString
    )
    : user(_user),
    httpMethod(_httpMethod),
    url(_url),
    headers(_headers),
    requestBodyString(_requestBodyString)
{
}