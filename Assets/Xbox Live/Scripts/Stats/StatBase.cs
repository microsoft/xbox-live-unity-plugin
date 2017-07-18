// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
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
    public XboxLiveUserInfo XboxLiveUser;
    protected bool isLocalUserAdded = false;
    private bool LocalUserAddedSetup = false;

    /// <summary>
    /// The name of the stat that is published to the stats service.
    /// </summary>
    [Tooltip("The ID of the stat that is published to the stats service.")]
    public string ID;

    /// <summary>
    /// A friendly name for the stat that can be used for display purposes.
    /// </summary>
    [Tooltip("A friendly name for the stat that can be used for display purposes.")]
    public string DisplayName;

    private void Awake()
    {
        XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();

        // Ensure that a StatsManager has been created so that stats will be sync with the service as they are modified.
        var statsManager = StatsManagerComponent.Instance;
    }

    void Start()
    {
        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }

        if (this.XboxLiveUser != null && this.XboxLiveUser.User != null && this.XboxLiveUser.User.IsSignedIn)
        {
            this.HandleGetStat(this.XboxLiveUser.User, this.ID);
        }
    }

    protected void Update()
    {
        if (this.XboxLiveUser != null && this.XboxLiveUser.User != null && this.XboxLiveUser.User.IsSignedIn
            && !this.isLocalUserAdded && !this.LocalUserAddedSetup)
        {
            StatsManagerComponent.Instance.LocalUserAdded += (sender, args) =>
                {
                    if (args.User.Gamertag == this.XboxLiveUser.User.Gamertag)
                    {
                        this.HandleGetStat(args.User, this.ID);
                    }
                };
            this.LocalUserAddedSetup = true;
        }
        else
        {
            if (this.XboxLiveUser == null)
            {
                this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
            }
        }
    }

    protected abstract void HandleGetStat(XboxLiveUser user, string statName);
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