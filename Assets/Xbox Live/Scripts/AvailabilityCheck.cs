// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public class AvailabilityCheck : MonoBehaviour
    {
        // The UI element to display when the device is offline
        public GameObject OfflineStatus;

        bool showingOffline;

        void Update()
        {
            bool offline = Application.internetReachability == NetworkReachability.NotReachable;
            if (showingOffline != offline)
            {
                OfflineStatus.SetActive(offline);
                showingOffline = offline;
            }
        }
    }
}