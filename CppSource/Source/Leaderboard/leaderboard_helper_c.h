// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#ifndef MY_HEADER_FILE_
#define MY_HEADER_FILE_

#include "pch.h"
#include "xsapi\leaderboard_c.h"

using namespace xbox::services;
using namespace xbox::services::system;
using namespace xbox::services::leaderboard;

struct XSAPI_LEADERBOARD_COLUMN_IMPL {
    XSAPI_LEADERBOARD_COLUMN_IMPL(
        _In_ leaderboard_column cppLeaderboardColumn,
        _In_ XSAPI_LEADERBOARD_COLUMN* cLeaderboardColumn
    ) : m_cLeaderboardColumn(cLeaderboardColumn), m_cppLeaderboardColumn(cppLeaderboardColumn)
    {
        m_statName = utils::to_utf8string(m_cppLeaderboardColumn.stat_name());
        m_cLeaderboardColumn->statName = m_statName.c_str();

        m_cLeaderboardColumn->statType = static_cast<XSAPI_LEADERBOARD_STAT_TYPE>(m_cppLeaderboardColumn.stat_type());
    }

    std::string m_statName;

    leaderboard_column m_cppLeaderboardColumn;
    XSAPI_LEADERBOARD_COLUMN* m_cLeaderboardColumn;
};

inline XSAPI_LEADERBOARD_COLUMN* CreateLeaderboardColumnFromCpp(
    _In_ leaderboard_column cppLeaderboardColumn
) 
{
    auto leaderboardColumn = new XSAPI_LEADERBOARD_COLUMN();
    leaderboardColumn->pImpl = new XSAPI_LEADERBOARD_COLUMN_IMPL(cppLeaderboardColumn, leaderboardColumn);
    return leaderboardColumn;
}

struct XSAPI_LEADERBOARD_ROW_IMPL {
    XSAPI_LEADERBOARD_ROW_IMPL(
        _In_ leaderboard_row cppLeaderboardRow,
        _In_ XSAPI_LEADERBOARD_ROW* cLeaderboardRow
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

        for (size_t i = 0; i < m_cppLeaderboardRow.column_values().size(); i++) 
        {
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
    XSAPI_LEADERBOARD_ROW* m_cLeaderboardRow;
};

inline XSAPI_LEADERBOARD_ROW* CreateLeaderboardRowFromCpp(
    _In_ leaderboard_row cppLeaderboardRow
)
{
    auto leaderboardRow = new XSAPI_LEADERBOARD_ROW();
    leaderboardRow->pImpl = new XSAPI_LEADERBOARD_ROW_IMPL(cppLeaderboardRow, leaderboardRow);
    return leaderboardRow;
}


struct XSAPI_LEADERBOARD_QUERY_IMPL
{
    XSAPI_LEADERBOARD_QUERY_IMPL(
        _In_ leaderboard_query creationContext,
        _In_ XSAPI_LEADERBOARD_QUERY* cQuery
    ) : m_cQuery(cQuery), m_cppQuery(creationContext)
    {
        Refresh();
    }

    void Refresh()
    {
        m_cQuery->skipResultToMe = m_cppQuery.skip_result_to_me();

        m_cQuery->skipResultToRank = m_cppQuery.skip_result_to_rank();

        m_cQuery->maxItems = m_cppQuery.max_items();

        m_cQuery->order = static_cast<XSAPI_SORT_ORDER>(m_cppQuery.order());

        m_statName = utils::to_utf8string(m_cppQuery.stat_name());
        m_cQuery->statName = m_statName.c_str();

        m_socialGroup = utils::to_utf8string(m_cppQuery.social_group());
        m_cQuery->socialGroup = m_socialGroup.c_str();

        m_hasNext = m_cppQuery.has_next();
        m_cQuery->hasNext = m_hasNext;
    }

    void SetSkipResultToMe(bool skipResultToMe) 
    {
        m_cQuery->skipResultToMe = skipResultToMe;
        m_cppQuery.set_skip_result_to_me(skipResultToMe);
    }

    void SetSkipResultToRank(uint32 skipResultToRank) 
    {
        m_cQuery->skipResultToRank = skipResultToRank;
        m_cppQuery.set_skip_result_to_rank(skipResultToRank);
    }

    void SetMaxItems(uint32 maxItems) 
    {
        m_cQuery->maxItems = maxItems;
        m_cppQuery.set_max_items(maxItems);
    }

    void SetOrder(XSAPI_SORT_ORDER order)
    {
        m_cQuery->order = order;
        m_cppQuery.set_order(static_cast<sort_order>(order));
    }

    std::string m_statName;
    std::string m_socialGroup;
    bool m_hasNext;

    leaderboard_query m_cppQuery;
    XSAPI_LEADERBOARD_QUERY* m_cQuery;
};

inline XSAPI_LEADERBOARD_QUERY* CreateLeaderboardQueryFromCpp(
    _In_ leaderboard_query query
)
{
    auto leaderboardQuery = new XSAPI_LEADERBOARD_QUERY();
    leaderboardQuery->pImpl = new XSAPI_LEADERBOARD_QUERY_IMPL(query, leaderboardQuery);

    return leaderboardQuery;
}

struct XSAPI_LEADERBOARD_RESULT_IMPL {
    XSAPI_LEADERBOARD_RESULT_IMPL(
        _In_ leaderboard_result cppLeaderboardResult,
        _In_ XSAPI_LEADERBOARD_RESULT* cLeaderboardResult
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
    std::vector<XSAPI_LEADERBOARD_COLUMN*> m_columns;
    std::vector<XSAPI_LEADERBOARD_ROW*> m_rows;

    leaderboard_result m_cppLeaderboardResult;
    XSAPI_LEADERBOARD_RESULT* m_cLeaderboardResult;
};

inline XSAPI_LEADERBOARD_RESULT* CreateLeaderboardResultFromCpp(
    _In_ leaderboard_result cppLeaderboardResult
)
{
    auto leaderboardResult = new XSAPI_LEADERBOARD_RESULT();
    leaderboardResult->pImpl = new XSAPI_LEADERBOARD_RESULT_IMPL(cppLeaderboardResult, leaderboardResult);
    return leaderboardResult;
}

#endif