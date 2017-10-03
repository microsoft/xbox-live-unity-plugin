// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#ifndef MY_HEADER_FILE_
#define MY_HEADER_FILE_

#include "pch.h"
#include "xsapi\leaderboard_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::leaderboard;

struct LeaderboardColumnImpl {
    LeaderboardColumnImpl(
        _In_ leaderboard_column cppLeaderboardColumn,
        _In_ LeaderboardColumn* cLeaderboardColumn
    ) : m_cLeaderboardColumn(cLeaderboardColumn), m_cppLeaderboardColumn(cppLeaderboardColumn)
    {
        m_statName = utils::to_utf8string(m_cppLeaderboardColumn.stat_name());
        m_cLeaderboardColumn->statName = m_statName.c_str();

        m_statType = static_cast<LEADERBOARD_STAT_TYPE>(m_cppLeaderboardColumn.stat_type());
    }

    std::string m_statName;
    LEADERBOARD_STAT_TYPE m_statType;

    leaderboard_column m_cppLeaderboardColumn;
    LeaderboardColumn* m_cLeaderboardColumn;
};

inline LeaderboardColumn* CreateLeaderboardColumnFromCpp(
    _In_ leaderboard_column cppLeaderboardColumn
) 
{
    auto leaderboardColumn = new LeaderboardColumn();
    leaderboardColumn->pImpl = new LeaderboardColumnImpl(cppLeaderboardColumn, leaderboardColumn);
    return leaderboardColumn;
}

struct LeaderboardRowImpl {
    LeaderboardRowImpl(
        _In_ leaderboard_row cppLeaderboardRow,
        _In_ LeaderboardRow* cLeaderboardRow
    ) : m_cLeaderboardRow(cLeaderboardRow), m_cppLeaderboardRow(cppLeaderboardRow)
    {
        m_gamertag = utils::to_utf8string(m_cppLeaderboardRow.gamertag());
        m_cLeaderboardRow->gamertag = m_gamertag.c_str();

        m_xboxUserId = utils::to_utf8string(m_cppLeaderboardRow.xbox_user_id());
        m_cLeaderboardRow->xboxUserId = m_xboxUserId.c_str();

        m_percentile = m_cppLeaderboardRow.percentile();
        m_cLeaderboardRow->percentile = m_percentile;

        m_rank = m_cppLeaderboardRow.rank();
        m_cLeaderboardRow->rank = m_rank;

        // todo do I need 2 vectors one for std::string and one for PCSTR // yes
        for (size_t i = 0; i < m_cppLeaderboardRow.column_values().size(); i++) {
            m_columnValuesStrs.push_back(utils::to_utf8string(m_cppLeaderboardRow.column_values()[i]));
            m_columnValues.push_back(m_columnValuesStrs[i].c_str());
        }
        m_cLeaderboardRow->columnValues = m_columnValues.data();
        m_cLeaderboardRow->columnValuesSize = m_columnValues.size();
    }

    std::string m_gamertag;
    std::string m_xboxUserId;
    double m_percentile;
    uint32 m_rank;
    std::vector<std::string> m_columnValuesStrs;
    std::vector<PCSTR> m_columnValues;

    leaderboard_row m_cppLeaderboardRow;
    LeaderboardRow* m_cLeaderboardRow;
};

inline LeaderboardRow* CreateLeaderboardRowFromCpp(
    _In_ leaderboard_row cppLeaderboardRow
)
{
    auto leaderboardRow = new LeaderboardRow();
    leaderboardRow->pImpl = new LeaderboardRowImpl(cppLeaderboardRow, leaderboardRow);
    return leaderboardRow;
}


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

        m_statName = utils::to_utf8string(m_cppQuery.stat_name());
        m_cQuery->statName = m_statName.c_str();

        m_socialGroup = utils::to_utf8string(m_cppQuery.social_group());
        m_cQuery->socialGroup = m_socialGroup.c_str();

        m_hasNext = m_cppQuery.has_next();
        m_cQuery->hasNext = m_hasNext;
    }

    void SetSkipResultToMe(bool skipResultToMe) {
        m_skipResultToMe = skipResultToMe;
        m_cQuery->skipResultToMe = m_skipResultToMe;
        m_cppQuery.set_skip_result_to_me(m_skipResultToMe);
    }

    void SetSkipResultToRank(uint32 skipResultToRank) {
        m_skipResultToRank = skipResultToRank;
        m_cQuery->skipResultToRank = m_skipResultToRank;
        m_cppQuery.set_skip_result_to_rank(m_skipResultToRank);
    }

    void SetMaxItems(uint32 maxItems) {
        m_maxItems = maxItems;
        m_cQuery->maxItems = m_maxItems;
        m_cppQuery.set_max_items(m_maxItems);
    }

    void SetOrder(SORT_ORDER order) {
        m_order = order;
        m_cQuery->order = m_order;
        m_cppQuery.set_order(static_cast<sort_order>(m_order));
    }

    bool m_skipResultToMe;
    uint32 m_skipResultToRank;
    uint32 m_maxItems;
    SORT_ORDER m_order;
    std::string m_statName;
    std::string m_socialGroup;
    bool m_hasNext;
    leaderboard_query m_cppQuery;
    LeaderboardQuery* m_cQuery;
};

inline LeaderboardQuery* CreateLeaderboardQueryFromCpp(
    _In_ leaderboard_query query
)
{
    auto leaderboardQuery = new LeaderboardQuery();
    leaderboardQuery->pImpl = new LeaderboardQueryImpl(query, leaderboardQuery);

    return leaderboardQuery;
}

struct LeaderboardResultImpl {
    LeaderboardResultImpl(
        _In_ leaderboard_result cppLeaderboardResult,
        _In_ LeaderboardResult* cLeaderboardResult
    ) : m_cLeaderboardResult(cLeaderboardResult), m_cppLeaderboardResult(cppLeaderboardResult)
    {
        m_totalRowCount = m_cppLeaderboardResult.total_row_count();
        m_cLeaderboardResult->totalRowCount = m_totalRowCount;

        for (auto column : m_cppLeaderboardResult.columns())
        {
            m_columns.push_back(CreateLeaderboardColumnFromCpp(column));
        }
        m_cLeaderboardResult->columns = m_columns.data();
        m_cLeaderboardResult->columnsSize = m_columns.size();

        for (auto row : m_cppLeaderboardResult.rows())
        {
            m_rows.push_back(CreateLeaderboardRowFromCpp(row));
        }
        m_cLeaderboardResult->rows = m_rows.data();
        m_cLeaderboardResult->rowsSize = m_rows.size();
    }

    uint32 m_totalRowCount;
    std::vector<LeaderboardColumn*> m_columns;
    std::vector<LeaderboardRow*> m_rows;

    leaderboard_result m_cppLeaderboardResult;
    LeaderboardResult* m_cLeaderboardResult;
};

inline LeaderboardResult* CreateLeaderboardResultFromCpp(
    _In_ leaderboard_result cppLeaderboardResult
)
{
    auto leaderboardResult = new LeaderboardResult();
    leaderboardResult->pImpl = new LeaderboardResultImpl(cppLeaderboardResult, leaderboardResult);
    return leaderboardResult;
}

#endif