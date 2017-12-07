// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "user_impl.h"
#include "social_manager_helper.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::social::manager;

// todo - move global variabes into a pimpl and store it in the xsapi singleton
// Kept here so it isn't garbage collected before managed code can read it
XSAPI_SOCIAL_MANAGER_VARS socialVars;

XSAPI_DLLEXPORT bool XBL_CALLING_CONV
SocialManagerPresenceRecordIsUserPlayingTitle(
    _In_ XSAPI_SOCIAL_MANAGER_PRESENCE_RECORD* presenceRecord,
    _In_ uint32_t titleId
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    return presenceRecord->pImpl->cppSocialManagerPresenceRecord().is_user_playing_title(titleId);
}
CATCH_RETURN_WITH(false);

XSAPI_DLLEXPORT XSAPI_XBOX_SOCIAL_USER** XBL_CALLING_CONV
XboxSocialUserGroupGetUsersFromXboxUserIds(
    _In_ XSAPI_XBOX_SOCIAL_USER_GROUP* group,
    _In_ PCSTR* xboxUserIds,
    _In_ uint32_t xboxUserIdsCount,
    _Out_ uint32_t* xboxSocialUsersCount
    )
try
{
    verify_global_init();

    std::vector<xbox_user_id_container> cXboxUserIdContainers = std::vector<xbox_user_id_container>(xboxUserIdsCount);
    std::vector<std::wstring> cXboxUserIds = std::vector<std::wstring>(xboxUserIdsCount);

    for (size_t i = 0; i < xboxUserIdsCount; i++)
    {
        cXboxUserIds[i] = utils::to_utf16string(xboxUserIds[i]);
        cXboxUserIdContainers[i] = xbox_user_id_container(cXboxUserIds[i].c_str());
    }

    auto cppXboxSocialUsers = group->pImpl->cppSocialUserGroup()->get_users_from_xbox_user_ids(cXboxUserIdContainers);
    *xboxSocialUsersCount = (uint32_t)cppXboxSocialUsers.size();

    socialVars.cXboxSocialUserGroupUsers = std::vector<XSAPI_XBOX_SOCIAL_USER*>(*xboxSocialUsersCount);
    for (uint32_t i = 0; i < *xboxSocialUsersCount; i++)
    {
        socialVars.cXboxSocialUserGroupUsers[i] = CreateXboxSocialUserFromCpp(cppXboxSocialUsers[i]);
    }
    
    return socialVars.cXboxSocialUserGroupUsers.data();
}
CATCH_RETURN_WITH(nullptr)

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerAddLocalUser(
    _In_ XSAPI_XBOX_LIVE_USER *user,
    _In_ XSAPI_SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL extraLevelDetail,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    social_manager_extra_detail_level cExtraLevelDetail = static_cast<social_manager_extra_detail_level>(extraLevelDetail);
    socialVars.cppVoidResult = social_manager::get_singleton_instance()->add_local_user(user->pImpl->cppUser(), cExtraLevelDetail);

    *errMessage = socialVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerRemoveLocalUser(
    _In_ XSAPI_XBOX_LIVE_USER *user,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    socialVars.cppVoidResult = social_manager::get_singleton_instance()->remove_local_user(user->pImpl->cppUser());

    *errMessage = socialVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppVoidResult.err());
}
CATCH_RETURN()


XSAPI_DLLEXPORT XSAPI_SOCIAL_EVENT** XBL_CALLING_CONV
SocialManagerDoWork(
    _Out_ uint32_t* socialEventsCount
    )
try
{
    verify_global_init();

    std::vector<social_event> cppSocialEvents = social_manager::get_singleton_instance()->do_work();
    
    socialVars.cEvents.clear();

    if (cppSocialEvents.size() > 0) 
    {
        for (auto cEvent : cppSocialEvents) 
        {
            socialVars.cEvents.push_back(CreateSocialEventFromCpp(cEvent, socialVars.cGroups));
        }

        for (auto socialUserGroup : socialVars.cGroups) 
        {
            if (socialUserGroup != nullptr) 
            {
                socialUserGroup->pImpl->Refresh();
            }
        }
    }

    *socialEventsCount = (uint32_t)socialVars.cEvents.size();
    return socialVars.cEvents.data();
}
CATCH_RETURN_WITH(nullptr)

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromFilters(
    _In_ XSAPI_XBOX_LIVE_USER *user,
    _In_ XSAPI_PRESENCE_FILTER presenceDetailLevel,
    _In_ XSAPI_RELATIONSHIP_FILTER filter,
    _Out_ XSAPI_XBOX_SOCIAL_USER_GROUP** group,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    presence_filter cPresenceDetailLevel = static_cast<presence_filter>(presenceDetailLevel);
    relationship_filter cFilter = static_cast<relationship_filter>(filter);
    socialVars.cppGroupResult = social_manager::get_singleton_instance()->create_social_user_group_from_filters(user->pImpl->cppUser(), cPresenceDetailLevel, cFilter);

    auto socialUserGroup = new XSAPI_XBOX_SOCIAL_USER_GROUP();
    socialUserGroup->pImpl = new XSAPI_XBOX_SOCIAL_USER_GROUP_IMPL(socialVars.cppGroupResult.payload(), socialUserGroup);
    socialVars.cGroups.push_back(socialUserGroup);
    *group = socialUserGroup;
    

    *errMessage = socialVars.cppGroupResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppGroupResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromList(
    _In_ XSAPI_XBOX_LIVE_USER *user,
    _In_ PCSTR* xboxUserIdList,
    _In_ uint32_t xboxUserIdListCount,
    _Out_ XSAPI_XBOX_SOCIAL_USER_GROUP** group,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    std::vector<string_t> xboxUserIdVector = std::vector<string_t>(xboxUserIdListCount);

    for (uint32_t i = 0; i < xboxUserIdListCount; i++)
    {
        xboxUserIdVector[i] = utils::to_utf16string(xboxUserIdList[i]);
    }

    socialVars.cppGroupResult = social_manager::get_singleton_instance()->create_social_user_group_from_list(user->pImpl->cppUser(), xboxUserIdVector);

    auto socialUserGroup = new XSAPI_XBOX_SOCIAL_USER_GROUP();
    socialUserGroup->pImpl = new XSAPI_XBOX_SOCIAL_USER_GROUP_IMPL(socialVars.cppGroupResult.payload(), socialUserGroup);
    socialVars.cGroups.push_back(socialUserGroup);
    *group = socialUserGroup;

    *errMessage = socialVars.cppGroupResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppGroupResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerDestroySocialUserGroup(
    _In_ XSAPI_XBOX_SOCIAL_USER_GROUP *group,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    // Remove group from our local store of XboxSocialUserGroups
    auto newEnd = std::remove(socialVars.cGroups.begin(), socialVars.cGroups.end(), group);
    socialVars.cGroups.erase(newEnd, socialVars.cGroups.end());

    socialVars.cppVoidResult = social_manager::get_singleton_instance()->destroy_social_user_group(group->pImpl->cppSocialUserGroup());

    *errMessage = socialVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerUpdateSocialUserGroup(
    _In_ XSAPI_XBOX_SOCIAL_USER_GROUP *group,
    _In_ PCSTR* users,
    _In_ uint32_t usersCount,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();

    std::vector<string_t> usersVector = std::vector<string_t>();

    for (uint32_t i = 0; i < usersCount; i++)
    {
        usersVector.push_back(utils::to_utf16string(users[i]));
    }

    socialVars.cppVoidResult = social_manager::get_singleton_instance()->update_social_user_group(group->pImpl->cppSocialUserGroup(), usersVector);
    group->pImpl->Refresh();

    *errMessage = socialVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppVoidResult.err());
}
CATCH_RETURN()

XSAPI_DLLEXPORT XSAPI_RESULT XBL_CALLING_CONV
SocialManagerSetRichPresencePollingStatus(
    _In_ XSAPI_XBOX_LIVE_USER *user,
    _In_ bool shouldEnablePolling,
    _Out_ PCSTR* errMessage
    ) XSAPI_NOEXCEPT
try
{
    verify_global_init();
    
    socialVars.cppVoidResult = social_manager::get_singleton_instance()->set_rich_presence_polling_status(user->pImpl->cppUser(), shouldEnablePolling);
    user->pImpl->Refresh();

    *errMessage = socialVars.cppVoidResult.err_message().c_str();
    return utils::xsapi_result_from_xbox_live_result_err(socialVars.cppVoidResult.err());
}
CATCH_RETURN()