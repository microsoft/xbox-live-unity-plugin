// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/xbox_live_app_config_c.h"

XboxLiveAppConfigImpl::XboxLiveAppConfigImpl()
{
    auto singleton = get_xsapi_singleton();
    std::lock_guard<std::mutex> lock(singleton->m_singletonLock);

    m_cppConfig = xbox::services::xbox_live_app_config::get_app_config_singleton();

    m_scid = utils::to_utf8string(m_cppConfig->scid());
    singleton->m_appConfigSingleton->scid = m_scid.data();

    m_environment = utils::to_utf8string(m_cppConfig->environment());
    singleton->m_appConfigSingleton->environment = m_environment.data();

    m_sandbox = utils::to_utf8string(m_cppConfig->sandbox());
    singleton->m_appConfigSingleton->sandbox = m_sandbox.data();
}

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
GetXboxLiveAppConfigSingleton(
    _Out_  XboxLiveAppConfig const** ppConfig
    ) XSAPI_NOEXCEPT
try
{
    if (ppConfig == nullptr)
    {
        return XSAPI_E_INVALIDARG;
    }
    
    auto singleton = get_xsapi_singleton();
    std::lock_guard<std::mutex> lock(singleton->m_singletonLock);

    if (singleton->m_appConfigSingleton == nullptr)
    {
        singleton->m_appConfigSingleton = std::make_shared<XboxLiveAppConfig>();
        singleton->m_appConfigImplSingleton = std::make_unique<XboxLiveAppConfigImpl>();
    }
    *ppConfig = singleton->m_appConfigSingleton.get();
   
    return XSAPI_OK;
}
CATCH_RETURN()