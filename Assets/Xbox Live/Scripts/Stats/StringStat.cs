// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.Stats.Manager;

[Serializable]
public class StringStat : StatBase<string>
{
    public override string Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            StatsManager.Singleton.SetStatAsString(XboxLive.Instance.User, this.Name, value);
            base.Value = value;
        }
    }
}