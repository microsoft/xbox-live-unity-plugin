// -----------------------------------------------------------------------
//  <copyright file="Presence.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;

using Microsoft.Xbox.Services.Presence;

using UnityEngine;

[Serializable]
public class Presence : MonoBehaviour
{
    public string presenceId;

    public void UpdatePresence()
    {
        this.UpdatePresence(this.presenceId);
    }

    public void UpdatePresence(string presenceId)
    {
        this.StartCoroutine(this.UpdatePresenceAsync(presenceId));
    }

    private IEnumerator UpdatePresenceAsync(string presenceId)
    {
        XboxLive.EnsureEnabled();

        PresenceData data = new PresenceData(XboxLive.Instance.Configuration.ServiceConfigurationId, presenceId);
        yield return XboxLive.Instance.Context.PresenceService.SetPresenceAsync(true, data).AsCoroutine(); ;
    }
}