// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Stats.Manager;

using UnityEngine;
using UnityEngine.EventSystems;

public class StatsManagerComponent : Singleton<StatsManagerComponent>
{
    private IStatsManager manager;

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        XboxLive.EnsureEnabled();
        this.manager = StatsManager.Singleton;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled
    /// </summary>
    private void Update()
    {
        if (this.manager == null)
        {
            Debug.LogWarning("Somehow the manager got nulled out.");
            return;
        }

        foreach (StatEvent statEvent in this.manager.DoWork())
        {
            Debug.Log(string.Format("[StatsManager] {0} - {1}", statEvent.EventType, statEvent.LocalUser.Gamertag));
            XboxLiveUser user = statEvent.LocalUser;

            switch (statEvent.EventType)
            {
                case StatEventType.LocalUserAdded:
                    ExecuteEvents.Execute<IStatsManagerEventHandler>(this.gameObject, null, (handler, b) => { handler.LocalUserAdded(user); });
                    break;
                case StatEventType.LocalUserRemoved:
                    ExecuteEvents.Execute<IStatsManagerEventHandler>(this.gameObject, null, (handler, b) => { handler.LocalUserAdded(user); });
                    break;
                case StatEventType.StatUpdateComplete:
                    ExecuteEvents.Execute<IStatsManagerEventHandler>(this.gameObject, null, (handler, b) => { handler.StatUpdateComplete(); });
                    break;
            }
        }
    }
}