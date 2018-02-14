// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using Microsoft.Xbox.Services.Leaderboard;

    class MockStatisticManager : IStatisticManager
    {
        private List<StatisticEvent> mStatEventList;
        private static readonly List<StatisticEvent> mEmptyStatEventList = new List<StatisticEvent>();
        List<StatisticValue> mChangedStats;
        Dictionary<string, StatisticValue> mStats;

        internal MockStatisticManager()
        {
            this.LocalUsers = new List<XboxLiveUser>();
            mChangedStats = new List<StatisticValue>();
            mStats = new Dictionary<string, StatisticValue>
            {
                {
                    "DefaultNum", new StatisticValue()
                    {
                        Name = "DefaultNum",
                        AsNumber = 1.5,
                        DataType = StatisticDataType.Number
                    }
                },
                {
                    "DefaultString", new StatisticValue()
                    {
                        Name = "DefaultString",
                        AsString = "String Value",
                        DataType = StatisticDataType.String
                    }
                },
                {
                    "Default", new StatisticValue()
                    {
                        Name = "Default",
                        AsInteger = 1,
                        DataType = StatisticDataType.Number
                    }
                }
            };
            
            this.mStatEventList = new List<StatisticEvent>();
        }

        IList<XboxLiveUser> LocalUsers { get; set; }

        public void AddLocalUser(XboxLiveUser user)
        {
            LocalUsers.Add(user);
            mStatEventList.Add(new StatisticEvent(StatisticEventType.LocalUserAdded, user, null));
        }

        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            LocalUsers.Remove(user);
            mStatEventList.Add(new StatisticEvent(StatisticEventType.LocalUserRemoved, user, null));
        }

        public StatisticValue GetStatistic(XboxLiveUser user, string statName)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            if (!mStats.ContainsKey(statName))
            {
                mStats.Add(statName, new StatisticValue()
                {
                    Name = statName,
                    AsInteger = 100,
                    DataType = StatisticDataType.Number
                });
            }

            return mStats[statName];
        }

        public IList<string> GetStatisticNames(XboxLiveUser user)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            return mStats.Keys.ToList();
        }

        public void SetStatisticNumberData(XboxLiveUser user, string statName, double value)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            mChangedStats.Add(new StatisticValue()
            {
                Name = statName,
                AsNumber = value,
                DataType = StatisticDataType.Number
            });
        }

        public void SetStatisticIntegerData(XboxLiveUser user, string statName, long value)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            mChangedStats.Add(new StatisticValue()
            {
                Name = statName,
                AsNumber = value,
                AsInteger = value,
                DataType = StatisticDataType.Number
            });
        }

        public void SetStatisticStringData(XboxLiveUser user, string statName, string value)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            mChangedStats.Add(new StatisticValue()
            {
                Name = statName,
                AsString = value,
                DataType = StatisticDataType.String
            });
        }

        public void DeleteStatistic(XboxLiveUser user, string statName)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            mStats.Remove(statName);
        }

        public void RequestFlushToService(XboxLiveUser user, bool isHighPriority = false)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            foreach (StatisticValue stat in mChangedStats)
            {
                if (!mStats.ContainsKey(stat.Name))
                {
                    mStats.Add(stat.Name, stat);
                }
                else
                {
                    mStats[stat.Name] = stat;
                }
            }
            mChangedStats.Clear();
            mStatEventList.Add(new StatisticEvent(StatisticEventType.StatisticUpdateComplete, user, null));
        }

        public IList<StatisticEvent> DoWork()
        {
            List<StatisticEvent> copyList = null;
            if (mStatEventList.Count > 0)
            {
                copyList = mStatEventList.ToList();
            }
            else
            {
                copyList = mEmptyStatEventList;
            }
            
            mStatEventList.Clear();
            return copyList;
        }

        public void GetLeaderboard(XboxLiveUser user, string statName, LeaderboardQuery query)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            if (!mStats.ContainsKey(statName))
            {
                mStats[statName] = new StatisticValue()
                {
                    Name = statName,
                    AsInteger = 300,
                    AsNumber = 300,
                    DataType = StatisticDataType.Number
                };
            }

            StatisticValue stat = mStats[statName];

            List<LeaderboardRow> rows = new List<LeaderboardRow>();
            uint maxScore = query.MaxItems * 100;
            uint rankOffset = query.SkipResultToRank == 0 ? 1 : query.SkipResultToRank;
            bool userDisplayed = false;
            for (uint i = 0; i < query.MaxItems; i++)
            {
                uint score = maxScore - i * 100;
                LeaderboardRow row;
                if (!userDisplayed && stat.DataType == StatisticDataType.Number && (stat.AsNumber >= score || stat.AsInteger >= score))
                {
                    userDisplayed = true;
                    row = new LeaderboardRow(new List<string> { stat.AsNumber.ToString() }, i + rankOffset, 0.8, user.XboxUserId, user.Gamertag);
                }
                else
                {
                    row = new LeaderboardRow(new List<string> { score.ToString() }, i + rankOffset, 0.8, string.Format("{0}{0}{0}{0}{0}{0}{0}{0}", i), string.Format("Gamertag {0}", i));
                }

                rows.Add(row);
            }

            List<LeaderboardColumn> cols = new List<LeaderboardColumn>();
            cols.Add(new LeaderboardColumn(stat.DataType == StatisticDataType.String ? LeaderboardStatType.String : LeaderboardStatType.Integer, ""));

            LeaderboardResult result = new LeaderboardResult(rows, cols, query.MaxItems);

            LeaderboardResultEventArgs args = new LeaderboardResultEventArgs(result);

            mStatEventList.Add(new StatisticEvent(StatisticEventType.GetLeaderboardComplete, user, args));
        }

        public void GetSocialLeaderboard(XboxLiveUser user, string statName, string socialGroup, LeaderboardQuery query)
        {
            if (!LocalUsers.Contains(user))
            {
                throw new ArgumentException("Local User needs to be added.");
            }

            GetLeaderboard(user, statName, query);
        }
    }
}