// -----------------------------------------------------------------------
//  <copyright file="IntegerStat.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using Microsoft.Xbox.Services.Stats.Manager;

/// <summary>
/// The actual integer value of the for the stat.
/// </summary>
/// <remarks>
/// Yes, this should be a long, but Unity doesn't save seem to properly serialize long values.
/// </remarks>
[Serializable]
public class IntegerStat : StatBase<int>
{
    public void Increment()
    {
        this.SetValue(this.Value + 1);
    }

    public void Decrement()
    {
        this.SetValue(this.Value - 1);
    }

    public override void SetValue(int value)
    {
        base.SetValue(value);
        StatsManager.Singleton.SetStatAsInteger(XboxLive.Instance.User, this.Name, this.Value);
        StatsManager.Singleton.RequestFlushToService(XboxLive.Instance.User, true);
    }
}