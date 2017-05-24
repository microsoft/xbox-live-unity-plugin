// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Multiplayer
{
    using global::System;
    using global::System.Threading.Tasks;

    public class MultiplayerService
    {
        public bool MultiplayerSubscriptionsEnabled { get; private set; }

        //public event EventHandler<MultiplayerSubscriptionLostEventArgs> MultiplayerSubscriptionLost;

        //public event EventHandler<MultiplayerSessionChangeEventArgs> MultiplayerSessionChanged;

        public Task<MultiplayerSession> WriteSessionAsync(MultiplayerSession multiplayerSession, MultiplayerSessionWriteMode multiplayerSessionWriteMode)
        {
            throw new NotImplementedException();
        }

        public Task<WriteSessionResult> TryWriteSessionAsync(MultiplayerSession multiplayerSession, MultiplayerSessionWriteMode multiplayerSessionWriteMode)
        {
            throw new NotImplementedException();
        }

        public Task<MultiplayerSession> WriteSessionByHandleAsync(MultiplayerSession multiplayerSession, MultiplayerSessionWriteMode multiplayerSessionWriteMode, string handleId)
        {
            throw new NotImplementedException();
        }

        public Task<WriteSessionResult> TryWriteSessionByHandleAsync(MultiplayerSession multiplayerSession, MultiplayerSessionWriteMode multiplayerSessionWriteMode, string handleId)
        {
            throw new NotImplementedException();
        }

        public Task<MultiplayerSession> GetCurrentSessionAsync(MultiplayerSessionReference sessionReference)
        {
            throw new NotImplementedException();
        }

        public Task<MultiplayerSession> GetCurrentSessionByHandleAsync(string handleId)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerSessionStates>> GetSessionsAsync(string serviceConfigurationId, string sessionTemplateNameFilter, string xboxUserIdFilter, string keywordFilter, MultiplayerSessionVisibility visibilityFilter, uint contractVersionFilter, bool includePrivateSessions, bool includeReservations, bool includeInactiveSessions, uint maxItems)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerSessionStates>> GetSessionsAsync(MultiplayerGetSessionsRequest getSessionsRequest)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerSessionStates>> GetSessionsForUsersFilterAsync(string serviceConfigurationId, string sessionTemplateNameFilter, string[] xboxUserIdsFilter, string keywordFilter, MultiplayerSessionVisibility visibilityFilter, uint contractVersionFilter, bool includePrivateSessions, bool includeReservations, bool includeInactiveSessions, uint maxItems)
        {
            throw new NotImplementedException();
        }

        public Task SetActivityAsync(MultiplayerSessionReference sessionReference)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetTransferHandleAsync(MultiplayerSessionReference targetSessionReference, MultiplayerSessionReference originSessionReference)
        {
            throw new NotImplementedException();
        }

        public Task SetSearchHandleAsync(MultiplayerSearchHandleRequest searchHandleRequest)
        {
            throw new NotImplementedException();
        }

        public Task ClearActivityAsync(string serviceConfigurationId)
        {
            throw new NotImplementedException();
        }

        public Task ClearSearchHandleAsync(string handleId)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<string>> SendInvitesAsync(MultiplayerSessionReference sessionReference, string[] xboxUserIds, uint titleId, string contextStringId, string activationContext)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<string>> SendInvitesAsync(MultiplayerSessionReference sessionReference, string[] xboxUserIds, uint titleId)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerActivityDetails>> GetActivitiesForSocialGroupAsync(string serviceConfigurationId, string socialGroupOwnerXboxUserId, string socialGroup)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerActivityDetails>> GetActivitiesForUsersAsync(string serviceConfigurationId, string[] xboxUserIds)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerSearchHandleDetails>> GetSearchHandlesAsync(MultiplayerQuerySearchHandleRequest searchHandleRequest)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<MultiplayerSearchHandleDetails>> GetSearchHandlesAsync(string serviceConfigurationId, string sessionTemplateName, string orderBy, bool orderAscending, string searchFilter)
        {
            throw new NotImplementedException();
        }

        public void EnableMultiplayerSubscriptions()
        {
            throw new NotImplementedException();
        }

        public void DisableMultiplayerSubscriptions()
        {
            throw new NotImplementedException();
        }
    }
}