// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once

#include "threadpool.h"
#include "xbox_live_app_config_impl.h"

struct XboxLiveUser;
struct XboxLiveAppConfig;

struct xsapi_singleton
{
    xsapi_singleton();

    std::mutex m_singletonLock;

    std::unique_ptr<xsapi_thread_pool> m_threadPool;

    std::mutex m_usersLock;
    std::map<std::string, XboxLiveUser*> m_signedInUsers;

    std::shared_ptr<XboxLiveAppConfig> m_appConfigSingleton;
    std::unique_ptr<XboxLiveAppConfigImpl> m_appConfigImplSingleton;
};

xsapi_singleton* get_xsapi_singleton(_In_ bool createIfRequired = false);
void cleanup_xsapi_singleton();

void verify_global_init();
