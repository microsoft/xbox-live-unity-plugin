// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "user_impl_c.h"
#include "social_manager_helper_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::social::manager;
using namespace xbox::httpclient;

std::vector<XboxSocialUserGroup*> mGroups;

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerAddLocalUser(
	_In_ XboxLiveUser *user,
	_In_ SOCIAL_MANAGER_EXTRA_DETAIL_LEVEL extraLevelDetail
	)
{
	VerifyGlobalXsapiInit();

	social_manager_extra_detail_level cExtraLevelDetail = static_cast<social_manager_extra_detail_level>(extraLevelDetail);
	social_manager::get_singleton_instance()->add_local_user(user->pImpl->m_cppUser, cExtraLevelDetail);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerRemoveLocalUser(
	_In_ XboxLiveUser *user
	)
{
	VerifyGlobalXsapiInit();

	social_manager::get_singleton_instance()->remove_local_user(user->pImpl->m_cppUser);
}


// Kept here so it isn't garbage collected before managed code can read it
std::vector<SocialEvent *> mEvents;
XSAPI_DLLEXPORT SocialEvent** XBL_CALLING_CONV
SocialManagerDoWork(
    _Inout_ int* numOfEvents
	)
{
	VerifyGlobalXsapiInit();

    std::vector<social_event> socialEvents = social_manager::get_singleton_instance()->do_work();
	
    *numOfEvents = socialEvents.size();

    mEvents.clear();

    if (socialEvents.size() > 0) {
        for (auto cEvent : socialEvents) {
            mEvents.push_back(CreateSocialEventFromCpp(cEvent, mGroups));
        }

        for (auto socialUserGroup : mGroups) {
            if (socialUserGroup != nullptr) {
                socialUserGroup->pImpl->Refresh();
            }
        }
    }

    return mEvents.data();
}


XSAPI_DLLEXPORT XboxSocialUserGroup* XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromFilters(
	_In_ XboxLiveUser *user,
	_In_ PRESENCE_FILTER presenceDetailLevel,
	_In_ RELATIONSHIP_FILTER filter
	)
{
	VerifyGlobalXsapiInit();

	presence_filter cPresenceDetailLevel = static_cast<presence_filter>(presenceDetailLevel);
	relationship_filter cFilter = static_cast<relationship_filter>(filter);
	auto cSocialUserGroup = social_manager::get_singleton_instance()->create_social_user_group_from_filters(user->pImpl->m_cppUser, cPresenceDetailLevel, cFilter);

	auto socialUserGroup = new XboxSocialUserGroup();
	socialUserGroup->pImpl = new XboxSocialUserGroupImpl(cSocialUserGroup.payload(), socialUserGroup);
    mGroups.push_back(socialUserGroup);
	return socialUserGroup;
}

XSAPI_DLLEXPORT XboxSocialUserGroup* XBL_CALLING_CONV
SocialManagerCreateSocialUserGroupFromList(
	_In_ XboxLiveUser *user,
	_In_ PCSTR_T* xboxUserIdList,
    _In_ int numOfXboxUserIds
	)
{
	VerifyGlobalXsapiInit();

    std::vector<string_t> xboxUserIdVector = std::vector<string_t>(numOfXboxUserIds);

    for (int i = 0; i < numOfXboxUserIds; i++)
    {
        xboxUserIdVector[i] = xboxUserIdList[i];
    }

	auto result = social_manager::get_singleton_instance()->create_social_user_group_from_list(user->pImpl->m_cppUser, xboxUserIdVector);

	auto socialUserGroup = new XboxSocialUserGroup();
	socialUserGroup->pImpl = new XboxSocialUserGroupImpl(result.payload(), socialUserGroup);
    mGroups.push_back(socialUserGroup);
	return socialUserGroup;
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerDestroySocialUserGroup(
    _In_ XboxSocialUserGroup *group
    )
{
    VerifyGlobalXsapiInit();

    // Remove group from our local store of XboxSocialUserGroups
    auto newEnd = std::remove(mGroups.begin(), mGroups.end(), group);
    mGroups.erase(newEnd, mGroups.end());

    social_manager::get_singleton_instance()->destroy_social_user_group(group->pImpl->m_cppSocialUserGroup);
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerUpdateSocialUserGroup(
	_In_ XboxSocialUserGroup *group,
	_In_ PCSTR_T* users,
    _In_ int numOfUsers
	)
{
	VerifyGlobalXsapiInit();

    std::vector<string_t> usersVector = std::vector<string_t>();

    for (int i = 0; i < numOfUsers; i++)
    {
        usersVector.push_back(users[i]);
    }

	social_manager::get_singleton_instance()->update_social_user_group(group->pImpl->m_cppSocialUserGroup, usersVector);
    group->pImpl->Refresh();
}

XSAPI_DLLEXPORT void XBL_CALLING_CONV
SocialManagerSetRichPresencePollingStatus(
	_In_ XboxLiveUser *user,
	_In_ bool shouldEnablePolling
	)
{
	VerifyGlobalXsapiInit();
	
	social_manager::get_singleton_instance()->set_rich_presence_polling_status(user->pImpl->m_cppUser, shouldEnablePolling);
}