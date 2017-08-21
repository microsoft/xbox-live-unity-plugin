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
    PCSTR_T scid;
    PCSTR_T environment;
    PCSTR_T sandbox;
} XboxLiveAppConfig;

XSAPI_DLLEXPORT const XboxLiveAppConfig* XBL_CALLING_CONV
GetXboxLiveAppConfigSingleton();

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)