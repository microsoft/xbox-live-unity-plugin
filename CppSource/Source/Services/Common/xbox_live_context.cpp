// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/xbox_live_app_config_c.h"
#include "xsapi/xbox_live_context_c.h"
#include "user_impl.h"

using namespace xbox::services;

struct XSAPI_XBOX_LIVE_CONTEXT_IMPL
{
public:
    XSAPI_XBOX_LIVE_CONTEXT_IMPL(
        _In_ XSAPI_XBOX_LIVE_USER* pUser,
        _In_ XSAPI_XBOX_LIVE_CONTEXT* pXboxLiveContext
        ) 
        : m_pXboxLiveContext(pXboxLiveContext),
        m_cppContext(pUser->pImpl->cppUser())
    {
        GetXboxLiveAppConfigSingleton(&m_pXboxLiveContext->pAppConfig);
    }

private:
    XSAPI_XBOX_LIVE_CONTEXT* m_pXboxLiveContext;
    xbox_live_context m_cppContext;
};

XSAPI_DLLEXPORT XSAPI_XBOX_LIVE_CONTEXT* XBL_CALLING_CONV
XboxLiveContextCreate(
    XSAPI_XBOX_LIVE_USER* pUser
    )
{
    // TODO improve error handling
    if (pUser == nullptr)
    {
        return nullptr;
    }

    verify_global_init();

    auto context = new XSAPI_XBOX_LIVE_CONTEXT();
    context->pImpl = new XSAPI_XBOX_LIVE_CONTEXT_IMPL(pUser, context);
    return context;
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
XboxLiveContextDelete(
    XSAPI_XBOX_LIVE_CONTEXT *context
    )
{
    delete context->pImpl;
    delete context;
}
