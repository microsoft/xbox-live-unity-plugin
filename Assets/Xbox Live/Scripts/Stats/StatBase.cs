// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Stats.Manager;

using UnityEngine;

/// <summary>
/// A simple base class to allow us to bind to a stat of any type.
/// </summary>
/// <remarks>
/// All stat classes should derive from <see cref="StatBase{T}"/> instead of from <see cref="StatBase"/>.
/// This class should only be used when you need to be able to bind to a stat regardless of the underlying value.
/// </remarks>
[Serializable]
public abstract class StatBase : MonoBehaviour
{
    /// <summary>
    /// The name of the stat that is published to the stats service.
    /// </summary>
    public string Name;

    /// <summary>
    /// A friendly name for the stat that can be used for display purposes.
    /// </summary>
    public string DisplayName;

    private void Awake()
    {
        // Ensure that a StatsManager has been created so that stats will be
        // pushed up to the service as they are modified.
        IStatsManager statsManager = StatsManager.Singleton;
    }
}

/// <summary>
/// A generic base class for all stats.  Wraps 
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class StatBase<T> : StatBase, IStatsManagerEventHandler
{
    /// <summary>
    /// Indicates whether or not the initial value of the stat has been recieved from the stats service yet.
    /// </summary>
    private bool initialized;

    public T Value;

    public bool UseInitialValueFromService;

    public virtual void SetValue(T value)
    {
        this.Value = value;
    }

    public void LocalUserAdded(XboxLiveUser user)
    {
        this.EnsureInitialized();
    }

    public void LocalUserRemoved(XboxLiveUser user)
    {
    }

    public void StatUpdateComplete()
    {
        // Grab the value and store it locally.
        StatValue statValue = StatsManager.Singleton.GetStat(XboxLive.Instance.User, this.Name);
        this.SetValue((T)statValue.Value);
    }

    public override string ToString()
    {
        return this.Value != null ? this.Value.ToString() : string.Empty;
    }

    private void EnsureInitialized()
    {
        if (this.initialized) return;

        if (!this.UseInitialValueFromService)
        {
            // Set the initial stat value.
            this.SetValue(this.Value);
        }
        else
        {
            StatValue statValue = StatsManager.Singleton.GetStat(XboxLive.Instance.User, this.Name);
            this.SetValue(statValue != null ? (T)statValue.Value : this.Value);

        }

        this.initialized = true;
    }
}