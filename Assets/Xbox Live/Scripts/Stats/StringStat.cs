// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

[Serializable]
public class StringStat : StatBase<string>
{
    protected override void HandleGetStat(XboxLiveUser user, string statName)
    {
        this.isLocalUserAdded = true;
        StatValue statValue = XboxLive.Instance.StatsManager.GetStat(user, statName);
        if (statValue != null)
        {
            this.Value = statValue.AsString();
        }
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
                XboxLive.Instance.StatsManager.SetStatAsString(this.XboxLiveUser.User, this.Name, value);
            }
            base.Value = value;
        }
    }
}