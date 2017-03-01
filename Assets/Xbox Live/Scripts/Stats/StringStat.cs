// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.Stats.Manager;

[Serializable]
public class StringStat : StatBase<string>
{
    private bool isLocalUserAdded = false;
    private void Awake()
    {
        StatsManagerComponent.Instance.LocalUserAdded += (sender, args) =>
        {
            StatValue statValue = StatsManager.Singleton.GetStat(args.User, Name);
            if (statValue != null)
            {
                this.Value = statValue.AsString();
            }
            isLocalUserAdded = true;
        };
    }
    public override string Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if(isLocalUserAdded)
            {
                StatsManager.Singleton.SetStatAsString(XboxLive.Instance.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}