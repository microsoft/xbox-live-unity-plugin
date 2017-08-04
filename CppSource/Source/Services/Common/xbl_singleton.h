// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once
#include "threadpool.h"


struct xsapi_singleton
{
    xsapi_singleton();
    ~xsapi_singleton();

    std::mutex m_singletonLock;

    std::unique_ptr<xbl_thread_pool> m_threadPool;
    std::map<HC_TASK_HANDLE, std::shared_ptr<xbl_args>> m_taskArgMap;
};

xsapi_singleton* get_xsapi_singleton(_In_ bool createIfRequired = false);

void VerifyGlobalXsapiInit();
