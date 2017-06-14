// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "singleton.h"

static std::mutex g_xsapiSingletonLock;
static std::unique_ptr<xsapi_singleton> g_xsapiSingleton;

xsapi_singleton::xsapi_singleton() :
    asyncLastHandleId(0)
{
    threadPool = std::make_unique<xbl_thread_pool>();
    threadPool->start_threads();
}

xsapi_singleton::~xsapi_singleton()
{
    std::lock_guard<std::mutex> guard(g_xsapiSingletonLock);
    g_xsapiSingleton = nullptr;
}

xsapi_singleton*
get_xsapi_singleton(_In_ bool createIfRequired)
{
    if (createIfRequired)
    {
        std::lock_guard<std::mutex> guard(g_xsapiSingletonLock);
        if (g_xsapiSingleton == nullptr)
        {
            g_xsapiSingleton = std::make_unique<xsapi_singleton>();
        }
    }

    return g_xsapiSingleton.get();
}

double
XSAPI_CALL
xbl_get_version()
{
    return 5.0f;
}
