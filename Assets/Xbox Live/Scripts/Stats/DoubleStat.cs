// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.Stats.Manager;

[Serializable]
public class DoubleStat : StatBase<double>
{
    public void Multiply(float multiplier)
    {
        this.Value = this.Value * multiplier;
    }

    public override double Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            StatsManager.Singleton.SetStatAsNumber(XboxLive.Instance.User, this.Name, value);
            base.Value = value;
        }
    }
}