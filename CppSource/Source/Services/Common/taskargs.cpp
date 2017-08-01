// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#include "pch.h"
#include "taskargs.h"

void StoreTaskArgs(_In_ HC_TASK_HANDLE taskHandle, _In_ std::shared_ptr<xbl_args> args)
{
    get_xsapi_singleton()->m_taskArgMap[taskHandle] = args;
}

std::shared_ptr<xbl_args> GetTaskArgs(_In_ HC_TASK_HANDLE taskHandle)
{
    return get_xsapi_singleton()->m_taskArgMap[taskHandle];
}

void ClearTaskArgs(_In_ HC_TASK_HANDLE taskHandle)
{
    get_xsapi_singleton()->m_taskArgMap.erase(taskHandle);
}
