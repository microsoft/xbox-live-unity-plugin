// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"

XBL_API XSAPI_RESULT XBL_CALLING_CONV
XBLGlobalInitialize() XSAPI_NOEXCEPT
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
XBLGlobalCleanup() XSAPI_NOEXCEPT
try
{
    cleanup_xsapi_singleton();
    HCGlobalCleanup();
}
CATCH_RETURN_WITH(;)