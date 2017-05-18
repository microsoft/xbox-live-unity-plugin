// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

using UnityEngine;
using System.Collections.Generic;

public class StatsManagerComponent : Singleton<StatsManagerComponent>
{
    public event EventHandler<XboxLiveUserEventArgs> LocalUserAdded;

    public event EventHandler<XboxLiveUserEventArgs> LocalUserRemoved;

    public event EventHandler<XboxLivePrefab.StatEventArgs> GetLeaderboardCompleted;

    public event EventHandler StatUpdateComplete;

    private IStatsManager manager;

    protected StatsManagerComponent()
    {
    }

    private void Awake()
    {
        this.manager = XboxLive.Instance.StatsManager;
    }

    private void Update()
    {
        if (this.manager == null)
        {
            Debug.LogWarning("Somehow the manager got nulled out.");
            return;
        }
        List<StatEvent> events = this.manager.DoWork();
        foreach (StatEvent statEvent in events)
        {
            Debug.Log(string.Format("[StatsManager] Processed {0} event for {1}.", statEvent.EventType, statEvent.LocalUser.Gamertag));

            switch (statEvent.EventType)
            {
                case StatEventType.LocalUserAdded:
                    this.OnLocalUserAdded(statEvent.LocalUser);
                    break;
                case StatEventType.LocalUserRemoved:
                    this.OnLocalUserRemoved(statEvent.LocalUser);
                    break;
                case StatEventType.StatUpdateComplete:
                    this.OnStatUpdateComplete();
                    break;
                case StatEventType.GetLeaderboardComplete:
                    this.OnGetLeaderboardCompleted(new XboxLivePrefab.StatEventArgs(statEvent));
                    break;
            }
        }
    }

    public void RequestFlushToService(System.Boolean isHighPriority)
    {
        this.manager.RequestFlushToService(XboxLiveComponent.Instance.User, isHighPriority);
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

    protected virtual void OnGetLeaderboardCompleted(XboxLivePrefab.StatEventArgs statEvent)
    {
        var handler = this.GetLeaderboardCompleted;
        if (handler != null) handler(this, statEvent);
    }
}