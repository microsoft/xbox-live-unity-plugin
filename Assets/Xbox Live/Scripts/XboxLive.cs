// -----------------------------------------------------------------------
//  <copyright file="XboxLive.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Xbox.Services;

using UnityEngine;

/// <summary>
/// Handles initializing any Xbox Live functionality when the game starts.  If the game is not properly configured for Xbox Live this will result in errors.
/// </summary>
[HelpURL("http://github.com/Microsoft/xbox-live-unity-plugin")]
public class XboxLive : Singleton<XboxLive>
{
    protected XboxLive()
    {
    }

    public XboxLiveAppConfiguration Configuration { get; set; }

    public static bool IsEnabled
    {
        get
        {
            return Instance.Configuration != null;
        }
    }

    public XboxLiveUser User { get; set; }

    public XboxLiveContext Context { get; set; }

    public static void EnsureEnabled()
    {
        if (!IsEnabled)
        {
            throw new InvalidOperationException("Xbox Live must be enabled.  Run the Xbox Live Association Wizard from Xbox Live > Configuration before using this feature.");
        }
    }

    public void Awake()
    {
        MockXboxLiveData.Load(Path.Combine(Application.dataPath, "MockData.json"));

        try
        {
            this.Configuration = XboxLiveAppConfiguration.Instance;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("You must associate your game with an Xbox Live Title in order to use Xbox Live functionality.", e);
        }
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