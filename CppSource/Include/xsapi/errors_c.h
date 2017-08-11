// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "types_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

typedef struct XboxLiveResult
{
    int errorCode;
    PCSTR_T errorMessage;
} XboxLiveResult;

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)