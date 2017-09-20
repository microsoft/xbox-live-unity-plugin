// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::leaderboard;

struct LeaderboardQueryImpl
{
    LeaderboardQueryImpl(
        _In_ leaderboard_query creationContext,
        _In_ LeaderboardQuery* cQuery
    ) : m_cQuery(cQuery), m_cppQuery(creationContext)
    {
        Refresh();
    }

    void Refresh()
    {
        m_skipResultToMe = m_cppQuery.skip_result_to_me();
        m_cQuery->skipResultToMe = m_skipResultToMe;

        m_skipResultToRank = m_cppQuery.skip_result_to_rank();
        m_cQuery->skipResultToRank = m_skipResultToRank;

        m_maxItems = m_cppQuery.max_items();
        m_cQuery->maxItems = m_maxItems;

        m_order = static_cast<SORT_ORDER>(m_cppQuery.order());
        m_cQuery->order = m_order;

        m_statName = m_cppQuery.stat_name().c_str();
        m_cQuery->statName = m_statName;

        m_socialGroup = m_cppQuery.social_group().c_str();
        m_cQuery->socialGroup = m_socialGroup;

        m_hasNext = m_cppQuery.has_next();
        m_cQuery->hasNext = m_hasNext;
    }

    bool m_skipResultToMe;
    uint32 m_skipResultToRank;
    uint32 m_maxItems;
    SORT_ORDER m_order;
    PCSTR_T m_statName;
    PCSTR_T m_socialGroup;
    bool m_hasNext;
    leaderboard_query m_cppQuery;
    LeaderboardQuery* m_cQuery;
};