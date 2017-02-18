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
        this.SetValue(this.Value * multiplier);
    }

    public override void SetValue(double value)
    {
        base.SetValue(value);
        StatsManager.Singleton.SetStatAsNumber(XboxLive.Instance.User, this.Name, this.Value);
        StatsManager.Singleton.RequestFlushToService(XboxLive.Instance.User, true);
    }
}