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

SocialEvent* CreateSocialEventFromCpp(
    _In_ xbox::services::social::manager::social_event cppSocialEvent
)
{
    auto cSocialEvent = new SocialEvent();

    auto user = new XboxLiveUser();
    user->pImpl = new XboxLiveUserImpl(cppSocialEvent.user()->windows_system_user(), user);
    cSocialEvent->user = user;

    cSocialEvent->eventType = static_cast<SOCIAL_EVENT_TYPE>(cppSocialEvent.event_type());

    // todo

    // XboxUserIdContainer usersAffected;

    // cSocialEvent->eventArgs = cppSocialEvent.eventArgs;

    // cSocialEvent->err = static_cast<ERROR_TYPES>(m_cppSocialEvent->err);

    // cSocialEvent->err_message = cppSocialEvent.err_message().c_str();

    return cSocialEvent;
}

struct XboxSocialUserGroupImpl
{
    XboxSocialUserGroupImpl(
        _In_ std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> creationContext,
        _In_ XboxSocialUserGroup *cSocialUserGroup
    ) : m_cppSocialUserGroup(creationContext), m_cSocialUserGroup(cSocialUserGroup)
    {
    }

    void Refresh() {
        if (m_cSocialUserGroup != nullptr) {
            if (m_users != nullptr)
                free(m_users);

            const std::vector<xbox::services::social::manager::xbox_social_user *> users = m_cppSocialUserGroup->users();
            m_numOfUsers = users.size();
            m_users = (XboxSocialUser**)malloc(sizeof(XboxSocialUser*) * m_numOfUsers);
            for (int i = 0; i < users.size(); i++) {
                m_users[i] = CreateXboxSocialUserFromCpp(users[i]);
            }
            m_cSocialUserGroup->users = m_users;
            m_cSocialUserGroup->numOfUsers = m_numOfUsers;

            m_socialUserGroupType = static_cast<SOCIAL_USER_GROUP_TYPE>(m_cppSocialUserGroup->social_user_group_type());
            m_cSocialUserGroup->socialUserGroupType = m_socialUserGroupType;

            // todo: XboxSocialUser** m_usersTrackedBySocialUserGroup

            if (m_localUser == nullptr) {
                m_localUser = (XboxLiveUser *)malloc(sizeof(XboxLiveUser *));

                m_localUser->pImpl = new XboxLiveUserImpl(m_cppSocialUserGroup->local_user()->windows_system_user(), m_localUser);
            }
            m_localUser->pImpl->Refresh();
            m_cSocialUserGroup->localUser = m_localUser;

            m_presenceFilterOfGroup = static_cast<PRESENCE_FILTER>(m_cppSocialUserGroup->presence_filter_of_group());
            m_cSocialUserGroup->presenceFilterOfGroup = m_presenceFilterOfGroup;

            m_relationshipFilterOfGroup = static_cast<RELATIONSHIP_FILTER>(m_cppSocialUserGroup->relationship_filter_of_group());
            m_cSocialUserGroup->relationshipFilterOfGroup = m_relationshipFilterOfGroup;
        }
    }

    XboxSocialUser** m_users;
    int m_numOfUsers;
    SOCIAL_USER_GROUP_TYPE m_socialUserGroupType;
    XboxSocialUser** m_usersTrackedBySocialUserGroup;
    XboxLiveUser* m_localUser;
    PRESENCE_FILTER m_presenceFilterOfGroup;
    RELATIONSHIP_FILTER m_relationshipFilterOfGroup;

    XboxSocialUserGroup* m_cSocialUserGroup;
    std::shared_ptr<xbox::services::social::manager::xbox_social_user_group> m_cppSocialUserGroup;
};