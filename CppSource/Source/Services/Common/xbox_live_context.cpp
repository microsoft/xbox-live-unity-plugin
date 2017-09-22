// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/xbox_live_app_config_c.h"
#include "xsapi/xbox_live_context_c.h"
#include "user_impl.h"

using namespace xbox::services;

struct XboxLiveContextImpl
{
    XboxLiveContextImpl(
        _In_ XboxLiveUser *user,
        _In_ XboxLiveContext *cContext
        ) 
        : m_cContext(cContext),
        m_cppContext(user->pImpl->m_cppUser)
    {
         GetXboxLiveAppConfigSingleton(&(cContext->appConfig));
    }

    XboxLiveContext *m_cContext;
    xbox_live_context m_cppContext;
};

XSAPI_DLLEXPORT XboxLiveContext* XBL_CALLING_CONV
XboxLiveContextCreate(
    XboxLiveUser *user
    )
{
    // TODO improve error handling
    if (user == nullptr)
    {
        return nullptr;
    }

    verify_global_init();

    auto context = new XboxLiveContext();
    context->pImpl = new XboxLiveContextImpl(user, context);
    return context;
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
XboxLiveContextDelete(
    XboxLiveContext *context
    )
{
    delete context->pImpl;
    delete context;
}
