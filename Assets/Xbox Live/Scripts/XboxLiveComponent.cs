// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

using Microsoft.Xbox.Services.CSharp;

using UnityEngine;

/// <summary>
/// Handles initializing any Xbox Live functionality when the game starts.  If the game is not properly configured for Xbox Live this will result in errors.
/// </summary>
[HelpURL("http://github.com/Microsoft/xbox-live-unity-plugin")]
public class XboxLiveComponent : Singleton<XboxLiveComponent>
{
    protected XboxLiveComponent()
    {
    }

    public XboxLiveUser User { get; set; }

    public void Awake()
    {
        // Super simple check to determine if configuration is non-empty.  This is not a thorough check to determine if the configuration is valid.
        // A user can easly bypass this check which will just cause them to fail at runtime if they try to use any functionality.
        if (XboxLive.Instance.AppConfig == null || XboxLive.Instance.AppConfig.TitleId == 0 && Application.isPlaying)
        {
            const string message = "Xbox Live is not configured, but the game is attempting to use Xbox Live functionality.  You must update the configuration in 'Xbox Live > Configuration' before building the game to enable Xbox Live.";
            if (Application.isEditor)
            {
                Debug.LogWarning(message);
            }
            else
            {
                Debug.LogError(message);
            }
        }

        MockXboxLiveData.Load(Path.Combine(Application.dataPath, "MockData.json"));

        // TODO: Move user handling into the XboxLive class UserManager.  Until then, this class will just be used as the cache for the individual XboxLiveUser object.
        this.User = new XboxLiveUser();
    }
}