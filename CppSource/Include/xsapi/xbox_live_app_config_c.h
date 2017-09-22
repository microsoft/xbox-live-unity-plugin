// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include <stdint.h>
#include "types_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

typedef struct XboxLiveAppConfig
{
    uint32_t titleId;
    PCSTR scid;
    PCSTR environment;
    PCSTR sandbox;
} XboxLiveAppConfig;

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
GetXboxLiveAppConfigSingleton(
    _Out_ XboxLiveAppConfig const** ppConfig
    );

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)