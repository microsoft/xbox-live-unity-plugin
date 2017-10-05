// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "types_c.h"
#include "xsapi/errors_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

#if !XDK_API

typedef enum PRESENCE_DEVICE_TYPE
{
    UNKNOWN_PDT,
    WINDOWS_PHONE_PDT,
    WINDOWS_PHONE_7_PDT,
    WEB_PDT,
    XBOX_360_PDT,
    PC_PDT,
    WINDOWS_8_PDT,
    XBOX_ONE_PDT,
    WINDOWS_ONE_CORE_PDT,
    WINDOWS_ONE_CORE_MOBILE_PDT
} PRESENCE_DEVICE_TYPE;

typedef enum USER_PRESENCE_STATE 
{
    UNKNOWN_UPS,
    ONLINE_UPS,
    AWAY_UPS,
    OFFLINE_UPS
} USER_PRESENCE_STATE;

typedef enum PRESENCE_TITLE_VIEW_STATE 
{
    UNKNOWN_PRESENCE_TITLE_VIEW_STATE,
    FULL_SCREEN,
    FILLED,
    SNAPPED,
    BACKGROUND
} PRESENCE_TITLE_VIEW_STATE;

typedef enum PRESENCE_DETAIL_LEVEL 
{
    DEFAULT_LEVEL,
    USER,
    DEVICE,
    TITLE,
    ALL
} PRESENCE_DETAIL_LEVEL;

typedef enum PRESENCE_MEDIA_ID_TYPE 
{
    UNKNOWN_PRESENCE_MEDIA_ID_TYPE,
    BING,
    MEDIA_PROVIDER
} PRESENCE_MEDIA_ID_TYPE;

typedef enum TITLE_PRESENCE_STATE 
{
    UNKNOWN_TITLE_PRESENCE_STATE,
    STARTED,
    ENDED
} TITLE_PRESENCE_STATE;

#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)