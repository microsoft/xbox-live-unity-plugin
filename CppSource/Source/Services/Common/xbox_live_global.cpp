// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"

HC_API void HC_CALLING_CONV
XSAPIMemSetFunctions(
    _In_opt_ XSAPI_MEM_ALLOC_FUNC memAllocFunc,
    _In_opt_ XSAPI_MEM_FREE_FUNC memFreeFunc
    ) XSAPI_NOEXCEPT
{
    HCMemSetFunctions(memAllocFunc, memFreeFunc);
}

HC_API XSAPI_RESULT HC_CALLING_CONV
XSAPIMemGetFunctions(
    _Out_ XSAPI_MEM_ALLOC_FUNC* memAllocFunc,
    _Out_ XSAPI_MEM_FREE_FUNC* memFreeFunc
    ) XSAPI_NOEXCEPT
{
    return utils::xsapi_result_from_hc_result(HCMemGetFunctions(memAllocFunc, memFreeFunc));
}

XBL_API XSAPI_RESULT XBL_CALLING_CONV
XSAPIGlobalInitialize() XSAPI_NOEXCEPT
try
{
    auto singleton = get_xsapi_singleton(true);
    auto hcr = HCGlobalInitialize();
    if (hcr != HC_OK)
    {
        return utils::xsapi_result_from_hc_result(hcr);
    }
    singleton->m_threadPool->start_threads();

    return XSAPI_RESULT_OK;
}
CATCH_RETURN()

XBL_API void XBL_CALLING_CONV
XSAPIGlobalCleanup() XSAPI_NOEXCEPT
try
{
    cleanup_xsapi_singleton();
    HCGlobalCleanup();
}
CATCH_RETURN_WITH(;)