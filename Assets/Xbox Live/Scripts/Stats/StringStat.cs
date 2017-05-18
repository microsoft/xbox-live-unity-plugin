// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

[Serializable]
public class StringStat : StatBase<string>
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
            this.Value = statValue.AsString();
        }
        this.isLocalUserAdded = true;
    }

    public override string Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if (this.isLocalUserAdded)
            {
                XboxLive.Instance.StatsManager.SetStatAsString(XboxLiveComponent.Instance.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}