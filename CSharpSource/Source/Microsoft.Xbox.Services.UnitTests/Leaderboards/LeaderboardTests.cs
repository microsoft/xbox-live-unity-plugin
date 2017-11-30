// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.UnitTests.Leaderboards
{
    using global::System;
    using global::System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Leaderboard;

    [TestClass]
    public class LeaderboardTests : TestBase
    {
        private LeaderboardService leaderboardService;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            MockXboxLiveData.Load(Environment.CurrentDirectory + "\\Leaderboards\\MockDataForLeaderboardTests.json");
            this.leaderboardService = new LeaderboardService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            XboxLive.Instance.Dispose();
        }

        private static void VerifyLeaderboardColumn(LeaderboardColumn column, JObject columnToVerify)
        {
            Assert.AreNotEqual(column, null, "LeaderboardColumn was null.");
            Assert.AreEqual(column.StatisticName, columnToVerify["statName"].ToString());
            Assert.AreEqual(column.StatisticType.ToString(), columnToVerify["type"].ToString());
        }

        private static void VerifyLeaderboardRow(LeaderboardRow row, JObject rowToVerify)
        {
            Assert.AreNotEqual(row, null, "LeaderboardRow was null.");

            Assert.AreEqual(row.Gamertag, rowToVerify["gamertag"].ToString());
            Assert.AreEqual(row.XboxUserId, rowToVerify["xuid"].ToString());
            Assert.AreEqual(row.Percentile, (double)rowToVerify["percentile"]);
            Assert.AreEqual(row.Rank, (int)rowToVerify["rank"]);

            // TODO Add checks for values
        }

        private static void VerifyLeaderboardResult(LeaderboardResult result, JObject resultToVerify)
        {
            Assert.AreNotEqual(result, null, "LeaderboardResult was null.");

            JObject leaderboardInfoJson = JObject.Parse(resultToVerify["leaderboardInfo"].ToString());
            Assert.AreEqual(result.TotalRowCount, (uint)leaderboardInfoJson["totalCount"]);

            JObject jsonColumn = JObject.Parse(leaderboardInfoJson["columnDefinition"].ToString());
            VerifyLeaderboardColumn(result.Columns[0], jsonColumn);

            JArray jsonRows = (JArray)(resultToVerify)["userList"];
            int index = 0;
            foreach (var row in jsonRows)
            {
                VerifyLeaderboardRow(result.Rows[index++], (JObject)row);
            }
        }
    }
}