// -----------------------------------------------------------------------
//  <copyright file="StatBase.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Stats.Manager;

using UnityEngine;

[Serializable]
public abstract class StatBase : MonoBehaviour
{
    public string Name;

    public abstract string ValueString { get; }
}

[Serializable]
public abstract class StatBase<T> : StatBase, IStatsManagerEventHandler
{
    public T Value;

    public override string ValueString
    {
        get
        {
            return this.Value.ToString();
        }
    }

    public abstract void SetValue(T value);

    public void LocalUserAdded(XboxLiveUser user)
    {
        // Set the initial stat value.
        this.SetValue(this.Value);
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
}