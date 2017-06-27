// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "xsapi/types.h"
#include "xsapi/services_c.h"
#include "singleton.h"

std::shared_ptr<xsapi_async_info>
xbl_asyncop_get_next_async_op()
{
    std::lock_guard<std::mutex> guard(get_xsapi_singleton()->asyncLock);
    auto& pendingQueue = get_xsapi_singleton()->asyncPendingQueue;
    if (!pendingQueue.empty())
    {
        auto it = pendingQueue.front();
        pendingQueue.pop();
        return it;
    }
    return nullptr;
}

std::shared_ptr<xsapi_async_info>
xbl_asyncop_get_async_op_from_handle(XSAPI_ASYNC_HANDLE handle)
{
    std::lock_guard<std::mutex> guard(get_xsapi_singleton()->asyncLock);

    auto& asyncMap = get_xsapi_singleton()->asyncMap;
    auto it = asyncMap.find(handle);
    if (it == asyncMap.end())
        return nullptr;
    else
        return it->second;
}

void xbl_asyncop_remove_handle_from_map(_In_ XSAPI_ASYNC_HANDLE handle)
{
    std::lock_guard<std::mutex> guard(get_xsapi_singleton()->asyncLock);
    auto& asyncMap = get_xsapi_singleton()->asyncMap;
    asyncMap.erase(handle);
}

void xbl_asyncop_set_info_in_new_handle(
    _In_ std::shared_ptr<xsapi_async_info> info,
    _In_ void* completionRoutineContext,
    _In_ void* completionRoutine
    )
{
    std::lock_guard<std::mutex> guard(get_xsapi_singleton()->asyncLock);
    auto& asyncQueue = get_xsapi_singleton()->asyncPendingQueue;
    info->state = xsapi_async_state::pending;
    info->handleId = ++get_xsapi_singleton()->asyncLastHandleId;
    info->completionRoutineContext = completionRoutineContext;
    info->completionRoutine = completionRoutine;
    asyncQueue.push(info);

    auto& asyncMap = get_xsapi_singleton()->asyncMap;
    asyncMap[info->handleId] = info;

    get_xsapi_singleton()->threadPool->set_async_op_ready();
}

void
xbl_thread_process_pending_async_op()
{
    std::shared_ptr<xsapi_async_info> info = xbl_asyncop_get_next_async_op();
    if (info == nullptr)
        return;

    info->state = xsapi_async_state::processing;
    info->executeRoutine(info);
    info->state = xsapi_async_state::completed;

    if (info->completionRoutine != nullptr)
    {
        xbl_asyncop_remove_handle_from_map(info->handleId);
    }
}

bool xbl_thread_async_op_get_result(_In_ XSAPI_ASYNC_HANDLE handle, _Out_ void* result, _In_ int size)
{
    auto& info = xbl_asyncop_get_async_op_from_handle(handle);
    if (info == nullptr) return false;
    bool returnCode = info->resultRoutine(info, result, size);
    xbl_asyncop_remove_handle_from_map(info->handleId);
    return returnCode;
}

bool
xbl_thread_is_async_op_done(_In_ XSAPI_ASYNC_HANDLE handle)
{
    auto& info = xbl_asyncop_get_async_op_from_handle(handle);
    if (info == nullptr) return true;

    return (info->state == completed);
}

