// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine.EventSystems;

public interface ISocialManagerEventHandler : IEventSystemHandler
{
    void OnSocialManagerEvent(SocialEvent socialEvent);
}