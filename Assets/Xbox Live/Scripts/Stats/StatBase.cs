// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.Statistics.Manager;

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
    [Tooltip("The name of the stat that is published to the stats service.")]
    public string Name;

    /// <summary>
    /// A friendly name for the stat that can be used for display purposes.
    /// </summary>
    [Tooltip("A friendly name for the stat that can be used for display purposes.")]
    public string DisplayName;

    private void Awake()
    {
        // Ensure that a StatsManager has been created so that stats will be sync with the service as they are modified.
        var statsManager = StatsManagerComponent.Instance;
    }
}

/// <summary>
/// A generic base class for all stats.  Wraps 
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class StatBase<T> : StatBase
{
    private T value;

    public virtual T Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = value;
        }
    }

    public override string ToString()
    {
        return this.Value != null ? this.Value.ToString() : string.Empty;
    }
}