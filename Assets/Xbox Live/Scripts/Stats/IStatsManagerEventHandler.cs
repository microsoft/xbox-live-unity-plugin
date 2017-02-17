// -----------------------------------------------------------------------
//  <copyright file="IStatsManagerEventHandler.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xbox.Services;

using UnityEngine.EventSystems;

public interface IStatsManagerEventHandler : IEventSystemHandler
{
    void LocalUserAdded(XboxLiveUser user);

    void LocalUserRemoved(XboxLiveUser user);

    void StatUpdateComplete();
}