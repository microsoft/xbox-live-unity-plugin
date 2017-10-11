// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "types_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

typedef enum XSAPI_RESULT
{
    XSAPI_RESULT_OK = 0,

    //////////////////////////////////////////////////////////////////////////
    // LibHttpClient errors
    //////////////////////////////////////////////////////////////////////////
    XSAPI_RESULT_E_HC_FAIL = -1,
    XSAPI_RESULT_E_HC_POINTER = -2,
    XSAPI_RESULT_E_HC_INVALIDARG = -3,
    XSAPI_RESULT_E_HC_OUTOFMEMORY = -4,
    XSAPI_RESULT_E_HC_BUFFERTOOSMALL = -5,
    XSAPI_RESULT_E_HC_NOTINITIALIZED = -6,
    XSAPI_RESULT_E_HC_FEATURENOTPRESENT = -7,

    XSAPI_RESULT_E_HC_MIN = XSAPI_RESULT_E_HC_FEATURENOTPRESENT,
    XSAPI_RESULT_E_HC_MAX = XSAPI_RESULT_E_HC_FAIL,

    //////////////////////////////////////////////////////////////////////////
    // XSAPI Core error conditions
    //////////////////////////////////////////////////////////////////////////
    XSAPI_RESULT_E_GENERIC_ERROR = 1,
    XSAPI_RESULT_E_OUT_OF_RANGE = 2,
    XSAPI_RESULT_E_AUTH = 3,
    XSAPI_RESULT_E_NETWORK = 4,
    XSAPI_RESULT_E_HTTP = 5,
    XSAPI_RESULT_E_HTTP_404_NOT_FOUND = 6,
    XSAPI_RESULT_E_HTTP_412_PRECONDITION_FAILED = 7,
    XSAPI_RESULT_E_HTTP_429_TOO_MANY_REQUESTS = 8,
    XSAPI_RESULT_E_HTTP_SERVICE_TIMEOUT = 9,
    XSAPI_RESULT_E_RTA = 10

} XSAPI_RESULT;

typedef struct XSAPI_RESULT_INFO
{
    XSAPI_RESULT errorCode;
    PCSTR errorMessage;
} XSAPI_RESULT_INFO;

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)