using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Xbox_Live.Scripts.MockData
{
    using System.Globalization;
    using System.Threading.Tasks;

    using System;
    using System.Net;

    using Microsoft.Xbox.Services;
    using Microsoft.Xbox.Services.Leaderboard;

    //public static class MockLeaderboardData
    //{
    //    public static void AddMockLeaderboardData(string serviceConfigurationId, string leaderboardName, string xuid, string socialGroup, uint skipToRank, string[] additionalColumns, uint maxItems)
    //    {
    //        LeaderboardResult result = MockLeaderboardResult.Generate(leaderboardName, 100, 3, skipToRank, maxItems);

    //        XboxLiveHttpRequest request = new MockXboxLiveHttpRequest(null, "GET", "leaderboards.xboxlive.com", "/scid/1234");
    //        //HttpWebResponse response = new HttpWebResponse();
    //        //XboxLiveHttpResponse response = new XboxLiveHttpResponse()

    //        //MockXboxLiveHttpRequest.AddMockResponse(new MockXboxLiveHttpRequest(), );
            
    //        //return Task.FromResult(result);
    //    }
    //}

    //public class MockLeaderboardResult : LeaderboardResult
    //{
    //    private static readonly Random random = new Random(24021524);

    //    private readonly IList<LeaderboardRow> allRows;
    //    private readonly uint offset;

    //    public MockLeaderboardResult(string displayName, IList<LeaderboardColumn> columns, IList<LeaderboardRow> allRows, uint skipToRank, uint maxItems)
    //    {
    //        this.DisplayName = displayName;

    //        this.Columns = columns;
    //        this.allRows = allRows;
    //        this.Rows = this.allRows.Skip((int)skipToRank).Take((int)maxItems).ToList();
    //        this.offset = skipToRank;

    //        this.TotalRowCount = (uint)this.allRows.Count;
    //        this.HasNext = skipToRank + maxItems < this.allRows.Count;
    //    }

    //    public override Task<LeaderboardResult> GetNextAsync(uint maxItems)
    //    {
    //        if (!this.HasNext)
    //        {
    //            return null;
    //        }

    //        LeaderboardResult result = new MockLeaderboardResult(this.DisplayName, this.Columns, this.allRows, this.offset + (uint)this.Rows.Count, maxItems);
    //        return Task.FromResult(result);
    //    }

    //    public static MockLeaderboardResult Generate(string displayName, int rowCount, int columnCount, uint skipToRank, uint maxItems)
    //    {
    //        List<LeaderboardColumn> columns = Enumerable.Range(0, columnCount).Select(i => NextLeaderboardColumn()).ToList();
    //        List<LeaderboardRow> rows = Enumerable.Range(1, rowCount).Select(i => NextLeaderboardRow(columns, i)).ToList();

    //        MockLeaderboardResult result = new MockLeaderboardResult(
    //            displayName == null ? "Mock Leaderboard " + random.Next() : displayName + " Leaderboard",
    //            columns,
    //            rows,
    //            skipToRank,
    //            maxItems);

    //        return result;
    //    }


    //    public static LeaderboardRow NextLeaderboardRow(IList<LeaderboardColumn> columns, int rank)
    //    {
    //        return new LeaderboardRow
    //        {
    //            Gamertag = random.NextGamertag(),
    //            Percentile = 0.99,
    //            Rank = (uint)rank,
    //            Values = columns.Select(c =>
    //            {
    //                if (c.StatisticType == typeof(int))
    //                {
    //                    return random.Next(0, 1000).ToString();
    //                }

    //                if (c.StatisticType == typeof(double))
    //                {
    //                    return random.NextDouble().ToString(CultureInfo.CurrentUICulture);
    //                }

    //                return Guid.NewGuid().ToString();
    //            }).ToList()
    //        };
    //    }

    //    public static LeaderboardColumn NextLeaderboardColumn()
    //    {
    //        int id = random.Next();
    //        Type statType;
    //        switch (id)
    //        {
    //            case 0:
    //                statType = typeof(int);
    //                break;
    //            case 1:
    //                statType = typeof(double);
    //                break;
    //            default:
    //                statType = typeof(string);
    //                break;
    //        }

    //        return new LeaderboardColumn
    //        {
    //            DisplayName = "Column " + id,
    //            StatisticName = "Stat" + id,
    //            StatisticType = statType,
    //        };
    //    }

    //}
}
