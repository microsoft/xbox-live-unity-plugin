// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "xsapi/system_c.h"

struct XboxLiveUserImpl
{
    XboxLiveUserImpl(
        _In_ Windows::System::User^ creationContext,
        _In_ XboxLiveUser *cUser
        )
        : m_cUser(cUser)
    {
        if (creationContext != nullptr)
        {
            m_cppUser = std::make_shared<xbox::services::system::xbox_live_user>(creationContext);
        }
        else
        {
            m_cppUser = std::make_shared<xbox::services::system::xbox_live_user>();
        }
    }

    XboxLiveUserImpl(
        _In_ std::shared_ptr<xbox::services::system::xbox_live_user> cppUser,
        _In_ XboxLiveUser *cUser
    )
        : m_cppUser(cppUser), m_cUser(cUser)
    {
        Refresh();
    }

    void Refresh()
    {
        if (m_cUser != nullptr)
        {
            m_xboxUserId = utils::to_utf8string(m_cppUser->xbox_user_id());
            m_cUser->xboxUserId = m_xboxUserId.data();

            m_gamertag = utils::to_utf8string(m_cppUser->gamertag());
            m_cUser->gamertag = m_gamertag.data();

            m_ageGroup = utils::to_utf8string(m_cppUser->age_group());
            m_cUser->ageGroup = m_ageGroup.data();

            m_privileges = utils::to_utf8string(m_cppUser->privileges());
            m_cUser->privileges = m_privileges.data();

            m_cUser->isSignedIn = m_cppUser->is_signed_in();

#if WINAPI_FAMILY && WINAPI_FAMILY==WINAPI_FAMILY_APP
            m_webAccountId = utils::to_utf8string(m_cppUser->web_account_id());
            m_cUser->webAccountId = m_webAccountId.data();
#endif
            m_cUser->windowsSystemUser = m_cppUser->windows_system_user();
        }
    }

    std::string m_xboxUserId;
    std::string m_gamertag;
    std::string m_ageGroup;
    std::string m_privileges;
    std::string m_webAccountId;

    XboxLiveUser* m_cUser;
    std::shared_ptr<xbox::services::system::xbox_live_user> m_cppUser;

};