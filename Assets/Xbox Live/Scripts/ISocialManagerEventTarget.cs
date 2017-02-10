// -----------------------------------------------------------------------
//  <copyright file="ISocialManagerEventTarget.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine.EventSystems;

public interface ISocialManagerEventHandler : IEventSystemHandler
{
    void OnSocialManagerEvent(SocialEvent socialEvent);
}