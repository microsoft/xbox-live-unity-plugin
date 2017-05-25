// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Matchmaking
{
    public class MatchmakingService
    {

        public Task<CreateMatchTicketResponse> CreateMatchTicketAsync(Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference ticketSessionReference, string matchmakingServiceConfigurationId, string hopperName, TimeSpan ticketTimeout, PreserveSessionMode preserveSession, string ticketAttributesJson)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMatchTicketAsync(string serviceConfigurationId, string hopperName, string ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<MatchTicketDetailsResponse> GetMatchTicketDetailsAsync(string serviceConfigurationId, string hopperName, string ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<HopperStatisticsResponse> GetHopperStatisticsAsync(string serviceConfigurationId, string hopperName)
        {
            throw new NotImplementedException();
        }

    }
}
