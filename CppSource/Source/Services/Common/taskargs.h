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

struct xbl_args_tcui_show_profile_card_ui : public xbl_args<XboxLiveResult>
{
    PCSTR targetXboxUserId;
    std::string resultErrorMsg;
};

struct xbl_args_tcui_check_gaming_privilege : public xbl_args<TCUICheckGamingPrivilegeResult>
{
    GAMING_PRIVILEGE privilege;
    std::string resultErrorMsg;
    std::string friendlyMessage;
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

    std::string resultErrorMsg;
};

struct xbl_args_xbox_live_user_get_token_and_signature : public xbl_args<TokenAndSignatureResult>
{
    xbl_args_xbox_live_user_get_token_and_signature(
        _In_ XboxLiveUser* user,
        _In_ PCSTR httpMethod,
        _In_ PCSTR url,
        _In_ PCSTR headers,
        _In_ PCSTR requestBodyString
        );

    XboxLiveUser* user;
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

struct xbl_args_xbox_live_user_refresh_token : public xbl_args<XboxLiveResult>
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
