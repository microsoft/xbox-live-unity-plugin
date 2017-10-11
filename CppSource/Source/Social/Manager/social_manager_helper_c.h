// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "xsapi/social_manager_c.h"
#include<time.h>

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::social::manager;

struct XSAPI_SOCIAL_MANAGER_VARS
{
    public:
        std::vector<XSAPI_XBOX_SOCIAL_USER_GROUP*> cGroups;
        std::vector<XSAPI_SOCIAL_EVENT*> cEvents;
        xbox_live_result<void> cppVoidResult;
        xbox_live_result<std::shared_ptr<xbox_social_user_group>> cppGroupResult;
};

struct XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD_IMPL
{
    XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD_IMPL(
        _In_ social_manager_presence_title_record cppPresenceTitleRecord,
        _In_ XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD* cPresenceTitleRecord
    ) : m_cPresenceTitleRecord(cPresenceTitleRecord), m_cppPresenceTitleRecord(cppPresenceTitleRecord)
    {
        m_cPresenceTitleRecord->isTitleActive = m_cppPresenceTitleRecord.is_title_active();

        m_cPresenceTitleRecord->deviceType = static_cast<XSAPI_PRESENCE_DEVICE_TYPE>(m_cppPresenceTitleRecord.device_type());

        m_cPresenceTitleRecord->isBroadcasting = m_cppPresenceTitleRecord.is_broadcasting();

        m_cPresenceTitleRecord->titleId = m_cppPresenceTitleRecord.title_id();

        m_presenceText = utils::to_utf8string(m_cppPresenceTitleRecord.presence_text());
        m_cPresenceTitleRecord->presenceText = m_presenceText.c_str();
    }
    
    std::string m_presenceText;

    social_manager_presence_title_record m_cppPresenceTitleRecord;
    XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD* m_cPresenceTitleRecord;
};

XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD* CreateSocialManagerPresenceTitleRecordFromCpp(
    _In_ social_manager_presence_title_record cppPresenceTitleRecord
)
{
    auto cPresenceTitleRecord = new XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD();
    cPresenceTitleRecord->pImpl = new XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD_IMPL(cppPresenceTitleRecord, cPresenceTitleRecord);
        
    return cPresenceTitleRecord;
}


struct XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD_IMPL
{
    XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD_IMPL(
        _In_ const social_manager_presence_record cppPresenceRecord,
        _In_ XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD *cPresenceRecord
    ) : m_cppSocialManagerPresenceRecord(cppPresenceRecord), m_cSocialManagerPresenceRecord(cPresenceRecord)
    {
        Refresh();
    }

    void Refresh() 
    {
        if (m_cSocialManagerPresenceRecord != nullptr) 
        {
            m_presenceState = static_cast<XSAPI_USER_PRESENCE_STATE>(m_cppSocialManagerPresenceRecord.user_state());
            m_cSocialManagerPresenceRecord->userState = m_presenceState;

            m_titleRecords.clear();
            for (auto cppTitleRecord : m_cppSocialManagerPresenceRecord.presence_title_records()) 
            {
                m_titleRecords.push_back(CreateSocialManagerPresenceTitleRecordFromCpp(cppTitleRecord));
            }
            m_cSocialManagerPresenceRecord->presenceTitleRecords = m_titleRecords.data();
            m_cSocialManagerPresenceRecord->numOfPresenceTitleRecords = m_titleRecords.size();
        }
    }

    XSAPI_USER_PRESENCE_STATE m_presenceState;
    std::vector<XSAPI_SOCIAL_MANAGER_PRESENCE_TITLE_RECORD *> m_titleRecords;

    const xbox::services::social::manager::social_manager_presence_record m_cppSocialManagerPresenceRecord;
    XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD* m_cSocialManagerPresenceRecord;
};

struct XSAPI_PREFERRED_COLOR_IMPL
{
    XSAPI_PREFERRED_COLOR_IMPL(
        _In_ preferred_color cppColor,
        _In_ XSAPI_PREFERRED_COLOR* cColor
    ) : m_cColor(cColor), m_cppColor(cppColor)
    {
        m_primaryColor = utils::to_utf8string(m_cppColor.primary_color());
        m_cColor->primaryColor = m_primaryColor.c_str();

        m_secondaryColor = utils::to_utf8string(m_cppColor.secondary_color());
        m_cColor->secondaryColor = m_secondaryColor.c_str();

        m_tertiaryColor = utils::to_utf8string(m_cppColor.tertiary_color());
        m_cColor->tertiaryColor = m_tertiaryColor.c_str();
    }

    std::string m_primaryColor;
    std::string m_secondaryColor;
    std::string m_tertiaryColor;

    preferred_color m_cppColor;
    XSAPI_PREFERRED_COLOR* m_cColor;
};

struct XSAPI_XBOX_SOCIAL_USER_IMPL
{
    XSAPI_XBOX_SOCIAL_USER_IMPL(
        _In_ xbox_social_user* cppXboxSocialUser,
        _In_ XSAPI_XBOX_SOCIAL_USER* cXboxSocialUser
    ) : m_cXboxSocialUser(cXboxSocialUser), m_cppXboxSocialUser(cppXboxSocialUser)
    {
        m_xboxUserId = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->xbox_user_id()));
        m_cXboxSocialUser->xboxUserId = m_xboxUserId.c_str();

        m_cXboxSocialUser->isFavorite = m_cppXboxSocialUser->is_favorite();

        m_cXboxSocialUser->isFollowingUser = m_cppXboxSocialUser->is_following_user();

        m_cXboxSocialUser->isFollowedByCaller = m_cppXboxSocialUser->is_followed_by_caller();

        m_displayName = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->display_name()));
        m_cXboxSocialUser->displayName = m_displayName.c_str();

        m_realName = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->real_name()));
        m_cXboxSocialUser->realName = m_realName.c_str();

        m_displayPicUrlRaw = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->display_pic_url_raw()));
        m_cXboxSocialUser->displayPicUrlRaw = m_displayPicUrlRaw.c_str();

        m_cXboxSocialUser->useAvatar = m_cppXboxSocialUser->use_avatar();

        m_gamerscore = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->gamerscore()));
        m_cXboxSocialUser->gamerscore = m_gamerscore.c_str();

        m_gamertag = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->gamertag()));
        m_cXboxSocialUser->gamertag = m_gamertag.c_str();

        m_presenceRecord = new XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD();
        m_presenceRecord->pImpl = new XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD_IMPL(m_cppXboxSocialUser->presence_record(), m_presenceRecord);
        m_cXboxSocialUser->presenceRecord = m_presenceRecord;

        auto cppTitleHistory = m_cppXboxSocialUser->title_history();
        m_titleHistory = new XSAPI_TITLE_HISTORY();
        m_titleHistory->userHasPlayed = cppTitleHistory.has_user_played();
        auto diffTime = utility::datetime().utc_now() - cppTitleHistory.last_time_user_played();
        time_t currentTime = time(NULL);
        currentTime -= diffTime;
        m_titleHistory->lastTimeUserPlayed = currentTime;
        m_cXboxSocialUser->titleHistory = m_titleHistory;

        m_preferredColor = new XSAPI_PREFERRED_COLOR();
        m_preferredColor->pImpl = new XSAPI_PREFERRED_COLOR_IMPL(m_cppXboxSocialUser->preferred_color(), m_preferredColor);
        m_cXboxSocialUser->preferredColor = m_preferredColor;
    }

    std::string m_xboxUserId;
    std::string m_displayName;
    std::string m_realName;
    std::string m_displayPicUrlRaw;
    std::string m_gamerscore;
    std::string m_gamertag;
    XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD* m_presenceRecord;
    XSAPI_TITLE_HISTORY* m_titleHistory;
    XSAPI_PREFERRED_COLOR* m_preferredColor;

    xbox_social_user* m_cppXboxSocialUser;
    XSAPI_XBOX_SOCIAL_USER* m_cXboxSocialUser;
};

XSAPI_XBOX_SOCIAL_USER* CreateXboxSocialUserFromCpp(
    _In_ xbox::services::social::manager::xbox_social_user* cppXboxSocialUser
)
{
    auto cXboxSocialUser = new XSAPI_XBOX_SOCIAL_USER();
    cXboxSocialUser->pImpl = new XSAPI_XBOX_SOCIAL_USER_IMPL(cppXboxSocialUser, cXboxSocialUser);

    return cXboxSocialUser;
}

struct XSAPI_XBOX_USER_ID_CONTAINER_IMPL
{
    XSAPI_XBOX_USER_ID_CONTAINER_IMPL(
        _In_ xbox_user_id_container cppContainer,
        _In_ XSAPI_XBOX_USER_ID_CONTAINER* cContainer
    ) : m_cContainer(cContainer), m_cppContainer(cppContainer)
    {
        m_xboxUserId = utils::to_utf8string(m_cppContainer.xbox_user_id());
        m_cContainer->xboxUserId = m_xboxUserId.c_str();
    }

    std::string m_xboxUserId;

    xbox_user_id_container m_cppContainer;
    XSAPI_XBOX_USER_ID_CONTAINER* m_cContainer;
};

struct XSAPI_XBOX_SOCIAL_USER_GROUP_IMPL
{
    XSAPI_XBOX_SOCIAL_USER_GROUP_IMPL(
        _In_ std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> cppSocialUserGroup,
        _In_ XSAPI_XBOX_SOCIAL_USER_GROUP *cSocialUserGroup
    ) : m_cppSocialUserGroup(cppSocialUserGroup), m_cSocialUserGroup(cSocialUserGroup)
    {
        Init();
    }

    void Init() 
    {
        auto user = new XSAPI_XBOX_LIVE_USER();
        user->pImpl = new XSAPI_XBOX_LIVE_USER_IMPL(m_cppSocialUserGroup->local_user()->windows_system_user(), user);
        m_cSocialUserGroup->localUser = user;

        Refresh();
    }

    // Sets the c object's properties to the cpp object's properties
    void Refresh() 
    {
        if (m_cSocialUserGroup != nullptr) 
        {
            const std::vector<xbox::services::social::manager::xbox_social_user *> cppUsers = m_cppSocialUserGroup->users();
            m_users.clear();
            for (auto cppUser : cppUsers) 
            {
                m_users.push_back(CreateXboxSocialUserFromCpp(cppUser));
            }
            m_cSocialUserGroup->users = m_users.data();
            m_cSocialUserGroup->numOfUsers = cppUsers.size();

            m_cSocialUserGroup->socialUserGroupType = static_cast<XSAPI_SOCIAL_USER_GROUP_TYPE>(m_cppSocialUserGroup->social_user_group_type());

            auto cppTrackedUsers = m_cppSocialUserGroup->users_tracked_by_social_user_group();
            m_usersTrackedBySocialUserGroup.clear();
            for (auto cppUserIdContainer : cppTrackedUsers) 
            {
                auto cUserIdContainer = new XSAPI_XBOX_USER_ID_CONTAINER();
                cUserIdContainer->pImpl = new XSAPI_XBOX_USER_ID_CONTAINER_IMPL(cppUserIdContainer, cUserIdContainer);
                m_usersTrackedBySocialUserGroup.push_back(cUserIdContainer);
            }
            m_cSocialUserGroup->usersTrackedBySocialUserGroup = m_usersTrackedBySocialUserGroup.data();
            m_cSocialUserGroup->numOfUsersTrackedBySocialUserGroup = m_usersTrackedBySocialUserGroup.size();

            m_cSocialUserGroup->localUser->pImpl->Refresh();

            m_cSocialUserGroup->presenceFilterOfGroup = static_cast<XSAPI_PRESENCE_FILTER>(m_cppSocialUserGroup->presence_filter_of_group());

            m_cSocialUserGroup->relationshipFilterOfGroup = static_cast<XSAPI_RELATIONSHIP_FILTER>(m_cppSocialUserGroup->relationship_filter_of_group());
        }
    }

    std::vector<XSAPI_XBOX_SOCIAL_USER *> m_users;
    std::vector<XSAPI_XBOX_USER_ID_CONTAINER *> m_usersTrackedBySocialUserGroup;

    XSAPI_XBOX_SOCIAL_USER_GROUP* m_cSocialUserGroup;
    std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> m_cppSocialUserGroup;
};

XSAPI_SOCIAL_USER_GROUP_LOADED_EVENT_ARGS* CreateSocialUserGroupLoadedEventArgs(
    _In_ std::shared_ptr<xbox::services::social::manager::social_user_group_loaded_event_args> cppSocialUserGroupLoadedEventArgs,
    _In_ std::vector<XSAPI_XBOX_SOCIAL_USER_GROUP*> groups
)
{
    XSAPI_XBOX_SOCIAL_USER_GROUP* groupAffected = new XSAPI_XBOX_SOCIAL_USER_GROUP();
    for (auto group : groups) 
    {
        if (group->pImpl->m_cppSocialUserGroup == cppSocialUserGroupLoadedEventArgs->social_user_group()) {
            groupAffected = group;
        }
    }

    auto args = new XSAPI_SOCIAL_USER_GROUP_LOADED_EVENT_ARGS();
    args->socialUserGroup = groupAffected;

    return args;
}

struct XSAPI_SOCIAL_EVENT_IMPL {
    XSAPI_SOCIAL_EVENT_IMPL(
        _In_ social_event cppEvent,
        _In_ XSAPI_SOCIAL_EVENT* cEvent,
        _In_ std::vector<XSAPI_XBOX_SOCIAL_USER_GROUP*> groups
    ) : m_cEvent(cEvent), m_cppEvent(cppEvent)
    {
        auto user = new XSAPI_XBOX_LIVE_USER();
        user->pImpl = new XSAPI_XBOX_LIVE_USER_IMPL(m_cppEvent.user(), user);
        m_cEvent->user = user;

        m_cEvent->eventType = static_cast<XSAPI_SOCIAL_EVENT_TYPE>(m_cppEvent.event_type());

        for (auto user : m_cppEvent.users_affected())
        {
            auto container = new XSAPI_XBOX_USER_ID_CONTAINER();
            container->pImpl = new XSAPI_XBOX_USER_ID_CONTAINER_IMPL(user, container);
            m_usersAffectedList.push_back(container);
        }

        m_cEvent->usersAffected = m_usersAffectedList.data();

        m_cEvent->numOfUsersAffected = m_cppEvent.users_affected().size();

        try
        {
            auto cSocialUserGroupLoadedEventArgs = std::dynamic_pointer_cast<xbox::services::social::manager::social_user_group_loaded_event_args>(m_cppEvent.event_args());
            if (cSocialUserGroupLoadedEventArgs != NULL)
            {
                m_cEvent->eventArgs = CreateSocialUserGroupLoadedEventArgs(cSocialUserGroupLoadedEventArgs, groups);
            }
        }
        catch (const std::exception&)
        {
            // not a social_user_group_loaded_event_args
        }

        m_cEvent->err = m_cppEvent.err().value();

        m_errMessage = m_cppEvent.err_message();
        m_cEvent->errMessage = m_errMessage.c_str();
    }

    std::vector<XSAPI_XBOX_USER_ID_CONTAINER *> m_usersAffectedList;
    std::string m_errMessage;

    social_event m_cppEvent;
    XSAPI_SOCIAL_EVENT* m_cEvent;
};

XSAPI_SOCIAL_EVENT* CreateSocialEventFromCpp(
    _In_ xbox::services::social::manager::social_event cppSocialEvent,
    _In_ std::vector<XSAPI_XBOX_SOCIAL_USER_GROUP*> groups
)
{
    auto cSocialEvent = new XSAPI_SOCIAL_EVENT();
    cSocialEvent->pImpl = new XSAPI_SOCIAL_EVENT_IMPL(cppSocialEvent, cSocialEvent, groups);

    return cSocialEvent;
}