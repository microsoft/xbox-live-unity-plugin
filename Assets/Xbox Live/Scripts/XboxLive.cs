// -----------------------------------------------------------------------
//  <copyright file="XboxLive.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xbox.Services;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Handles initializing any Xbox Live functionality when the game starts.  If the game is not properly configured for Xbox Live this will result in errors.
/// </summary>
public class XboxLive : MonoBehaviour
{
    public const string ConfigurationFileName = "xboxservices.config";

    public XboxServicesConfiguration Configuration;

    private static XboxLive instance;

    private XboxLiveContext context;

    public static XboxLive Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<XboxLive>();

                if (instance == null)
                {
                    Debug.LogError("An instance of XboxLive is needed in the scene.");
                }
            }

            return instance;
        }
    }

    public bool IsConfigured
    {
        get
        {
            return this.Configuration != null;
        }
    }

    public XboxLiveContext Context { get; set; }

    public void Awake()
    {
        Debug.Log("Awake");
        DontDestroyOnLoad(this);

        this.Configuration = XboxServicesConfiguration.Load();
    }
}