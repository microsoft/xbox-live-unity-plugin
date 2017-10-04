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
	
struct PreferredColorImpl;
struct SocialManagerPresenceTitleRecordImpl;
struct XboxUserIdContainerImpl;
struct SocialEventImpl;
struct XboxSocialUserImpl;
struct XboxSocialUserGroupImpl;
struct SocialManagerPresenceRecordImpl;

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
	USERS_ADDED_TO_SOCIAL_GRAPH_SOCIAL,
	USERS_REMOVED_FROM_SOCIAL_GRAPH_SOCIAL,
	PRESENCE_CHANGED_SOCIAL,
	PROFILES_CHANGED_SOCIAL,
	SOCIAL_RELATIONSHIPS_CHANGED_SOCIAL,
	LOCAL_USER_ADDED_SOCIAL,
	LOCAL_USER_REMOVED_SOCIAL,
	SOCIAL_USER_GROUP_LOADED_SOCIAL,
	SOCIAL_USER_GROUP_UPDATED_SOCIAL,
	UNKNOWN_EVENT_SOCIAL
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
    PCSTR primaryColor;
    PCSTR secondaryColor;
    PCSTR tertiaryColor;

    PreferredColorImpl* pImpl;
} PreferredColor;

typedef struct SocialManagerPresenceTitleRecord
{
    bool isTitleActive;
    bool isBroadcasting;
    PRESENCE_DEVICE_TYPE deviceType;
    uint32 titleId;
    PCSTR presenceText;

    SocialManagerPresenceTitleRecordImpl* pImpl;
} SocialManagerPresenceTitleRecord;

typedef struct SocialManagerPresenceRecord
{
    USER_PRESENCE_STATE userState;
    SocialManagerPresenceTitleRecord** presenceTitleRecords;
    int numOfPresenceTitleRecords;

    SocialManagerPresenceRecordImpl * pImpl;
} SocialManagerPresenceRecord;

XSAPI_DLLEXPORT bool XBL_CALLING_CONV
SocialManagerPresenceRecordIsUserPlayingTitle(
    _In_ SocialManagerPresenceRecord* presenceRecord,
    _In_ uint32_t titleId
    );

typedef struct XboxSocialUser
{
    PCSTR xboxUserId;
    bool isFavorite;
    bool isFollowingUser;
    bool isFollowedByCaller;
    PCSTR displayName;
    PCSTR realName;
    PCSTR displayPicUrlRaw;
    bool useAvatar;
    PCSTR gamerscore;
    PCSTR gamertag;
    SocialManagerPresenceRecord *presenceRecord;
    TitleHistory *titleHistory;
    PreferredColor *preferredColor;

    XboxSocialUserImpl* pImpl;
} XboxSocialUser;

typedef struct SocialEventArgs {

} SocialEventArgs;

typedef struct XboxUserIdContainer {
    PCSTR xboxUserId;

    XboxUserIdContainerImpl* pImpl;
} XboxUserIdContainer;

typedef struct SocialEvent
{
    XboxLiveUser* user;
    SOCIAL_EVENT_TYPE eventType;
    XboxUserIdContainer** usersAffected;
    int numOfUsersAffected;
    SocialEventArgs* eventArgs;
    int err;
    PCSTR errMessage;

    SocialEventImpl* pImpl;
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
    PCSTR localUsers;
} SocialManager;

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerAddLocalUser(
	_In_ XboxLiveUser *user,
	_In_ SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL extraLevelDetail,
    _Out_ PCSTR* errMessage
	);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerRemoveLocalUser(
	_In_ XboxLiveUser *user,
    _Out_ PCSTR* errMessage
	);

XSAPI_DLLEXPORT SocialEvent** XBL_CALLING_CONV
SocialManagerDoWork(
    _Out_ int32* numOfEvents
    );

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromFilters(
	_In_ XboxLiveUser *user,
	_In_ PRESENCE_FILTER presenceDetailLevel,
	_In_ RELATIONSHIP_FILTER filter,
    _Out_ XboxSocialUserGroup** group,
    _Out_ PCSTR* errMessage
	);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromList(
    _In_ XboxLiveUser *user,
    _In_ PCSTR* xboxUserIdList,
    _In_ int numOfXboxUserIds,
    _Out_ XboxSocialUserGroup** group,
    _Out_ PCSTR* errMessage
	);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerDestroySocialUserGroup(
    _In_ XboxSocialUserGroup *group,
    _Out_ PCSTR* errMessage
    );

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerUpdateSocialUserGroup(
    _In_ XboxSocialUserGroup *group,
    _In_ PCSTR* users,
    _In_ int numOfUsers,
    _Out_ PCSTR* errMessage
	);

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerSetRichPresencePollingStatus(
	_In_ XboxLiveUser *user,
	_In_ bool shouldEnablePolling,
    _Out_ PCSTR* errMessage
	);
#endif //!XDK_API

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)