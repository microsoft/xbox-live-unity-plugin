// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.GameServerPlatform
{
    public class GameServerPlatformService
    {

        public Task<ClusterResult> AllocateClusterAsync(uint gameServerTitleId, string serviceConfigurationId, string sessionTemplateName, string sessionName, bool abortIfQueued)
        {
            throw new NotImplementedException();
        }

        public Task<ClusterResult> AllocateClusterInlineAsync(uint gameServerTitleId, string serviceConfigurationId, string sandboxId, string ticketId, string gsiSetId, string gameVariantId, ulong maxAllowedPlayers, string location, bool abortIfQueued)
        {
            throw new NotImplementedException();
        }

        public Task<GameServerTicketStatus> GetTicketStatusAsync(uint gameServerTitleId, string ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<GameServerMetadataResult> GetGameServerMetadataAsync(uint titleId, uint maxAllowedPlayers, bool publisherOnly, uint maxVariants, string locale, Dictionary<string, string> filterTags)
        {
            throw new NotImplementedException();
        }

        public Task<GameServerMetadataResult> GetGameServerMetadataAsync(uint titleId, uint maxAllowedPlayers, bool publisherOnly, uint maxVariants, string locale)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<QualityOfServiceServer>> GetQualityOfServiceServersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AllocationResult> AllocateSessionHost(uint gameServerTitleId, string[] locations, string sessionId, string cloudGameId, string gameModeId, string sessionCookie)
        {
            throw new NotImplementedException();
        }

        public Task<AllocationResult> GetSessionHostAllocationStatus(uint gameServerTitleId, string sessionId)
        {
            throw new NotImplementedException();
        }

    }
}
