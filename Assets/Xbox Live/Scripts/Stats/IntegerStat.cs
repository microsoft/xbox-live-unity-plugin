// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.CSharp;
using Microsoft.Xbox.Services.CSharp.Stats.Manager;

/// <summary>
/// The actual integer value of the for the stat.
/// </summary>
/// <remarks>
/// Yes, this should be a long, but Unity doesn't save seem to properly serialize long values.
/// </remarks>
[Serializable]
public class IntegerStat : StatBase<int>
{
    private bool isLocalUserAdded = false;
    private void Awake()
    {
        StatsManagerComponent.Instance.LocalUserAdded += (sender, args) =>
        {
            StatValue statValue = XboxLive.Instance.StatsManager.GetStat(args.User, Name);
            if(statValue != null)
            {
                this.Value = statValue.AsInteger();
            }
            isLocalUserAdded = true;
        };
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
            if(isLocalUserAdded)
            {
                XboxLive.Instance.StatsManager.SetStatAsInteger(XboxLiveComponent.Instance.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}