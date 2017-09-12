// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "pch.h"
#include "user_impl_c.h"

SocialManagerPresenceTitleRecord* CreateSocialManagerPresenceTitleRecordFromCpp(
    _In_ xbox::services::social::manager::social_manager_presence_title_record cppPresenceRecord
)
{
    auto cTitleRecord = new SocialManagerPresenceTitleRecord();

    cTitleRecord->isTitleActive = cppPresenceRecord.is_title_active();
    cTitleRecord->isBroadcasting = cppPresenceRecord.is_broadcasting();
    // todo: cTitleRecord->deviceType = static_cast<PresenceDeviceType>(cppPresenceRecord.device_type());
    cTitleRecord->titleId = cppPresenceRecord.title_id();
    cTitleRecord->presenceText = cppPresenceRecord.presence_text();

    return cTitleRecord;
}

SocialManagerPresenceRecord* CreateSocialManagerPresenceRecordFromCpp(
    _In_ const xbox::services::social::manager::social_manager_presence_record cppPresenceRecord
)
{
    auto cPresenceRecord = new SocialManagerPresenceRecord();

    auto records = cppPresenceRecord.presence_title_records();
    cPresenceRecord->numOfPresenceTitleRecords = records.size();
    cPresenceRecord->presenceTitleRecords = (SocialManagerPresenceTitleRecord **)malloc(sizeof(SocialManagerPresenceTitleRecord *) * cPresenceRecord->numOfPresenceTitleRecords);
    for (int i = 0; i < cPresenceRecord->numOfPresenceTitleRecords; i++)
    {
        cPresenceRecord->presenceTitleRecords[i] = CreateSocialManagerPresenceTitleRecordFromCpp(records[i]);
    }

    return cPresenceRecord;
}

XboxSocialUser* CreateXboxSocialUserFromCpp(
    _In_ xbox::services::social::manager::xbox_social_user* cppXboxSocialUser
)
{
    auto cXboxSocialUser = new XboxSocialUser();

    cXboxSocialUser->xboxUserId = cppXboxSocialUser->xbox_user_id();
    cXboxSocialUser->isFavorite = cppXboxSocialUser->is_favorite();
    cXboxSocialUser->isFollowingUser = cppXboxSocialUser->is_following_user();
    cXboxSocialUser->isFollowedByCaller = cppXboxSocialUser->is_followed_by_caller();
    cXboxSocialUser->displayName = cppXboxSocialUser->display_name();
    cXboxSocialUser->realName = cppXboxSocialUser->real_name();
    cXboxSocialUser->displayPicUrlRaw = cppXboxSocialUser->display_pic_url_raw();
    cXboxSocialUser->useAvatar = cppXboxSocialUser->use_avatar();
    cXboxSocialUser->gamerscore = cppXboxSocialUser->gamerscore();
    cXboxSocialUser->gamertag = cppXboxSocialUser->gamertag();
    cXboxSocialUser->presenceRecord = CreateSocialManagerPresenceRecordFromCpp(cppXboxSocialUser->presence_record());
    
    auto cppTitleHistory = cppXboxSocialUser->title_history();
    auto cTitleHistory = new TitleHistory();
    cTitleHistory->userHasPlayed = cppTitleHistory.has_user_played();
    // todo: cTitleHistory->lastTimeUserPlayed = cppTitleHistory.last_time_user_played();
    cXboxSocialUser->titleHistory = cTitleHistory;

    auto cppPreferredColor = cppXboxSocialUser->preferred_color();
    auto cPreferredColor = new PreferredColor();
    cPreferredColor->primaryColor = cppPreferredColor.primary_color();
    cPreferredColor->secondaryColor = cppPreferredColor.secondary_color();
    cPreferredColor->tertiaryColor = cppPreferredColor.tertiary_color();
    cXboxSocialUser->preferredColor = cPreferredColor;
    
    return cXboxSocialUser;
}

struct XboxSocialUserGroupImpl
{
    XboxSocialUserGroupImpl(
        _In_ std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> creationContext,
        _In_ XboxSocialUserGroup *cSocialUserGroup
    ) : m_cppSocialUserGroup(creationContext), m_cSocialUserGroup(cSocialUserGroup)
    {
        Init();
    }

    void Init() {
        m_localUser = new XboxLiveUser();
        m_localUser->pImpl = new XboxLiveUserImpl(m_cppSocialUserGroup->local_user()->windows_system_user(), m_localUser);

        Refresh();
    }

    // Sets the c object's properties to the cpp object's properties
    void Refresh() {
        if (m_cSocialUserGroup != nullptr) {
            const std::vector<xbox::services::social::manager::xbox_social_user *> cppUsers = m_cppSocialUserGroup->users();
            m_users.clear();
            m_numOfUsers = cppUsers.size();
            for (auto cppUser : cppUsers) {
                m_users.push_back(CreateXboxSocialUserFromCpp(cppUser));
            }
            m_cSocialUserGroup->users = m_users.data();
            m_cSocialUserGroup->numOfUsers = m_numOfUsers;

            m_socialUserGroupType = static_cast<SOCIAL_USER_GROUP_TYPE>(m_cppSocialUserGroup->social_user_group_type());
            m_cSocialUserGroup->socialUserGroupType = m_socialUserGroupType;

            auto cppTrackedUsers = m_cppSocialUserGroup->users_tracked_by_social_user_group();
            m_usersTrackedBySocialUserGroup.clear();
            for (auto cppUserIdContainer : cppTrackedUsers) {
                auto cUserIdContainer = new XboxUserIdContainer();
                cUserIdContainer->xboxUserId = cppUserIdContainer.xbox_user_id();
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
    for (auto group : groups) {
        if (group->pImpl->m_cppSocialUserGroup == cppSocialUserGroupLoadedEventArgs->social_user_group()) {
            groupAffected = group;
        }
    }

    // Should check if social user group already exists

    //auto socialUserGroup = new XboxSocialUserGroup();
    //socialUserGroup->pImpl = new XboxSocialUserGroupImpl(cppSocialUserGroupLoadedEventArgs->social_user_group(), socialUserGroup);

    auto args = new SocialUserGroupLoadedEventArgs();
    args->socialUserGroup = groupAffected;

    return args;
}

// Kept here so it isn't garbage collected before managed code can read it
std::vector<XboxUserIdContainer *> mUsersAffected;
SocialEvent* CreateSocialEventFromCpp(
    _In_ xbox::services::social::manager::social_event cppSocialEvent,
    _In_ std::vector<XboxSocialUserGroup*> groups
)
{
    auto cSocialEvent = new SocialEvent();

    auto user = new XboxLiveUser();
    user->pImpl = new XboxLiveUserImpl(cppSocialEvent.user(), user);
    cSocialEvent->user = user;

    cSocialEvent->eventType = static_cast<SOCIAL_EVENT_TYPE>(cppSocialEvent.event_type());

    mUsersAffected.clear();
    for (auto user : cppSocialEvent.users_affected())
    {
        auto container = new XboxUserIdContainer();
        container->xboxUserId = user.xbox_user_id();
        mUsersAffected.push_back(container);
    }
    cSocialEvent->usersAffected = mUsersAffected.data();
    cSocialEvent->numOfUsersAffected = cppSocialEvent.users_affected().size();


    // todo // review // cSocialEvent->eventArgs = cppSocialEvent.event_args();
    // There's got to be a way to check if the downcast worked and create the event args that way vs checking for specific types
    try
    {
        auto cSocialUserGroupLoadedEventArgs = std::dynamic_pointer_cast<xbox::services::social::manager::social_user_group_loaded_event_args>(cppSocialEvent.event_args());
        if (cSocialUserGroupLoadedEventArgs != NULL)
            cSocialEvent->eventArgs = CreateSocialUserGroupLoadedEventArgs(cSocialUserGroupLoadedEventArgs, groups);
    }
    catch (const std::exception&)
    {

    }

    // todo cSocialEvent->err = static_cast<ERROR_TYPES>(m_cppSocialEvent->err());

    cSocialEvent->err_message = std::wstring (cppSocialEvent.err_message().begin(), cppSocialEvent.err_message().end()).data();

    return cSocialEvent;
}