// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once
#include "pch.h"
#include "threadpool.h"
#include "asyncop.h"

struct xsapi_singleton
{
    xsapi_singleton();
    ~xsapi_singleton();

    std::mutex singletonLock;

    std::mutex asyncLock;
    std::queue<std::shared_ptr<xsapi_async_info>> asyncPendingQueue;
    std::map<XSAPI_ASYNC_HANDLE, std::shared_ptr<xsapi_async_info>> asyncMap;
    XSAPI_ASYNC_HANDLE asyncLastHandleId;

    std::unique_ptr<xbl_thread_pool> threadPool;
};

xsapi_singleton* get_xsapi_singleton(_In_ bool createIfRequired = true);
