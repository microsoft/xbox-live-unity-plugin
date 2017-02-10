// -----------------------------------------------------------------------
//  <copyright file="StatsManagerComponent.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xbox.Services.Stats.Manager;

using UnityEngine;

public class StatsManagerComponent : Singleton<StatsManagerComponent>
{
    private IStatsManager manager;

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
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
            switch (statEvent.EventType)
            {
                case StatEventType.LocalUserAdded:
                case StatEventType.LocalUserRemoved:
                case StatEventType.StatUpdateComplete:
                    Debug.Log(string.Format("[StatsManager] {0} - {1}", statEvent.EventType, statEvent.LocalUser.Gamertag));
                    break;
            }
        }
    }
}