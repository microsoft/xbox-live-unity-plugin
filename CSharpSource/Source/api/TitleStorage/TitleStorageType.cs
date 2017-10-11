// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.TitleStorage
{
    /// <summary> Defines values used to indicate title storage type.</summary>
    public enum TitleStorageType : int
    {
        /// <summary>
        /// Unknown storage type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Per-user data storage such as game state or game settings that can be only be accessed by Xbox One.
        /// User restrictions can be configured to public or owner only in the service configuration.
        /// </summary>
        TrustedPlatformStorage = 0,

        /// <summary>
        /// Global data storage.  This storage type is only writable via the Xbox Developer Portal (XDP).
        /// Any platform may read from this storage type. Data could be rosters, maps, challenges, art resources, etc.
        /// </summary>
        GlobalStorage = 2,

        /// <summary>
        /// Per-user data storage such as game state or game settings the can be accessed by Xbox One, Windows 10, and Windows Phone 10 devices
        /// User restrictions can be configured to public or owner only in the service configuration.
        /// </summary>
        Universal = 5
    }

}
