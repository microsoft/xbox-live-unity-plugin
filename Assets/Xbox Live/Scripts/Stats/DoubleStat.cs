// -----------------------------------------------------------------------
//  <copyright file="DoubleStat.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using Microsoft.Xbox.Services.Stats.Manager;

[Serializable]
public class DoubleStat : StatBase<double>
{
    public void Multiply(double multiplier)
    {
        this.SetValue(Value * multiplier);
    }

    public override void SetValue(double value)
    {
        Value = value;
        StatsManager.Singleton.SetStatAsNumber(XboxLive.Instance.User, Name, Value);
    }
}