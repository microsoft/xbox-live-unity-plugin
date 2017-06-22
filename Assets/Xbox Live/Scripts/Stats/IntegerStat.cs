// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

/// <summary>
/// The actual integer value of the for the stat.
/// </summary>
/// <remarks>
/// Yes, this should be a long, but Unity doesn't save seem to properly serialize long values.
/// </remarks>
[Serializable]
public class IntegerStat : StatBase<int>
{
    protected override void HandleGetStat(XboxLiveUser user, string statName)
    {
        this.isLocalUserAdded = true;
        StatValue statValue = XboxLive.Instance.StatsManager.GetStat(user, statName);
        if (statValue != null)
        {
            this.Value = statValue.AsInteger();
        }
    }

    public void Increment()
    {
        this.Value = this.Value + 1;
    }

    public void Decrement()
    {
        this.Value = this.Value - 1;
    }

    public override int Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if (this.isLocalUserAdded)
            {
                XboxLive.Instance.StatsManager.SetStatAsInteger(this.XboxLiveUser.User, this.ID, value);
            }
            base.Value = value;
        }
    }
}