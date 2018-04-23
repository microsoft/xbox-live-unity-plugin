// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public class XboxLiveServicesSettings : Singleton<XboxLiveServicesSettings>
    {

        public bool DebugLogsOn = true;

        private void Awake()
        {
            // Ensure that a XboxLiveServicesSettings Instance has been created.
            var manager = XboxLiveServicesSettings.Instance;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            ExceptionManager.Instance.OnAnyException(this.HandleExceptions);
        }


        /// <summary>
        /// Ensures that there is an Xbox Live Debug Manager instance on the scene used to configure debug settings of Xbox Live Prefabs
        /// </summary>
        public static void EnsureXboxLiveServicesSettings()
        {
            if (Object.FindObjectOfType<XboxLiveServicesSettings>() == null)
            {
                Debug.LogErrorFormat("Make sure to drag at least one instance of the XboxLiveServices prefab on your initial scene.");
            }
        }

        public static bool EnsureXboxLiveServiceConfiguration() {
            return XboxLive.Instance.AppConfig != null && XboxLive.Instance.AppConfig.ServiceConfigurationId != null;
        }

        private void HandleExceptions(XboxLiveException ex) {
            if (DebugLogsOn) {
                Debug.LogWarning("An exception occured in " + ex.Source + "of type: " + ex.Type + " Details: " + ex.InnerException.Message);
            }
        }
    }
}