// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XboxLiveDebugManager : Singleton<XboxLiveDebugManager> {

    public bool DebugLogsOn = true;

    private void Awake()
    {
        // Ensure that a XboxLiveDebugManager Instance has been created.
        var manager = XboxLiveDebugManager.Instance;
        DontDestroyOnLoad(this);
    }


    /// <summary>
    /// Ensures that there is an Xbox Live Debug Manager instance on the scene used to configure debug settings of Xbox Live Prefabs
    /// </summary>
    public static void EnsureXboxLiveDebugManager()
    {
        if (Object.FindObjectOfType<XboxLiveDebugManager>() == null)
        {
            Debug.LogErrorFormat("Make sure to drag at least one instance of the XboxLiveDebugManager prefab on your initial scene.");
        }
    }
}
