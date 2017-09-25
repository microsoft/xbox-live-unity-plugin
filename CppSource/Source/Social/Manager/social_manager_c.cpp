// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "user_impl.h"
#include "social_manager_helper_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::social::manager;

// Kept here so it isn't garbage collected before managed code can read it
std::vector<XboxSocialUserGroup*> mGroups;
std::vector<SocialEvent *> mEvents;


XSAPI_DLLEXPORT bool XBL_CALLING_CONV
SocialManagerPresenceRecordIsUserPlayingTitle(
    _In_ SocialManagerPresenceRecord* presenceRecord,
    _In_ uint32_t titleId
    )
{
    verify_global_init();

    return presenceRecord->pImpl->m_cppSocialManagerPresenceRecord.is_user_playing_title(titleId);
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerAddLocalUser(
    _In_ XboxLiveUser *user,
    _In_ SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL extraLevelDetail,
    _Out_ PCSTR* errMessage
	)
{
    verify_global_init();

	social_manager_extra_detail_level cExtraLevelDetail = static_cast<social_manager_extra_detail_level>(extraLevelDetail);
	auto result = social_manager::get_singleton_instance()->add_local_user(user->pImpl->m_cppUser, cExtraLevelDetail);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerRemoveLocalUser(
    _In_ XboxLiveUser *user,
    _Out_ PCSTR* errMessage
	)
{
    verify_global_init();

	auto result = social_manager::get_singleton_instance()->remove_local_user(user->pImpl->m_cppUser);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}


XSAPI_DLLEXPORT SocialEvent** XBL_CALLING_CONV
SocialManagerDoWork(
    _Out_ int32* numOfEvents
	)
{
    verify_global_init();

    std::vector<social_event> cppSocialEvents = social_manager::get_singleton_instance()->do_work();
	
    mEvents.clear();

    if (cppSocialEvents.size() > 0) {
        for (auto cEvent : cppSocialEvents) {
            mEvents.push_back(CreateSocialEventFromCpp(cEvent, mGroups));
        }

        for (auto socialUserGroup : mGroups) {
            if (socialUserGroup != nullptr) {
                socialUserGroup->pImpl->Refresh();
            }
        }
    }

    *numOfEvents = mEvents.size();
    return mEvents.data();
}


XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromFilters(
	_In_ XboxLiveUser *user,
	_In_ PRESENCE_FILTER presenceDetailLevel,
	_In_ RELATIONSHIP_FILTER filter,
    _Out_ XboxSocialUserGroup** group,
    _Out_ PCSTR* errMessage
	)
{
    verify_global_init();

	presence_filter cPresenceDetailLevel = static_cast<presence_filter>(presenceDetailLevel);
	relationship_filter cFilter = static_cast<relationship_filter>(filter);
	auto result = social_manager::get_singleton_instance()->create_social_user_group_from_filters(user->pImpl->m_cppUser, cPresenceDetailLevel, cFilter);

	auto socialUserGroup = new XboxSocialUserGroup();
	socialUserGroup->pImpl = new XboxSocialUserGroupImpl(result.payload(), socialUserGroup);
    mGroups.push_back(socialUserGroup);
    *group = socialUserGroup;

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromList(
    _In_ XboxLiveUser *user,
    _In_ PCSTR* xboxUserIdList,
    _In_ int numOfXboxUserIds,
    _Out_ XboxSocialUserGroup** group,
    _Out_ PCSTR* errMessage
	)
{
    verify_global_init();

    std::vector<string_t> xboxUserIdVector = std::vector<string_t>(numOfXboxUserIds);

    for (int i = 0; i < numOfXboxUserIds; i++)
    {
        xboxUserIdVector[i] = utils::to_utf16string(xboxUserIdList[i]);
    }

	auto result = social_manager::get_singleton_instance()->create_social_user_group_from_list(user->pImpl->m_cppUser, xboxUserIdVector);

	auto socialUserGroup = new XboxSocialUserGroup();
	socialUserGroup->pImpl = new XboxSocialUserGroupImpl(result.payload(), socialUserGroup);
    mGroups.push_back(socialUserGroup);
    *group = socialUserGroup;

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerDestroySocialUserGroup(
    _In_ XboxSocialUserGroup *group,
    _Out_ PCSTR* errMessage
    )
{
    verify_global_init();

    // Remove group from our local store of XboxSocialUserGroups
    auto newEnd = std::remove(mGroups.begin(), mGroups.end(), group);
    mGroups.erase(newEnd, mGroups.end());

    auto result = social_manager::get_singleton_instance()->destroy_social_user_group(group->pImpl->m_cppSocialUserGroup);

    *errMessage = result.err_message().c_str();
    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerUpdateSocialUserGroup(
    _In_ XboxSocialUserGroup *group,
    _In_ PCSTR* users,
    _In_ int numOfUsers,
    _Out_ PCSTR* errMessage
	)
{
    verify_global_init();

    std::vector<string_t> usersVector = std::vector<string_t>();

    for (int i = 0; i < numOfUsers; i++)
    {
        usersVector.push_back(utils::to_utf16string(users[i]));
    }

	auto result = social_manager::get_singleton_instance()->update_social_user_group(group->pImpl->m_cppSocialUserGroup, usersVector);
    group->pImpl->Refresh();

    auto wErrMessage = std::wstring(result.err_message().size(), '\0');
    std::copy(result.err_message().begin(), result.err_message().end(), wErrMessage.begin());
    *errMessage = result.err_message().c_str();

    return result.err().value();
}

XSAPI_DLLEXPORT int32 XBL_CALLING_CONV
SocialManagerSetRichPresencePollingStatus(
    _In_ XboxLiveUser *user,
    _In_ bool shouldEnablePolling,
    _Out_ PCSTR* errMessage
	)
{
    verify_global_init();
	
	auto result = social_manager::get_singleton_instance()->set_rich_presence_polling_status(user->pImpl->m_cppUser, shouldEnablePolling);
    user->pImpl->Refresh();

    *errMessage = result.err_message().c_str();
    return result.err().value();
}