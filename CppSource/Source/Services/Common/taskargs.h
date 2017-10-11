// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once

#include "xsapi/errors_c.h"
#include "xsapi/title_callable_ui_c.h"
#include "xsapi/system_c.h"
#include "xsapi/leaderboard_c.h"

template<typename T>
struct xbl_args
{
    T result;
    virtual ~xbl_args() {}
};

struct xbl_args_tcui_show_profile_card_ui : public xbl_args<XSAPI_RESULT_INFO>
{
    PCSTR targetXboxUserId;
    std::string resultErrorMsg;
};

struct xbl_args_tcui_check_gaming_privilege : public xbl_args<XSAPI_TCUI_CHECK_GAMING_PRIVILEGE_RESULT>
{
    XSAPI_GAMING_PRIVILEGE privilege;
    std::string resultErrorMsg;
    std::string friendlyMessage;
};

struct xbl_args_xbox_live_user_sign_in : public xbl_args<XSAPI_SIGN_IN_RESULT>
{
    xbl_args_xbox_live_user_sign_in(
        _In_ XSAPI_XBOX_LIVE_USER* pUser,
        _In_ Platform::Object^ coreDispatcher,
        _In_opt_ bool signInSilently = false
        );

    XSAPI_XBOX_LIVE_USER* pUser;
    bool signInSilently;
    Platform::Object^ coreDispatcher;

    std::string resultErrorMsg;
};

struct xbl_args_xbox_live_user_get_token_and_signature : public xbl_args<XSAPI_TOKEN_AND_SIGNATURE_RESULT>
{
    xbl_args_xbox_live_user_get_token_and_signature(
        _In_ XSAPI_XBOX_LIVE_USER* pUser,
        _In_ PCSTR httpMethod,
        _In_ PCSTR url,
        _In_ PCSTR headers,
        _In_ PCSTR requestBodyString
        );

    XSAPI_XBOX_LIVE_USER* pUser;
    PCSTR httpMethod;
    PCSTR url;
    PCSTR headers;
    PCSTR requestBodyString;

    std::string token;
    std::string signature;
    std::string xboxUserId;
    std::string gamertag;
    std::string xboxUserHash;
    std::string ageGroup;
    std::string privileges;
    std::string webAccountId;

    std::string resultErrorMsg;
};

struct xbl_args_xbox_live_user_refresh_token : public xbl_args<XSAPI_RESULT_INFO>
{
    std::string resultErrorMsg;
};

struct xbl_args_leaderboard_result_get_next : public xbl_args<XSAPI_GET_NEXT_RESULT>
{
    xbl_args_leaderboard_result_get_next(
        _In_ XSAPI_LEADERBOARD_RESULT* leaderboard,
        _In_ uint32 maxItems
    );

    XSAPI_LEADERBOARD_RESULT* leaderboard;
    uint32 maxItems;

    XSAPI_LEADERBOARD_RESULT* nextResult;

    std::string resultErrorMsg;
};
