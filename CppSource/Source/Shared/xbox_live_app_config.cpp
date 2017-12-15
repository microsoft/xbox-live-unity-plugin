// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/xbox_live_app_config_c.h"

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
GetXboxLiveAppConfigSingleton(
    _Out_  CONST XSAPI_XBOX_LIVE_APP_CONFIG** ppConfig
    ) XSAPI_NOEXCEPT
try
{
    if (ppConfig == nullptr)
    {
        return XSAPI_RESULT_E_HC_INVALIDARG;
    }
    
    auto singleton = get_xsapi_singleton();
    std::lock_guard<std::mutex> lock(singleton->m_singletonLock);

    if (singleton->m_appConfigSingleton == nullptr)
    {
        singleton->m_appConfigSingleton = std::make_unique<XSAPI_XBOX_LIVE_APP_CONFIG>();

        auto cppConfig = xbox::services::xbox_live_app_config::get_app_config_singleton();

        singleton->m_scid = utils::to_utf8string(cppConfig->scid());
        singleton->m_appConfigSingleton->scid = singleton->m_scid.data();

        singleton->m_environment = utils::to_utf8string(cppConfig->environment());
        singleton->m_appConfigSingleton->environment = singleton->m_environment.data();

        singleton->m_sandbox = utils::to_utf8string(cppConfig->sandbox());
        singleton->m_appConfigSingleton->sandbox = singleton->m_sandbox.data();

        singleton->m_appConfigSingleton->titleId = cppConfig->title_id();
    }
    *ppConfig = singleton->m_appConfigSingleton.get();
   
    return XSAPI_RESULT_OK;
}
CATCH_RETURN()