// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once
#include "xsapi/social_manager_c.h"
#include<time.h>

SocialManagerPresenceTitleRecord* CreateSocialManagerPresenceTitleRecordFromCpp(
    _In_ xbox::services::social::manager::social_manager_presence_title_record cppPresenceRecord
)
{
    auto cTitleRecord = new SocialManagerPresenceTitleRecord();

    cTitleRecord->isTitleActive = cppPresenceRecord.is_title_active();
    cTitleRecord->isBroadcasting = cppPresenceRecord.is_broadcasting();
    cTitleRecord->deviceType = static_cast<PRESENCE_DEVICE_TYPE>(cppPresenceRecord.device_type());
    cTitleRecord->titleId = cppPresenceRecord.title_id();
    cTitleRecord->presenceText = utils::to_utf8string(cppPresenceRecord.presence_text()).c_str();

    return cTitleRecord;
}


struct SocialManagerPresenceRecordImpl
{
    SocialManagerPresenceRecordImpl(
        _In_ const xbox::services::social::manager::social_manager_presence_record creationContext,
        _In_ SocialManagerPresenceRecord *cSocialUserGroup
    ) : m_cppSocialManagerPresenceRecord(creationContext), m_cSocialManagerPresenceRecord(cSocialUserGroup)
    {
        Refresh();
    }

    void Refresh() {
        if (m_cSocialManagerPresenceRecord != nullptr) {
            m_presenceState = static_cast<USER_PRESENCE_STATE>(m_cppSocialManagerPresenceRecord.user_state());
            m_cSocialManagerPresenceRecord->userState = m_presenceState;

            m_titleRecords.clear();
            for (auto cppTitleRecord : m_cppSocialManagerPresenceRecord.presence_title_records()) {
                m_titleRecords.push_back(CreateSocialManagerPresenceTitleRecordFromCpp(cppTitleRecord));
            }
            m_cSocialManagerPresenceRecord->presenceTitleRecords = m_titleRecords.data();
        }
    }

    USER_PRESENCE_STATE m_presenceState;
    std::vector<SocialManagerPresenceTitleRecord *> m_titleRecords;
    const xbox::services::social::manager::social_manager_presence_record m_cppSocialManagerPresenceRecord;
    SocialManagerPresenceRecord* m_cSocialManagerPresenceRecord;
};

XboxSocialUser* CreateXboxSocialUserFromCpp(
    _In_ xbox::services::social::manager::xbox_social_user* cppXboxSocialUser
)
{
    auto cXboxSocialUser = new XboxSocialUser();

    cXboxSocialUser->xboxUserId = utils::to_utf8string(std::wstring(cppXboxSocialUser->xbox_user_id())).c_str();
    cXboxSocialUser->isFavorite = cppXboxSocialUser->is_favorite();
    cXboxSocialUser->isFollowingUser = cppXboxSocialUser->is_following_user();
    cXboxSocialUser->isFollowedByCaller = cppXboxSocialUser->is_followed_by_caller();
    cXboxSocialUser->displayName = utils::to_utf8string(std::wstring(cppXboxSocialUser->display_name())).c_str();
    cXboxSocialUser->realName = utils::to_utf8string(std::wstring(cppXboxSocialUser->real_name())).c_str();
    cXboxSocialUser->displayPicUrlRaw = utils::to_utf8string(std::wstring(cppXboxSocialUser->display_pic_url_raw())).c_str();
    cXboxSocialUser->useAvatar = cppXboxSocialUser->use_avatar();
    cXboxSocialUser->gamerscore = utils::to_utf8string(std::wstring(cppXboxSocialUser->gamerscore())).c_str();
    cXboxSocialUser->gamertag = utils::to_utf8string(std::wstring(cppXboxSocialUser->gamertag())).c_str();

    auto socialManagerPresenceRecord = new SocialManagerPresenceRecord();
    socialManagerPresenceRecord->pImpl = new SocialManagerPresenceRecordImpl(cppXboxSocialUser->presence_record(), socialManagerPresenceRecord);
    cXboxSocialUser->presenceRecord = socialManagerPresenceRecord;
    
    auto cppTitleHistory = cppXboxSocialUser->title_history();
    auto cTitleHistory = new TitleHistory();
    cTitleHistory->userHasPlayed = cppTitleHistory.has_user_played();
    auto diffTime = utility::datetime().utc_now() - cppTitleHistory.last_time_user_played();
    time_t currentTime = time(NULL);
    currentTime -= diffTime;
    cTitleHistory->lastTimeUserPlayed = currentTime;
    cXboxSocialUser->titleHistory = cTitleHistory;

    auto cppPreferredColor = cppXboxSocialUser->preferred_color();
    auto cPreferredColor = new PreferredColor();
    cPreferredColor->primaryColor = utils::to_utf8string(cppPreferredColor.primary_color()).c_str();
    cPreferredColor->secondaryColor = utils::to_utf8string(cppPreferredColor.secondary_color()).c_str();
    cPreferredColor->tertiaryColor = utils::to_utf8string(cppPreferredColor.tertiary_color()).c_str();
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
                cUserIdContainer->xboxUserId = utils::to_utf8string(cppUserIdContainer.xbox_user_id()).c_str();
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
        container->xboxUserId = utils::to_utf8string(user.xbox_user_id()).data();
        mUsersAffected.push_back(container);
    }
    cSocialEvent->usersAffected = mUsersAffected.data();
    cSocialEvent->numOfUsersAffected = cppSocialEvent.users_affected().size();

    try
    {
        auto cSocialUserGroupLoadedEventArgs = std::dynamic_pointer_cast<xbox::services::social::manager::social_user_group_loaded_event_args>(cppSocialEvent.event_args());
        if (cSocialUserGroupLoadedEventArgs != NULL)
            cSocialEvent->eventArgs = CreateSocialUserGroupLoadedEventArgs(cSocialUserGroupLoadedEventArgs, groups);
    }
    catch (const std::exception&)
    {

    }

    // todo: unsure if this is correct
    cSocialEvent->err = cppSocialEvent.err().value();

    cSocialEvent->err_message = cppSocialEvent.err_message().data();

    return cSocialEvent;
}