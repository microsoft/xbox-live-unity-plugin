// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

[Serializable]
public class DoubleStat : StatBase<double>
{
    protected override void HandleGetStat(XboxLiveUser user, string statName)
    {
        this.isLocalUserAdded = true;
        StatisticValue statValue = XboxLive.Instance.StatsManager.GetStatistic(user, statName);
        if (statValue != null)
        {
            this.Value = statValue.AsNumber;
        }
    }

    public void Multiply(float multiplier)
    {
        this.Value = this.Value * multiplier;
    }

    public void Square()
    {
        var value = this.Value;
        this.Value = value * value;
    }

    public override double Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if (this.isLocalUserAdded)
            {
                XboxLive.Instance.StatsManager.SetStatisticNumberData(this.XboxLiveUser.User, this.ID, value);
            }
            base.Value = value;
        }
    }
}