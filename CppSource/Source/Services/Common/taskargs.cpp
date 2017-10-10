// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#include "pch.h"
#include "taskargs.h"

xbl_args_xbox_live_user_sign_in::xbl_args_xbox_live_user_sign_in(
    _In_ XSAPI_XBOX_LIVE_USER* _pUser,
    _In_ Platform::Object^ _coreDispatcher,
    _In_opt_ bool _signInSilently
    )
    : pUser(_pUser),
    coreDispatcher(_coreDispatcher),
    signInSilently(_signInSilently)
{
}

xbl_args_xbox_live_user_get_token_and_signature::xbl_args_xbox_live_user_get_token_and_signature(
    _In_ XSAPI_XBOX_LIVE_USER* _pUser,
    _In_ PCSTR _httpMethod,
    _In_ PCSTR _url,
    _In_ PCSTR _headers,
    _In_ PCSTR _requestBodyString
    )
    : pUser(_pUser),
    httpMethod(_httpMethod),
    url(_url),
    headers(_headers),
    requestBodyString(_requestBodyString)
{
}