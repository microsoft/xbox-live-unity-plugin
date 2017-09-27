// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Leaderboard;
    using Microsoft.Xbox.Services.Shared;

    public class StatsManager : IStatsManager
    {
        private static readonly object instanceLock = new object();
        private static IStatsManager instance;

        private readonly List<XboxLiveUser> m_localUsers = new List<XboxLiveUser>();

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

        private delegate Int32 StatsManagerAddLocalUser(IntPtr user, IntPtr errMessage);
        public void AddLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerAddLocalUser>(user.Impl.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }

            // Does local work
            m_localUsers.Add(user);
        }

        private delegate Int32 StatsManagerRemoveLocalUser(IntPtr user, IntPtr errMessage);
        public void RemoveLocalUser(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerRemoveLocalUser>(user.Impl.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }

            // Does local work
            m_localUsers.Remove(user);
        }

        private delegate Int32 StatsManagerRequestFlushToService(IntPtr user, bool isHighPriority, IntPtr errMessage);
        public void RequestFlushToService(XboxLiveUser user, bool isHighPriority = false)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerRequestFlushToService>(user.Impl.GetPtr(), isHighPriority, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate IntPtr StatsManagerDoWork(IntPtr numOfEvents);
        public IList<StatEvent> DoWork()
        {
            // Allocates memory for returned objects
            IntPtr cNumOfEvents = Marshal.AllocHGlobal(Marshal.SizeOf<Int32>());

            // Invokes the c method
            IntPtr eventsPtr = XboxLive.Instance.Invoke<IntPtr, StatsManagerDoWork>(cNumOfEvents);

            // Does local work
            int numOfEvents = Marshal.ReadInt32(cNumOfEvents);
            Marshal.FreeHGlobal(cNumOfEvents);

            List<StatEvent> events = new List<StatEvent>();

            if (numOfEvents > 0)
            {
                IntPtr[] cEvents = new IntPtr[numOfEvents];
                Marshal.Copy(eventsPtr, cEvents, 0, numOfEvents);

                foreach (IntPtr cEvent in cEvents)
                {
                    events.Add(new StatEvent(cEvent));
                }

                // Refresh objects
                foreach (XboxLiveUser user in m_localUsers)
                {
                    user.Impl.UpdatePropertiesFromXboxLiveUser_c();
                }
            }

            return events.AsReadOnly();
        }

        private delegate Int32 StatsManagerSetStatisticNumberData(IntPtr user, string statName, double value, IntPtr errMessage);
        public void SetStatisticNumberData(XboxLiveUser user, string statName, double value)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerSetStatisticNumberData>(user.Impl.GetPtr(), statName, value, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate Int32 StatsManagerSetStatisticIntegerData(IntPtr user, string statName, long value, IntPtr errMessage);
        public void SetStatisticIntegerData(XboxLiveUser user, string statName, long value)
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerSetStatisticIntegerData>(user.Impl.GetPtr(), statName, value, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate Int32 StatsManagerSetStatisticStringData(IntPtr user, string statName, string value, IntPtr errMessage);
        public void SetStatisticStringData(XboxLiveUser user, string statName, string value)
        {
            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerSetStatisticStringData>(user.Impl.GetPtr(), statName, value, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate Int32 StatsManagerDeleteStat(IntPtr user, string statName, IntPtr errMessage);
        public void DeleteStat(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerDeleteStat>(user.Impl.GetPtr(), statName, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate Int32 StatsManagerGetLeaderboard(IntPtr user, string statName, IntPtr query, IntPtr errMessage);
        public void GetLeaderboard(XboxLiveUser user, string statName, LeaderboardQuery query)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = 0; // todo XboxLive.Instance.Invoke<Int32, StatsManagerGetLeaderboard>(user.Impl.GetPtr(), statName, query.GetPtr(), cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate Int32 StatsManagerGetSocialLeaderboard(IntPtr user, string statName, string socialGroup, IntPtr query, IntPtr errMessage);
        public void GetSocialLeaderboard(XboxLiveUser user, string statName, string socialGroup, LeaderboardQuery query)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = 0; // todo XboxLive.Instance.Invoke<Int32, StatsManagerGetSocialLeaderboard>(user.Impl.GetPtr(), statName, socialGroup, query.GetPtr() cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }
        }

        private delegate Int32 StatsManagerGetStat(IntPtr user, IntPtr statName, IntPtr statValue, IntPtr errMessage);
        public StatValue GetStat(XboxLiveUser user, string statName)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cStatValue = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cStatName = Marshal.StringToHGlobalAnsi(statName);

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerGetStat>(user.Impl.GetPtr(), cStatName, cStatValue, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
            {
                // todo do something
            }

            // Handles returned objects
            StatValue statValue = new StatValue(Marshal.ReadIntPtr(cStatValue));
            Marshal.FreeHGlobal(cStatValue);

            return statValue;
        }

        private delegate Int32 StatsManagerGetStatNames(IntPtr user, IntPtr statNameList, IntPtr statNameListSize, IntPtr errMessage);
        public IList<string> GetStatNames(XboxLiveUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            // Allocates memory for returned objects
            IntPtr cStatListPtr = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cStatListSize = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());
            IntPtr cErrMessage = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>());

            // Invokes the c method
            int errCode = XboxLive.Instance.Invoke<Int32, StatsManagerGetStatNames>(user.Impl.GetPtr(), cStatListPtr, cStatListSize, cErrMessage);

            // Handles error
            string errMessage = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(cErrMessage));
            Marshal.FreeHGlobal(cErrMessage);

            if (errCode > 0)
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct StatValue_c
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Name;

            [MarshalAs(UnmanagedType.R8)]
            public double AsNumber;

            [MarshalAs(UnmanagedType.I8)]
            public long AsInteger;

            [MarshalAs(UnmanagedType.LPStr)]
            public string AsString;

            [MarshalAs(UnmanagedType.U4)]
            public StatValueType DataType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StatEvent_c
        {
            [MarshalAs(UnmanagedType.U4)]
            public StatEventType EventType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr EventArgs;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr LocalUser;

            [MarshalAs(UnmanagedType.I4)]
            public int ErrorCode;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ErrorMessage;
        }
    }
}