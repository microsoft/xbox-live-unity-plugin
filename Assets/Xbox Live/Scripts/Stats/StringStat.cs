// -----------------------------------------------------------------------
//  <copyright file="StringStat.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using Microsoft.Xbox.Services.Stats.Manager;

[Serializable]
public class StringStat : StatBase<string>
{
    public override void SetValue(string value)
    {
        base.SetValue(value);
        StatsManager.Singleton.SetStatAsString(XboxLive.Instance.User, this.Name, this.Value);
        StatsManager.Singleton.RequestFlushToService(XboxLive.Instance.User, true);
    }
}