// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

namespace xbox {
    namespace services {
        class xbox_live_app_config;
    }
}

struct XBOX_LIVE_APP_CONFIG_IMPL
{
public:
    XBOX_LIVE_APP_CONFIG_IMPL();

private:
    std::string m_scid;
    std::string m_environment;
    std::string m_sandbox;
    std::shared_ptr<xbox::services::xbox_live_app_config> m_cppConfig;
};