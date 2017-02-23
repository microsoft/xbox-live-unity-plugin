// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.Social.Manager;

using UnityEditor;

using UnityEngine;

public delegate void SocialEventHandler(object sender, SocialEvent socialEvent);

public class SocialManagerComponent : Singleton<SocialManagerComponent>
{
    private ISocialManager manager;

    public event SocialEventHandler EventProcessed;

    private SocialManagerComponent()
    {
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        this.manager = SocialManager.Instance;
    }

    private void Update()
    {
        try
        {
            var socialEvents = this.manager.DoWork();

            foreach (SocialEvent socialEvent in socialEvents)
            {
                Debug.LogFormat("[SocialManager] Processed {0} event.", socialEvent.EventType);
                this.OnEventProcessed(socialEvent);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    protected virtual void OnEventProcessed(SocialEvent socialEvent)
    {
        var handler = this.EventProcessed;
        if (handler != null) handler(this, socialEvent);
    }
}