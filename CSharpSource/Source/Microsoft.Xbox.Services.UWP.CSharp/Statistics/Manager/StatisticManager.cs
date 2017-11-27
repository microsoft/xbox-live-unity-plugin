// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    using Microsoft.Xbox.Services.Leaderboard;
    using System;

    public partial class StatisticManager : IStatisticManager
    {
        private readonly List<XboxLiveUser> m_localUsers = new List<XboxLiveUser>();

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerAddLocalUser(IntPtr user, IntPtr errMessage);
        public void AddLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerAddLocalUser(user.Impl.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Does local work
            m_localUsers.Add(user);
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerRemoveLocalUser(IntPtr user, IntPtr errMessage);
        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerRemoveLocalUser(user.Impl.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Does local work
            m_localUsers.Remove(user);
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerRequestFlushToService(IntPtr user, bool isHighPriority, IntPtr errMessage);
        public void RequestFlushToService(XboxLiveUser user, bool isHighPriority = false)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerRequestFlushToService(user.Impl.GetPtr(), isHighPriority, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern IntPtr StatsManagerDoWork(IntPtr numOfEvents);
        public IList<StatisticEvent> DoWork()
        {
            // Allocates memory for returned objects
            IntPtr cNumOfEvents = Marshal.AllocHGlobal(Marshal.SizeOf<Int32>());

            // Invokes the c method
            IntPtr eventsPtr = StatsManagerDoWork(cNumOfEvents);

            // Does local work
            int numOfEvents = Marshal.ReadInt32(cNumOfEvents);
            Marshal.FreeHGlobal(cNumOfEvents);

            List<StatisticEvent> events = new List<StatisticEvent>();

            if (numOfEvents > 0)
            {
                IntPtr[] cEvents = new IntPtr[numOfEvents];
                Marshal.Copy(eventsPtr, cEvents, 0, numOfEvents);

                foreach (IntPtr cEvent in cEvents)
                {
                    events.Add(new StatisticEvent(cEvent));
                }

                // Refresh objects
                foreach (XboxLiveUser user in m_localUsers)
                {
                    user.Impl.UpdatePropertiesFromXboxLiveUserPtr();
                }
            }

            return events.AsReadOnly();
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerSetStatisticNumberData(IntPtr user, string statName, double value, IntPtr errMessage);
        public void SetStatisticNumberData(XboxLiveUser user, string statName, double value)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerSetStatisticNumberData(user.Impl.GetPtr(), statName, value, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerSetStatisticIntegerData(IntPtr user, string statName, long value, IntPtr errMessage);
        public void SetStatisticIntegerData(XboxLiveUser user, string statName, long value)
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerSetStatisticIntegerData(user.Impl.GetPtr(), statName, value, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerSetStatisticStringData(IntPtr user, string statName, string value, IntPtr errMessage);
        public void SetStatisticStringData(XboxLiveUser user, string statName, string value)
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerSetStatisticStringData(user.Impl.GetPtr(), statName, value, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerDeleteStat(IntPtr user, string statName, IntPtr errMessage);
        public void DeleteStatistic(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerDeleteStat(user.Impl.GetPtr(), statName, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetLeaderboard(IntPtr user, string statName, IntPtr query, IntPtr errMessage);
        public void GetLeaderboard(XboxLiveUser user, string statName, LeaderboardQuery query)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetLeaderboard(user.Impl.GetPtr(), statName, query.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetSocialLeaderboard(IntPtr user, string statName, string socialGroup, IntPtr query, IntPtr errMessage);
        public void GetSocialLeaderboard(XboxLiveUser user, string statName, string socialGroup, LeaderboardQuery query)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetSocialLeaderboard(user.Impl.GetPtr(), statName, socialGroup, query.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetStat(IntPtr user, IntPtr statName, IntPtr statValue, IntPtr errMessage);
        public StatisticValue GetStatistic(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cStatValue = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cStatName = Marshal.StringToHGlobalAnsi(statName);

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetStat(user.Impl.GetPtr(), cStatName, cStatValue, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Handles returned objects
            StatisticValue statValue = new StatisticValue(Marshal.ReadIntPtr(cStatValue));
            Marshal.FreeHGlobal(cStatValue);

            return statValue;
        }

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT StatsManagerGetStatNames(IntPtr user, IntPtr statNameList, IntPtr statNameListSize, IntPtr errMessage);
        public IList<string> GetStatisticNames(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cStatListPtr = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cStatListSize = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            XSAPI_RESULT errCode = StatsManagerGetStatNames(user.Impl.GetPtr(), cStatListPtr, cStatListSize, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                // todo do something
            }

            // Handles returned objects
            int statListSize = Marshal.ReadInt32(cStatListSize);
            Marshal.FreeHGlobal(cStatListSize);

            List<string> statList = new List<string>();

            if (statListSize > 0)
            {
                IntPtr cListPtr = Marshal.ReadIntPtr(cStatListPtr);
                IntPtr[] cStatList = new IntPtr[statListSize];
                Marshal.Copy(cListPtr, cStatList, 0, statListSize);

                for (int i = 0; i < statListSize; i++)
                {
                    statList.Add(Marshal.PtrToStringAnsi(cStatList[i]));
                }
            }
            Marshal.FreeHGlobal(cStatListPtr);

            return statList;
        }
    }
}
