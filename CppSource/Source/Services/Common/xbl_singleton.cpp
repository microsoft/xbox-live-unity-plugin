// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xbl_singleton.h"

static std::mutex g_xblSingletonLock;
static std::unique_ptr<xsapi_singleton> g_xblSingleton;

xsapi_singleton::xsapi_singleton()
{
    m_threadPool = std::make_unique<xbl_thread_pool>();
}

xsapi_singleton::~xsapi_singleton()
{
}

xsapi_singleton*
get_xsapi_singleton(_In_ bool createIfRequired)
{
    if (createIfRequired)
    {
        std::lock_guard<std::mutex> guard(g_xblSingletonLock);
        if (g_xblSingleton == nullptr)
        {
            g_xblSingleton = std::make_unique<xsapi_singleton>();
        }
    }

    return g_xblSingleton.get();
}

XBL_API void XBL_CALLING_CONV
XBLGlobalInitialize()
{
    get_xsapi_singleton(true);
    HCGlobalInitialize();
    get_xsapi_singleton()->m_threadPool->start_threads();
}

XBL_API void XBL_CALLING_CONV
XBLGlobalCleanup()
{
    std::lock_guard<std::mutex> guard(g_xblSingletonLock);
    g_xblSingleton = nullptr;

    HCGlobalCleanup();
}

void VerifyGlobalXsapiInit()
{
    if (g_xblSingleton == nullptr)
    {
        LOG_ERROR("Call HCGlobalInitialize() first");
        assert(g_xblSingleton != nullptr);
    }
}
