// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

using Microsoft.Xbox.Services;

using UnityEngine;

/// <summary>
/// Handles initializing any Xbox Live functionality when the game starts.  If the game is not properly configured for Xbox Live this will result in errors.
/// </summary>
[HelpURL("http://github.com/Microsoft/xbox-live-unity-plugin")]
public class XboxLive : Singleton<XboxLive>
{
    private static readonly bool isConfigured;

    static XboxLive()
    {
        try
        {
            isConfigured = XboxLiveAppConfiguration.Instance != null;
        }
        catch (Exception)
        {
            isConfigured = false;
        }
    }

    protected XboxLive()
    {
    }

    public static bool IsConfigured
    {
        get
        {
            return isConfigured;
        }
    }

    public XboxLiveUser User { get; set; }

    public XboxLiveContext Context { get; set; }

    public static void EnsureConfigured()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("Xbox Live must be enabled.  Run the Xbox Live Association Wizard from Xbox Live > Configuration before using this feature.");
        }
    }

    public void Awake()
    {
        // Super simple check to determine if configuration is non-empty.  This is not a thorough check to determine if the configuration is valid.
        // A user can easly bypass this check which will just cause them to fail at runtime if they try to use any functionality.
        if (XboxLiveAppConfiguration.Instance.TitleId == 0 && Application.isPlaying)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning("Xbox Live is not configured, but the game is attempting to use Xbox Live functionality.  You must update the configuration in 'Xbox Live > Configuration' before building the game to enable Xbox Live.");
            }
            else
            {
                Debug.LogError("Xbox Live must be enabled.  Run the Xbox Live Association Wizard from Xbox Live > Configuration before using any Xbox Live features.");
            }
        }

        MockXboxLiveData.Load(Path.Combine(Application.dataPath, "MockData.json"));
    }

    public IEnumerator SignInAsync()
    {
        this.User = new XboxLiveUser();

        TaskYieldInstruction<SignInResult> signInTask = this.User.SignInAsync().AsCoroutine();
        yield return signInTask;

        // Throw any exceptions if needed.
        if (signInTask.Result.Status != SignInStatus.Success)
        {
            throw new Exception("Sign in Failed");
        }

        this.Context = new XboxLiveContext(this.User);
    }
}