// -----------------------------------------------------------------------
//  <copyright file="XboxLive.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;
using Microsoft.Xbox.Services.System;

using UnityEngine;

/// <summary>
/// Handles initializing any Xbox Live functionality when the game starts.  If the game is not properly configured for Xbox Live this will result in errors.
/// </summary>
[HelpURL("http://github.com/Microsoft/xbox-live-unity-plugin")]
public class XboxLive : MonoBehaviour
{
    private static bool applicationIsQuitting = false;

    private static readonly object createInstanceLock = new object();

    private static volatile XboxLive instance;

    protected XboxLive()
    {
    }

    public static XboxLive Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[XboxLive] Application is exiting and instance has already been destroyed");
                return null;
            }

            if (instance == null)
            {
                lock (createInstanceLock)
                {
                    if (instance == null)
                    {
                        // Check for an existing instance
                        instance = FindObjectOfType<XboxLive>();

                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<XboxLive>();
                            singleton.name = "(singleton) XboxLive";

                            if (Application.isPlaying)
                            {
                                DontDestroyOnLoad(singleton);

                                MockXboxLiveData.Load(Path.Combine(Application.dataPath, "MockData.json"));
                            }
                        }
                    }
                }
            }

            return instance;
        }
    }

    public XboxServicesConfiguration Configuration { get; set; }

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
        DontDestroyOnLoad(this);

        this.Configuration = XboxServicesConfiguration.Load();
        if (this.Configuration == null)
        {
            throw new InvalidOperationException("You must associate your game with an Xbox Live Title in order to use Xbox Live functionality.");
        }
    }

    public void Update()
    {
        SocialManager.Instance.DoWork();
    }

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public IEnumerator SignInAsync()
    {
        this.User = new XboxLiveUser();
        yield return this.User.SignInAsync().AsCoroutine();

        this.Context = new XboxLiveContext(this.User);
        yield break;
    }
}