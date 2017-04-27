// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.CSharp;
using Microsoft.Xbox.Services.CSharp.Stats.Manager;

[Serializable]
public class StringStat : StatBase<string>
{
    private bool isLocalUserAdded = false;
    private void Awake()
    {
        StatsManagerComponent.Instance.LocalUserAdded += (sender, args) =>
        {
            StatValue statValue = XboxLive.Instance.StatsManager.GetStat(args.User, Name);
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
                XboxLive.Instance.StatsManager.SetStatAsString(XboxLiveComponent.Instance.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}