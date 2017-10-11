// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "xsapi/xbox_live_context_c.h"

struct XSAPI_XBOX_LIVE_CONTEXT_IMPL
{
public:
    XSAPI_XBOX_LIVE_CONTEXT_IMPL(_In_ CONST XSAPI_XBOX_LIVE_USER* pUser, _In_ XSAPI_XBOX_LIVE_CONTEXT *pContext);
    xbox::services::xbox_live_context& cppObject();

private:
    XSAPI_XBOX_LIVE_CONTEXT *m_pContext;
    xbox::services::xbox_live_context m_cppContext;
};
