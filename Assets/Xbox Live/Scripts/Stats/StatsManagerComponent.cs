// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using Microsoft.Xbox.Services.Statistics.Manager;

using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.Xbox.Services.Client
{
    public class StatsManagerComponent : Singleton<StatsManagerComponent>
    {
        public event EventHandler<XboxLiveUserEventArgs> LocalUserAdded;

        public event EventHandler<XboxLiveUserEventArgs> LocalUserRemoved;

        public event EventHandler<StatEventArgs> GetLeaderboardCompleted;

        public event EventHandler StatUpdateComplete;

        private IStatisticManager manager;

        protected StatsManagerComponent()
        {
        }

        private void Awake()
        {
            this.manager = XboxLive.Instance.StatsManager;
        }

        private void Update()
        {
            if (this.manager == null && XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogWarning("Somehow the manager got nulled out.");
                return;
            }
            IList<StatisticEvent> events = this.manager.DoWork();
            foreach (StatisticEvent statEvent in events)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogFormat("[StatsManager] Processed {0} event for {1}.", statEvent.EventType, statEvent.User.Gamertag);
                }

                switch (statEvent.EventType)
                {
                    case StatisticEventType.LocalUserAdded:
                        this.OnLocalUserAdded(statEvent.User);
                        break;
                    case StatisticEventType.LocalUserRemoved:
                        this.OnLocalUserRemoved(statEvent.User);
                        break;
                    case StatisticEventType.StatisticUpdateComplete:
                        this.OnStatUpdateComplete();
                        break;
                    case StatisticEventType.GetLeaderboardComplete:
                        this.OnGetLeaderboardCompleted(new StatEventArgs(statEvent));
                        break;
                }
            }
        }

        public void RequestFlushToService(XboxLiveUser user, bool isHighPriority)
        {
            this.manager.RequestFlushToService(user, isHighPriority);
        }

        protected virtual void OnLocalUserAdded(XboxLiveUser user)
        {
            var handler = this.LocalUserAdded;
            if (handler != null) handler(this, new XboxLiveUserEventArgs(user));
        }

        protected virtual void OnLocalUserRemoved(XboxLiveUser user)
        {
            var handler = this.LocalUserRemoved;
            if (handler != null) handler(this, new XboxLiveUserEventArgs(user));
        }

        protected virtual void OnStatUpdateComplete()
        {
            var handler = this.StatUpdateComplete;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnGetLeaderboardCompleted(StatEventArgs statEvent)
        {
            var handler = this.GetLeaderboardCompleted;
            if (handler != null) handler(this, statEvent);
        }
    }
}