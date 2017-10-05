// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "xsapi/social_manager_c.h"
#include<time.h>

using namespace xbox::services::social::manager;

struct SocialManagerPresenceTitleRecordImpl
{
    SocialManagerPresenceTitleRecordImpl(
        _In_ social_manager_presence_title_record cppPresenceTitleRecord,
        _In_ SocialManagerPresenceTitleRecord* cPresenceTitleRecord
    ) : m_cPresenceTitleRecord(cPresenceTitleRecord), m_cppPresenceTitleRecord(cppPresenceTitleRecord)
    {
        m_isTitleActive = m_cppPresenceTitleRecord.is_title_active();
        m_cPresenceTitleRecord->isTitleActive = m_isTitleActive;

        m_deviceType = static_cast<PRESENCE_DEVICE_TYPE>(m_cppPresenceTitleRecord.device_type());
        m_cPresenceTitleRecord->deviceType = m_deviceType;

        m_isBroadcasting = m_cppPresenceTitleRecord.is_broadcasting();
        m_cPresenceTitleRecord->isBroadcasting = m_isBroadcasting;

        m_titleId = m_cppPresenceTitleRecord.title_id();
        m_cPresenceTitleRecord->titleId = m_titleId;

        m_presenceText = utils::to_utf8string(m_cppPresenceTitleRecord.presence_text());
        m_cPresenceTitleRecord->presenceText = m_presenceText.c_str();
    }

    bool m_isTitleActive;
    bool m_isBroadcasting;
    PRESENCE_DEVICE_TYPE m_deviceType;
    uint32 m_titleId;
    std::string m_presenceText;

    social_manager_presence_title_record m_cppPresenceTitleRecord;
    SocialManagerPresenceTitleRecord* m_cPresenceTitleRecord;
};

SocialManagerPresenceTitleRecord* CreateSocialManagerPresenceTitleRecordFromCpp(
    _In_ social_manager_presence_title_record cppPresenceTitleRecord
)
{
    auto cPresenceTitleRecord = new SocialManagerPresenceTitleRecord();
    cPresenceTitleRecord->pImpl = new SocialManagerPresenceTitleRecordImpl(cppPresenceTitleRecord, cPresenceTitleRecord);
        
    return cPresenceTitleRecord;
}


struct SocialManagerPresenceRecordImpl
{
    SocialManagerPresenceRecordImpl(
        _In_ const social_manager_presence_record cppPresenceRecord,
        _In_ SocialManagerPresenceRecord *cPresenceRecord
    ) : m_cppSocialManagerPresenceRecord(cppPresenceRecord), m_cSocialManagerPresenceRecord(cPresenceRecord)
    {
        Refresh();
    }

    void Refresh() 
    {
        if (m_cSocialManagerPresenceRecord != nullptr) 
        {
            m_presenceState = static_cast<USER_PRESENCE_STATE>(m_cppSocialManagerPresenceRecord.user_state());
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

    USER_PRESENCE_STATE m_presenceState;
    std::vector<SocialManagerPresenceTitleRecord *> m_titleRecords;

    const xbox::services::social::manager::social_manager_presence_record m_cppSocialManagerPresenceRecord;
    SocialManagerPresenceRecord* m_cSocialManagerPresenceRecord;
};

struct PreferredColorImpl
{
    PreferredColorImpl(
        _In_ preferred_color cppColor,
        _In_ PreferredColor* cColor
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
    PreferredColor* m_cColor;
};

struct XboxSocialUserImpl
{
    XboxSocialUserImpl(
        _In_ xbox_social_user* cppXboxSocialUser,
        _In_ XboxSocialUser* cXboxSocialUser
    ) : m_cXboxSocialUser(cXboxSocialUser), m_cppXboxSocialUser(cppXboxSocialUser)
    {
        m_xboxUserId = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->xbox_user_id()));
        m_cXboxSocialUser->xboxUserId = m_xboxUserId.c_str();

        m_isFavorite = m_cppXboxSocialUser->is_favorite();
        m_cXboxSocialUser->isFavorite = m_isFavorite;

        m_isFollowingUser = m_cppXboxSocialUser->is_following_user();
        m_cXboxSocialUser->isFollowingUser = m_isFollowingUser;

        m_isFollowedByCaller = m_cppXboxSocialUser->is_followed_by_caller();
        m_cXboxSocialUser->isFollowedByCaller = m_isFollowedByCaller;

        m_displayName = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->display_name()));
        m_cXboxSocialUser->displayName = m_displayName.c_str();

        m_realName = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->real_name()));
        m_cXboxSocialUser->realName = m_realName.c_str();

        m_displayPicUrlRaw = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->display_pic_url_raw()));
        m_cXboxSocialUser->displayPicUrlRaw = m_displayPicUrlRaw.c_str();

        m_useAvatar = m_cppXboxSocialUser->use_avatar();
        m_cXboxSocialUser->useAvatar = m_useAvatar;

        m_gamerscore = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->gamerscore()));
        m_cXboxSocialUser->gamerscore = m_gamerscore.c_str();

        m_gamertag = utils::to_utf8string(std::wstring(m_cppXboxSocialUser->gamertag()));
        m_cXboxSocialUser->gamertag = m_gamertag.c_str();

        m_presenceRecord = new SocialManagerPresenceRecord();
        m_presenceRecord->pImpl = new SocialManagerPresenceRecordImpl(m_cppXboxSocialUser->presence_record(), m_presenceRecord);
        m_cXboxSocialUser->presenceRecord = m_presenceRecord;

        auto cppTitleHistory = m_cppXboxSocialUser->title_history();
        m_titleHistory = new TitleHistory();
        m_titleHistory->userHasPlayed = cppTitleHistory.has_user_played();
        auto diffTime = utility::datetime().utc_now() - cppTitleHistory.last_time_user_played();
        time_t currentTime = time(NULL);
        currentTime -= diffTime;
        m_titleHistory->lastTimeUserPlayed = currentTime;
        m_cXboxSocialUser->titleHistory = m_titleHistory;

        m_preferredColor = new PreferredColor();
        m_preferredColor->pImpl = new PreferredColorImpl(m_cppXboxSocialUser->preferred_color(), m_preferredColor);
        m_cXboxSocialUser->preferredColor = m_preferredColor;
    }

    std::string m_xboxUserId;
    bool m_isFavorite;
    bool m_isFollowingUser;
    bool m_isFollowedByCaller;
    std::string m_displayName;
    std::string m_realName;
    std::string m_displayPicUrlRaw;
    bool m_useAvatar;
    std::string m_gamerscore;
    std::string m_gamertag;
    SocialManagerPresenceRecord* m_presenceRecord;
    TitleHistory* m_titleHistory;
    PreferredColor* m_preferredColor;

    xbox_social_user* m_cppXboxSocialUser;
    XboxSocialUser* m_cXboxSocialUser;
};

XboxSocialUser* CreateXboxSocialUserFromCpp(
    _In_ xbox::services::social::manager::xbox_social_user* cppXboxSocialUser
)
{
    auto cXboxSocialUser = new XboxSocialUser();
    cXboxSocialUser->pImpl = new XboxSocialUserImpl(cppXboxSocialUser, cXboxSocialUser);

    return cXboxSocialUser;
}

struct XboxUserIdContainerImpl
{
    XboxUserIdContainerImpl(
        _In_ xbox_user_id_container cppContainer,
        _In_ XboxUserIdContainer* cContainer
    ) : m_cContainer(cContainer), m_cppContainer(cppContainer)
    {
        m_xboxUserId = utils::to_utf8string(m_cppContainer.xbox_user_id());
        m_cContainer->xboxUserId = m_xboxUserId.c_str();
    }

    std::string m_xboxUserId;

    xbox_user_id_container m_cppContainer;
    XboxUserIdContainer* m_cContainer;
};

struct XboxSocialUserGroupImpl
{
    XboxSocialUserGroupImpl(
        _In_ std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> cppSocialUserGroup,
        _In_ XboxSocialUserGroup *cSocialUserGroup
    ) : m_cppSocialUserGroup(cppSocialUserGroup), m_cSocialUserGroup(cSocialUserGroup)
    {
        Init();
    }

    void Init() 
    {
        m_localUser = new XboxLiveUser();
        m_localUser->pImpl = new XboxLiveUserImpl(m_cppSocialUserGroup->local_user()->windows_system_user(), m_localUser);

        Refresh();
    }

    // Sets the c object's properties to the cpp object's properties
    void Refresh() 
    {
        if (m_cSocialUserGroup != nullptr) 
        {
            const std::vector<xbox::services::social::manager::xbox_social_user *> cppUsers = m_cppSocialUserGroup->users();
            m_users.clear();
            m_numOfUsers = cppUsers.size();
            for (auto cppUser : cppUsers) 
            {
                m_users.push_back(CreateXboxSocialUserFromCpp(cppUser));
            }
            m_cSocialUserGroup->users = m_users.data();
            m_cSocialUserGroup->numOfUsers = m_numOfUsers;

            m_socialUserGroupType = static_cast<SOCIAL_USER_GROUP_TYPE>(m_cppSocialUserGroup->social_user_group_type());
            m_cSocialUserGroup->socialUserGroupType = m_socialUserGroupType;

            auto cppTrackedUsers = m_cppSocialUserGroup->users_tracked_by_social_user_group();
            m_usersTrackedBySocialUserGroup.clear();
            for (auto cppUserIdContainer : cppTrackedUsers) 
            {
                auto cUserIdContainer = new XboxUserIdContainer();
                cUserIdContainer->pImpl = new XboxUserIdContainerImpl(cppUserIdContainer, cUserIdContainer);
                m_usersTrackedBySocialUserGroup.push_back(cUserIdContainer);
            }
            m_cSocialUserGroup->usersTrackedBySocialUserGroup = m_usersTrackedBySocialUserGroup.data();

            m_localUser->pImpl->Refresh();
            m_cSocialUserGroup->localUser = m_localUser;

            m_presenceFilterOfGroup = static_cast<PRESENCE_FILTER>(m_cppSocialUserGroup->presence_filter_of_group());
            m_cSocialUserGroup->presenceFilterOfGroup = m_presenceFilterOfGroup;

            m_relationshipFilterOfGroup = static_cast<RELATIONSHIP_FILTER>(m_cppSocialUserGroup->relationship_filter_of_group());
            m_cSocialUserGroup->relationshipFilterOfGroup = m_relationshipFilterOfGroup;
        }
    }

    std::vector<XboxSocialUser *> m_users;
    int m_numOfUsers;
    SOCIAL_USER_GROUP_TYPE m_socialUserGroupType;
    std::vector<XboxUserIdContainer *> m_usersTrackedBySocialUserGroup;
    XboxLiveUser* m_localUser;
    PRESENCE_FILTER m_presenceFilterOfGroup;
    RELATIONSHIP_FILTER m_relationshipFilterOfGroup;

    XboxSocialUserGroup* m_cSocialUserGroup;
    std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> m_cppSocialUserGroup;
};

SocialUserGroupLoadedEventArgs* CreateSocialUserGroupLoadedEventArgs(
    _In_ std::shared_ptr<xbox::services::social::manager::social_user_group_loaded_event_args> cppSocialUserGroupLoadedEventArgs,
    _In_ std::vector<XboxSocialUserGroup*> groups
)
{
    XboxSocialUserGroup* groupAffected = new XboxSocialUserGroup();
    for (auto group : groups) 
    {
        if (group->pImpl->m_cppSocialUserGroup == cppSocialUserGroupLoadedEventArgs->social_user_group()) {
            groupAffected = group;
        }
    }

    auto args = new SocialUserGroupLoadedEventArgs();
    args->socialUserGroup = groupAffected;

    return args;
}

struct SocialEventImpl {
    SocialEventImpl(
        _In_ social_event cppEvent,
        _In_ SocialEvent* cEvent,
        _In_ std::vector<XboxSocialUserGroup*> groups
    ) : m_cEvent(cEvent), m_cppEvent(cppEvent)
    {
        m_user = new XboxLiveUser();
        m_user->pImpl = new XboxLiveUserImpl(m_cppEvent.user(), m_user);
        m_cEvent->user = m_user;

        m_eventType = static_cast<SOCIAL_EVENT_TYPE>(m_cppEvent.event_type());
        m_cEvent->eventType = m_eventType;

        for (auto user : m_cppEvent.users_affected())
        {
            auto container = new XboxUserIdContainer();
            container->pImpl = new XboxUserIdContainerImpl(user, container);
            m_usersAffectedList.push_back(container);
        }

        m_usersAffected = m_usersAffectedList.data();
        m_cEvent->usersAffected = m_usersAffected;

        m_numOfUsersAffected = m_cppEvent.users_affected().size();
        m_cEvent->numOfUsersAffected = m_numOfUsersAffected;

        try
        {
            auto cSocialUserGroupLoadedEventArgs = std::dynamic_pointer_cast<xbox::services::social::manager::social_user_group_loaded_event_args>(m_cppEvent.event_args());
            if (cSocialUserGroupLoadedEventArgs != NULL)
            {
                m_eventArgs = CreateSocialUserGroupLoadedEventArgs(cSocialUserGroupLoadedEventArgs, groups);
                m_cEvent->eventArgs = m_eventArgs;
            }
        }
        catch (const std::exception&)
        {

        }

        m_err = m_cppEvent.err().value();
        m_cEvent->err = m_err;

        m_errMessage = m_cppEvent.err_message();
        m_cEvent->errMessage = m_errMessage.c_str();
    }

    XboxLiveUser* m_user;
    SOCIAL_EVENT_TYPE m_eventType;
    std::vector<XboxUserIdContainer *> m_usersAffectedList;
    XboxUserIdContainer** m_usersAffected;
    int m_numOfUsersAffected;
    SocialEventArgs* m_eventArgs;
    int m_err;
    std::string m_errMessage;

    social_event m_cppEvent;
    SocialEvent* m_cEvent;
};

SocialEvent* CreateSocialEventFromCpp(
    _In_ xbox::services::social::manager::social_event cppSocialEvent,
    _In_ std::vector<XboxSocialUserGroup*> groups
)
{
    auto cSocialEvent = new SocialEvent();
    cSocialEvent->pImpl = new SocialEventImpl(cppSocialEvent, cSocialEvent, groups);

    return cSocialEvent;
}