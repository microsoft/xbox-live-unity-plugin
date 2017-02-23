// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;
using UnityEngine.EventSystems;

public class SocialManagerComponent : Singleton<SocialManagerComponent>
{
    private ISocialManager manager;

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

    void Update()
    {
        try
        {
            var socialEvents = this.manager.DoWork();
            Debug.Log(string.Format("SocialManager processed {0} events", socialEvents.Count));

            foreach (SocialEvent socialEvent in socialEvents)
            {
                SocialEvent eventData = socialEvent;
                ExecuteEvents.Execute<ISocialManagerEventHandler>(null, null, (a, b) => { a.OnSocialManagerEvent(eventData); });
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}