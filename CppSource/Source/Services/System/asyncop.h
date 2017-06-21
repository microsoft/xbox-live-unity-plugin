// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once
#include "pch.h"
#include "xsapi/services_c.h"

struct xbl_args
{
    virtual ~xbl_args() {}
};

struct xsapi_async_info;

typedef void(* xbl_async_op_execute_routine)(
    _In_ const std::shared_ptr<xsapi_async_info>& info
    );

typedef bool(* xbl_async_op_result_routine)(
    _In_ const std::shared_ptr<xsapi_async_info>& info,
    _Out_ void* result,
    _In_ int size
    );

enum xsapi_async_state
{
    pending,
    processing,
    completed
};

struct xsapi_async_info
{
    void* completionRoutineContext;
    std::shared_ptr<xbl_args> args;
    xsapi_async_state state;
    XSAPI_ASYNC_HANDLE handleId;
    void* completionRoutine;
    xbl_async_op_execute_routine executeRoutine;
    xbl_async_op_result_routine resultRoutine;
};

void xbl_asyncop_set_info_in_new_handle( 
    _In_ std::shared_ptr<xsapi_async_info> info,
    _In_ void* completionRoutineContext,
    _In_ void* completionRoutine
    );

std::shared_ptr<xsapi_async_info> xbl_asyncop_get_next_async_op();
std::shared_ptr<xsapi_async_info> xbl_asyncop_get_async_op_from_handle(XSAPI_ASYNC_HANDLE handle);

template<typename T, typename T2> bool
xbl_asyncop_write_result(_In_ const std::shared_ptr<xsapi_async_info>& info, _Out_ void* result, _In_ int size)
{
    if (size != sizeof(T)) return false;
    auto args = std::dynamic_pointer_cast<T2>(info->args);
    *(T*)result = args->m_result;
    return true;
}

