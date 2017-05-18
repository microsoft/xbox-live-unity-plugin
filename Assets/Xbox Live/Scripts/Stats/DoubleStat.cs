// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

[Serializable]
public class DoubleStat : StatBase<double>
{
    private bool isLocalUserAdded = false;
    private void Awake()
    {
        if (XboxLiveComponent.Instance.User == null || !XboxLiveComponent.Instance.User.IsSignedIn)
        {
            StatsManagerComponent.Instance.LocalUserAdded += (sender, args) =>
            {
                this.HandleGetStat(args.User, this.Name);
            };
        }
        else
        {
            this.HandleGetStat(XboxLiveComponent.Instance.User, this.Name);
        }
    }

    private void HandleGetStat(XboxLiveUser user, string statName)
    {
        StatValue statValue = XboxLive.Instance.StatsManager.GetStat(user, statName);
        if (statValue != null)
        {
            this.Value = statValue.AsNumber();
        }
        this.isLocalUserAdded = true;
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
                XboxLive.Instance.StatsManager.SetStatAsNumber(XboxLiveComponent.Instance.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}