// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xbox.Services;

using UnityEngine;

#if NETFX_Core
using Windows.System;
#endif

public class XboxLiveUserInfo : MonoBehaviour
{
    public XboxLiveUser User { get; private set; }

#if NETFX_CORE
    public Windows.System.User WindowsSystemUser { get; set; }
#endif

    public void Start()
    {
        // Super simple check to determine if configuration is non-empty.  This is not a thorough check to determine if the configuration is valid.
        // A user can easly bypass this check which will just cause them to fail at runtime if they try to use any functionality.
        if (XboxLive.Instance.AppConfig == null || XboxLive.Instance.AppConfig.TitleId == 0 && Application.isPlaying)
        {
            const string message = "Xbox Live is not configured, but the game is attempting to use Xbox Live functionality.  You must update the configuration in 'Xbox Live > Configuration' before building the game to enable Xbox Live.";
            if (Application.isEditor && XboxLiveDebugManager.Instance.DebugLogsOn)
            {
                Debug.LogWarning(message);
            }
            else
            {
                if (XboxLiveDebugManager.Instance.DebugLogsOn)
                {
                    Debug.LogError(message);
                }
            }
        }

        MockXboxLiveData.Load(Path.Combine(Application.dataPath, "MockData.json"));
    }

    public void Initialize()
    {
#if NETFX_CORE
        this.InitializeWithWindowsSystemUser();
#else
        this.User = new XboxLiveUser();
#endif
    }

    private void InitializeWithWindowsSystemUser()
    {
#if NETFX_CORE
        if (this.WindowsSystemUser != null)
        {
            this.User = new XboxLiveUser(this.WindowsSystemUser);
        }
        else
        {
            this.User = new XboxLiveUser();
        }
#endif
    }
}
