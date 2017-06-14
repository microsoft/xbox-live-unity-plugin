// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

public class XboxLiveUserManager : Singleton<XboxLiveUserManager>
{

    public List<XboxLiveUserInfo> AvailableXboxLiveUsers { get; private set; }

    public XboxLiveUserInfo UserForSingleUserMode { get; set; }

    public bool SingleUserModeEnabled { get; private set; }

    public bool IsInitialized { get; private set; }

    // Use this for initialization
    public void Initialize()
    {
        this.AvailableXboxLiveUsers = new List<XboxLiveUserInfo>();
        var xboxLiveUserInstances = FindObjectsOfType<XboxLiveUserInfo>();
        foreach (var xboxLiveUserInstance in xboxLiveUserInstances)
        {
            this.AvailableXboxLiveUsers.Add(xboxLiveUserInstance);
        }

        this.SingleUserModeEnabled = (xboxLiveUserInstances.Length == 0) || (xboxLiveUserInstances.Length == 1);

        this.IsInitialized = true;
    }

    public void Reset()
    {
        this.UserForSingleUserMode = null;
        this.Initialize();
    }

    public XboxLiveUserInfo GetSingleModeUser()
    {
        return Instance.SingleUserModeEnabled ? Instance.UserForSingleUserMode : null;
    }
}
