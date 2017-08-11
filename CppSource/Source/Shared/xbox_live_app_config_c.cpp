// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xbox_live_app_config_impl.h"

XboxLiveAppConfigImpl::XboxLiveAppConfigImpl()
{
    auto singleton = get_xsapi_singleton();
    std::lock_guard<std::mutex> lock(singleton->m_singletonLock);

    m_cppConfig = xbox::services::xbox_live_app_config::get_app_config_singleton();

    m_scid = m_cppConfig->scid();
    singleton->m_appConfigSinglton->scid = m_scid.data();

    m_environment = m_cppConfig->environment();
    singleton->m_appConfigSinglton->environment = m_environment.data();

    m_sandbox = m_cppConfig->sandbox();
    singleton->m_appConfigSinglton->sandbox = m_sandbox.data();
}

XSAPI_DLLEXPORT const XboxLiveAppConfig* XBL_CALLING_CONV
GetXboxLiveAppConfigSingleton()
{
    auto singleton = get_xsapi_singleton();
    std::lock_guard<std::mutex> lock(singleton->m_singletonLock);

    if (singleton->m_appConfigSinglton == nullptr)
    {
        singleton->m_appConfigSinglton = std::make_shared<XboxLiveAppConfig>();
        singleton->m_appConfigImplSinglton = std::make_unique<XboxLiveAppConfigImpl>();
    }
    return singleton->m_appConfigSinglton.get();
}
