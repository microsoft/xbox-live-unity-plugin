// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.CSharp;
using Microsoft.Xbox.Services.CSharp.Stats.Manager;

[Serializable]
public class DoubleStat : StatBase<double>
{
    private bool isLocalUserAdded = false;
    private void Awake()
    {
        StatsManagerComponent.Instance.LocalUserAdded += (sender, args) =>
        {
            StatValue statValue = XboxLive.Instance.StatsManager.GetStat(args.User, Name);
            if (statValue != null)
            {
                this.Value = statValue.AsNumber();
            }
            isLocalUserAdded = true;
        };
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
            if(isLocalUserAdded)
            {
                XboxLive.Instance.StatsManager.SetStatAsNumber(XboxLiveComponent.Instance.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}