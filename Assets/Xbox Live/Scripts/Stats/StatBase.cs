// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    /// <summary>
    /// A simple base class to allow us to bind to a stat of any type.
    /// </summary>
    /// <remarks>
    /// All stat classes should derive from <see cref="StatBase{T}"/> instead of from <see cref="StatBase"/>.
    /// This class should only be used when you need to be able to bind to a stat regardless of the underlying value.
    /// </remarks>
    [Serializable]
    public abstract class StatBase : MonoBehaviour
    {
        public int PlayerNumber = 1;
        protected bool isLocalUserAdded = false;
        private bool LocalUserAddedSetup = false;
        internal XboxLiveUser xboxLiveUser;
        /// <summary>
        /// The name of the stat that is published to the stats service.
        /// </summary>
        [Tooltip("The ID of the stat that is published to the stats service.")]
        public string ID;

        /// <summary>
        /// A friendly name for the stat that can be used for display purposes.
        /// </summary>
        [Tooltip("A friendly name for the stat that can be used for display purposes.")]
        public string DisplayName;

        private void Awake()
        {
            XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();

            // Ensure that a StatsManager has been created so that stats will be sync with the service as they are modified.
            var statsManager = StatsManagerComponent.Instance;
        }

        void Start()
        {
            this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            StatsManagerComponent.Instance.LocalUserAdded += HandleGetStatHelper;
            if (this.xboxLiveUser != null && this.xboxLiveUser.IsSignedIn)
            {
                this.HandleGetStat(this.xboxLiveUser, this.ID);
            }

        }

        protected void HandleGetStatHelper(object sender, XboxLiveUserEventArgs args)
        {
            if (this.xboxLiveUser == null)
            {
                this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            }
            if (this.xboxLiveUser != null && this.xboxLiveUser.IsSignedIn)
            {
                if (args.User.Gamertag == this.xboxLiveUser.Gamertag)
                {
                    this.HandleGetStat(args.User, this.ID);
                }
            }
        }

        private void OnDestroy()
        {
            if (StatsManagerComponent.Instance != null)
            {
                StatsManagerComponent.Instance.LocalUserAdded -= HandleGetStatHelper;
            }
        }
        protected abstract void HandleGetStat(XboxLiveUser user, string statName);
    }

    /// <summary>
    /// A generic base class for all stats.  Wraps 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class StatBase<T> : StatBase
    {
        private T value;

        public virtual T Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public override string ToString()
        {
            return this.Value != null ? this.Value.ToString() : string.Empty;
        }
    }
}