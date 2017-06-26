// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#pragma warning(disable: 4265)
#pragma warning(disable: 4266)
#pragma warning(disable: 4062)

#include <windows.h>
#include "types_c.h"
#include "title_callable_ui_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

// 
// Async Op APIs
// 

XSAPI_DLLEXPORT
bool
xbl_thread_is_async_op_done(_In_ XSAPI_ASYNC_HANDLE handle);

XSAPI_DLLEXPORT
bool
xbl_thread_async_op_get_result(_In_ XSAPI_ASYNC_HANDLE handle, _Out_ void* result, _In_ int size);

XSAPI_DLLEXPORT
void
xbl_thread_process_pending_async_op();

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)

