// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;

    using Microsoft.Xbox.Services.Leaderboard;
    using System;

    public partial class StatisticManager : IStatisticManager
    {
        private readonly Dictionary<IntPtr, XboxLiveUser> m_localUsers = new Dictionary<IntPtr, XboxLiveUser>();

        internal XboxLiveUser GetUser(IntPtr userPtr)
        {
            if (m_localUsers.ContainsKey(userPtr))
            {
                return m_localUsers[userPtr];
            }

            throw new XboxException("User doesn't exist. Did you call AddLocalUser?");
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerAddLocalUser(IntPtr user, out IntPtr errMessage);
        public void AddLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerAddLocalUser(user.Impl.XboxLiveUserPtr, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Does local work
            m_localUsers[user.Impl.XboxLiveUserPtr] = user;
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerRemoveLocalUser(IntPtr user, out IntPtr errMessage);
        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerRemoveLocalUser(user.Impl.XboxLiveUserPtr, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Does local work
            if (m_localUsers.ContainsKey(user.Impl.XboxLiveUserPtr))
            {
                m_localUsers.Remove(user.Impl.XboxLiveUserPtr);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerRequestFlushToService(IntPtr user, bool isHighPriority, out IntPtr errMessage);
        public void RequestFlushToService(XboxLiveUser user, bool isHighPriority = false)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerRequestFlushToService(user.Impl.XboxLiveUserPtr, isHighPriority, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr StatsManagerDoWork(out Int32 numOfEvents);
        public IList<StatisticEvent> DoWork()
        {

            Int32 eventsCount;
            // Invokes the c method
            IntPtr eventsPtr = StatsManagerDoWork(out eventsCount);

            List<StatisticEvent> events = new List<StatisticEvent>();

            if (eventsCount > 0)
            {
                IntPtr[] cEvents = new IntPtr[eventsCount];
                Marshal.Copy(eventsPtr, cEvents, 0, (int)eventsCount);

                foreach (IntPtr cEvent in cEvents)
                {
                    events.Add(new StatisticEvent(cEvent));
                }

                // Refresh objects
                foreach (XboxLiveUser user in m_localUsers.Values.ToList())
                {
                    user.Impl.UpdatePropertiesFromXboxLiveUserPtr();
                }
            }

            return events.AsReadOnly();
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerSetStatisticNumberData(IntPtr user, string statName, double value, out IntPtr errMessage);
        public void SetStatisticNumberData(XboxLiveUser user, string statName, double value)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerSetStatisticNumberData(user.Impl.XboxLiveUserPtr, statName, value, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerSetStatisticIntegerData(IntPtr user, string statName, long value, out IntPtr errMessage);
        public void SetStatisticIntegerData(XboxLiveUser user, string statName, long value)
        {
            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerSetStatisticIntegerData(user.Impl.XboxLiveUserPtr, statName, value, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerSetStatisticStringData(IntPtr user, string statName, string value, out IntPtr errMessage);
        public void SetStatisticStringData(XboxLiveUser user, string statName, string value)
        {
            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerSetStatisticStringData(user.Impl.XboxLiveUserPtr, statName, value, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerDeleteStat(IntPtr user, string statName, out IntPtr errMessage);
        public void DeleteStatistic(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerDeleteStat(user.Impl.XboxLiveUserPtr, statName, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetLeaderboard(IntPtr user, string statName, IntPtr query, out IntPtr errMessage);
        public void GetLeaderboard(XboxLiveUser user, string statName, LeaderboardQuery query)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetLeaderboard(user.Impl.XboxLiveUserPtr, statName, query.GetPtr(), out cErrMessage);

            if (errCode > 0)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetSocialLeaderboard(IntPtr user, string statName, string socialGroup, IntPtr query, out IntPtr errMessage);
        public void GetSocialLeaderboard(XboxLiveUser user, string statName, string socialGroup, LeaderboardQuery query)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetSocialLeaderboard(user.Impl.XboxLiveUserPtr, statName, socialGroup, query.GetPtr(), out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetStat(IntPtr user, IntPtr statName, out IntPtr statValue, out IntPtr errMessage);
        public StatisticValue GetStatistic(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cStatValue;
            IntPtr cErrMessage;
            IntPtr cStatName = MarshalingHelpers.StringToHGlobalUtf8(statName);

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetStat(user.Impl.XboxLiveUserPtr, cStatName, out cStatValue, out cErrMessage);

            Marshal.FreeHGlobal(cStatName);
            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }

            // Does local work
            StatisticValue statValue = new StatisticValue(cStatValue);

            return statValue;
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetStatNames(IntPtr user, out IntPtr statNameList, out UInt32 statNameListCount, out IntPtr errMessage);
        public IList<string> GetStatisticNames(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            IntPtr cStatListPtr;
            UInt32 statListCount;
            IntPtr cErrMessage;
            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetStatNames(user.Impl.XboxLiveUserPtr, out cStatListPtr, out statListCount, out cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(errCode, cErrMessage);
            }
            return MarshalingHelpers.Utf8StringArrayToStringList(cStatListPtr, statListCount);
        }
    }
}
