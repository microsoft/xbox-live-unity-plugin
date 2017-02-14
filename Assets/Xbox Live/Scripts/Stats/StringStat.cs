using System;

using Microsoft.Xbox.Services.Stats.Manager;

[Serializable]
public class StringStat : StatBase<string>
{
    public override void SetValue(string value)
    {
        this.Value = value;
        StatsManager.Singleton.SetStatAsString(XboxLive.Instance.User, Name, Value);
    }
}