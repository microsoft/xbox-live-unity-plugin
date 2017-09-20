// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Leaderboard;
    using Microsoft.Xbox.Services.Shared;

    public class StatsManager : IStatsManager
    {
        private static readonly object instanceLock = new object();
        private static IStatsManager instance;
        
        internal static IStatsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = XboxLive.UseMockServices ? new MockStatsManager() : (IStatsManager)new StatsManager();
                        }
                    }
                }
                return instance;
            }
        }

        private delegate void StatsManagerAddLocalUser(IntPtr user);
        public void AddLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // todo XboxLive.Instance.Invoke<StatslManagerAddLocalUser>(user.Impl.GetPtr());
            // todo m_localUsers.Add(user);
        }

        private delegate void StatsManagerRemoveLocalUser(IntPtr user);
        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // todo XboxLive.Instance.Invoke<StatslManagerAddLocalUser>(user.Impl.GetPtr());
            // todo m_localUsers.Remove(user);
        }

        private delegate void StatsManagerRequestFlushToService(IntPtr user, bool isHighPriority);
        public void RequestFlushToService(XboxLiveUser user, bool isHighPriority = false)
        {
            if (user == null) throw new ArgumentNullException("user");

            // todo XboxLive.Instance.Invoke<StatsManagerRequestFlushToService>(user.Impl.GetPtr(), isHighPriority);
        }

        public IList<StatEvent> DoWork()
        {
            throw new NotImplementedException();
        }

        private delegate void StatsManagerSetStatisticNumberData(IntPtr user, string statName, double value);
        public void SetStatisticNumberData(XboxLiveUser user, string statName, double value)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(statName)) throw new ArgumentNullException("statName");

            XboxLive.Instance.Invoke<StatsManagerSetStatisticNumberData>(user.Impl.GetPtr(), statName, value);
        }

        public void SetStatisticIntegerData(XboxLiveUser user, string statName, long value)
        {
            throw new NotImplementedException();
        }

        public void SetStatisticStringData(XboxLiveUser user, string statName, string value)
        {
            throw new NotImplementedException();
        }

        private delegate void StatsManagerDeleteStat(IntPtr user, string statName);
        public void DeleteStat(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(statName)) throw new ArgumentNullException("statName");

            // XboxLive.Instance.Invoke<StatsManagerDeleteStat>(user.Impl.GetPtr(), statName);
        }

        public void GetLeaderboard(XboxLiveUser user, string statName, LeaderboardQuery query)
        {
            throw new NotImplementedException();
        }

        public void GetSocialLeaderboard(XboxLiveUser user, string statName, string socialGroup, LeaderboardQuery query)
        {
            throw new NotImplementedException();
        }

        public StatValue GetStat(XboxLiveUser user, string statName)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetStatNames(XboxLiveUser user)
        {
            throw new NotImplementedException();
        }
    }
}