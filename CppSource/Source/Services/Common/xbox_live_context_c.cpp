// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "user_impl_c.h"

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
        cContext->appConfig = GetXboxLiveAppConfigSingleton();
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

    VerifyGlobalXsapiInit();

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
