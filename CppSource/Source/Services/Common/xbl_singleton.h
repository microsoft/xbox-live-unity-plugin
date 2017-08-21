// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once
#include "threadpool.h"
#include "xbox_live_app_config_impl.h"

struct xsapi_singleton
{
    xsapi_singleton();
    ~xsapi_singleton();

    std::mutex m_singletonLock;

    std::unique_ptr<xbl_thread_pool> m_threadPool;

    std::mutex m_usersLock;
    std::map<string_t, XboxLiveUser*> m_signedInUsers;

    std::shared_ptr<XboxLiveAppConfig> m_appConfigSinglton;
    std::unique_ptr<XboxLiveAppConfigImpl> m_appConfigImplSinglton;
};

xsapi_singleton* get_xsapi_singleton(_In_ bool createIfRequired = false);

void VerifyGlobalXsapiInit();
