// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "types_c.h"
#include "xsapi/errors_c.h"
#include "xsapi/system_c.h"
#include "xsapi/presence_c.h"

#if defined(__cplusplus)
extern "C" {
#endif

#if !XDK_API
	
struct XboxSocialUserGroupImpl;

typedef enum SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL {
	NO_EXTRA_DETAIL,
	TITLE_HISTORY_LEVEL = 0x1,
	PREFERRED_COLOR_LEVEL = 0x2,
} SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL;

typedef enum PRESENCE_FILTER {
	UNKNOWN_PRESENCE,
	TITLE_ONLINE,
	TITLE_OFFLINE,
	ALL_ONLINE,
	ALL_OFFLINE,
	ALL_TITLE,
	ALL_PF
} PRESENCE_FILTER;

typedef enum SOCIAL_EVENT_TYPE {
	USERS_ADDED_TO_SOCIAL_GRAPH,
	USERS_REMOVED_FROM_SOCIAL_GRAPH,
	PRESENCE_CHANGED,
	PROFILES_CHANGED,
	SOCIAL_RELATIONSHIPS_CHANGED,
	LOCAL_USER_ADDED,
	LOCAL_USER_REMOVED,
	SOCIAL_USER_GROUP_LOADED,
	SOCIAL_USER_GROUP_UPDATED,
	UNKNOWN_EVENT
} SOCIAL_EVENT_TYPE;

typedef enum RELATIONSHIP_FILTER {
	FRIENDS,
	FAVORITE
} RELATIONSHIP_FILTER;

typedef enum SOCIAL_USER_GROUP_TYPE {
	FILTER_TYPE,
	USER_LIST_TYPE
} SOCIAL_USER_GROUP_TYPE;

typedef struct TitleHistory
{
    bool userHasPlayed;
    time_t lastTimeUserPlayed;
} TitleHistory;

typedef struct PreferredColor
{
    PCSTR_T primaryColor;
    PCSTR_T secondaryColor;
    PCSTR_T tertiaryColor;
} PreferredColor;

typedef struct SocialManagerPresenceTitleRecord
{
    bool isTitleActive;
    bool isBroadcasting;
    PRESENCE_DEVICE_TYPE deviceType;
    uint32 titleId;
    PCSTR_T presenceText;
} SocialManagerPresenceTitleRecord;

typedef struct SocialManagerPresenceRecord
{
    USER_PRESENCE_STATE userState;
    SocialManagerPresenceTitleRecord** presenceTitleRecords;
    int numOfPresenceTitleRecords;
    /// todo: is_user_playing_title
} SocialManagerPresenceRecord;

typedef struct XboxSocialUser
{
    PCSTR_T xboxUserId;
    bool isFavorite;
    bool isFollowingUser;
    bool isFollowedByCaller;
    PCSTR_T displayName;
    PCSTR_T realName;
    PCSTR_T displayPicUrlRaw;
    bool useAvatar;
    PCSTR_T gamerscore;
    PCSTR_T gamertag;
    SocialManagerPresenceRecord *presenceRecord;
    TitleHistory *titleHistory;
    PreferredColor *preferredColor;
} XboxSocialUser;

typedef struct SocialEventArgs {

} SocialEventArgs;

typedef struct XboxUserIdContainer {
    PCSTR_T xboxUserId;
} XboxUserIdContainer;

typedef struct SocialEvent
{
    XboxLiveUser* user;
    SOCIAL_EVENT_TYPE eventType;
    XboxUserIdContainer** usersAffected;
    int numOfUsersAffected;
    SocialEventArgs* eventArgs;
    int err;
    PCSTR_T err_message;
} SocialEvent;

typedef struct XboxSocialUserGroup
{
	XboxSocialUser** users;
    int numOfUsers;
	SOCIAL_USER_GROUP_TYPE socialUserGroupType;
	XboxUserIdContainer** usersTrackedBySocialUserGroup;
    int numOfUsersTrackedBySocialUserGroup;
    XboxLiveUser* localUser;
	PRESENCE_FILTER presenceFilterOfGroup;
	RELATIONSHIP_FILTER relationshipFilterOfGroup;

	XboxSocialUserGroupImpl *pImpl;
} XboxSocialUserGroup;
// todo get_copy_of_users
// todo get_users_from_xbox_user_ids

typedef struct SocialUserGroupLoadedEventArgs : SocialEventArgs {
    XboxSocialUserGroup* socialUserGroup;
} SocialUserGroupLoadedEventArgs;

typedef struct SocialManager
{
	PCSTR_T localUsers;
} SocialManager;

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerAddLocalUser(
	_In_ XboxLiveUser *user,
	_In_ SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL extraLevelDetail
	);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerRemoveLocalUser(
	_In_ XboxLiveUser *user
	);

XSAPI_DLLEXPORT SocialEvent** XBL_CALLING_CONV
SocialManagerDoWork(
    _Inout_ int* numOfEvents
    );

XSAPI_DLLEXPORT XboxSocialUserGroup* XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromFilters(
	_In_ XboxLiveUser *user,
	_In_ PRESENCE_FILTER presenceDetailLevel,
	_In_ RELATIONSHIP_FILTER filter
	);

XSAPI_DLLEXPORT XboxSocialUserGroup* XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromList(
    _In_ XboxLiveUser *user,
    _In_ PCSTR_T* xboxUserIdList,
    _In_ int numOfXboxUserIds
	);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerDestroySocialUserGroup(
    _In_ XboxSocialUserGroup *group
    );

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerUpdateSocialUserGroup(
    _In_ XboxSocialUserGroup *group,
    _In_ PCSTR_T* users,
    _In_ int numOfUsers
	);

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerSetRichPresencePollingStatus(
	_In_ XboxLiveUser *user,
	_In_ bool shouldEnablePolling
	);
#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)