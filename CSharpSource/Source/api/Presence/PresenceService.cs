// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Presence
{
    using global::System;
    using global::System.Collections.ObjectModel;
    using global::System.Threading.Tasks;

    public class PresenceService
    {
        public event EventHandler<TitlePresenceChangeEventArgs> TitlePresenceChanged;

        public event EventHandler<DevicePresenceChangeEventArgs> DevicePresenceChanged;

        public virtual Task SetPresenceAsync(bool isUserActiveInTitle, PresenceData presenceData)
        {
            throw new NotImplementedException();
        }

        public Task SetPresenceAsync(bool isUserActiveInTitle)
        {
            return this.SetPresenceAsync(isUserActiveInTitle, null);
        }

        public virtual Task<PresenceRecord> GetPresenceAsync(string xboxUserId)
        {
            throw new NotImplementedException();
        }

        public Task<ReadOnlyCollection<PresenceRecord>> GetPresenceForMultipleUsersAsync(string[] xboxUserIds, PresenceDeviceType[] deviceTypes, uint[] titleIds, PresenceDetailLevel detailLevel, bool onlineOnly, bool broadcastingOnly)
        {
            throw new NotImplementedException();
        }

        public Task<ReadOnlyCollection<PresenceRecord>> GetPresenceForMultipleUsersAsync(string[] xboxUserIds)
        {
            return this.GetPresenceForMultipleUsersAsync(xboxUserIds, null, null, PresenceDetailLevel.All, true, false);
        }

        public Task<ReadOnlyCollection<PresenceRecord>> GetPresenceForSocialGroupAsync(string socialGroup, string socialGroupOwnerXboxuserId, PresenceDeviceType[] deviceTypes, uint[] titleIds, PresenceDetailLevel detailLevel, bool onlineOnly, bool broadcastingOnly)
        {
            throw new NotImplementedException();
        }

        public Task<ReadOnlyCollection<PresenceRecord>> GetPresenceForSocialGroupAsync(string socialGroup)
        {
            return this.GetPresenceForSocialGroupAsync(socialGroup, null, null, null, PresenceDetailLevel.All, true, false);
        }

        public DevicePresenceChangeSubscription SubscribeToDevicePresenceChange(string xboxUserId)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromDevicePresenceChange(DevicePresenceChangeSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public TitlePresenceChangeSubscription SubscribeToTitlePresenceChange(string xboxUserId, uint titleId)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromTitlePresenceChange(TitlePresenceChangeSubscription subscription)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnTitlePresenceChanged(TitlePresenceChangeEventArgs e)
        {
            var onTitlePresenceChanged = this.TitlePresenceChanged;
            if (onTitlePresenceChanged != null)
            {
                onTitlePresenceChanged(this, e);
            }
        }

        protected virtual void OnDevicePresenceChanged(DevicePresenceChangeEventArgs e)
        {
            var onDevicePresenceChanged = this.DevicePresenceChanged;
            if (onDevicePresenceChanged != null)
            {
                onDevicePresenceChanged(this, e);
            }
        }
    }
}