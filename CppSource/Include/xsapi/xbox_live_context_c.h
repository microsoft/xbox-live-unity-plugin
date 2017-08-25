// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "types_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

struct XboxLiveUser;
struct XboxLiveAppConfig;
struct XboxLiveContextImpl;

typedef struct XboxLiveContext
{
    PCSTR_T xboxUserId;

#if XDK_API | XBOX_UWP
    Windows::Xbox::System::User^ user;
#else
    XboxLiveUser *user;
#endif

    const XboxLiveAppConfig *appConfig;

    // TODO add services

    XboxLiveContextImpl *pImpl;

} XboxLiveContext;

#if !(XDK_API | XBOX_UWP)

XSAPI_DLLEXPORT XboxLiveContext* XBL_CALLING_CONV
XboxLiveContextCreate(
    XboxLiveUser *user
    );

#endif

XSAPI_DLLEXPORT void XBL_CALLING_CONV
XboxLiveContextDelete(
    XboxLiveContext *context
    );

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)