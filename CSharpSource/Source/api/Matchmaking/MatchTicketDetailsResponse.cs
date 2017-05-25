// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Matchmaking
{
    public class MatchTicketDetailsResponse
    {

        public string TicketAttributes
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference TargetSession
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference TicketSession
        {
            get;
            private set;
        }

        public PreserveSessionMode PreserveSession
        {
            get;
            private set;
        }

        public TimeSpan EstimatedWaitTime
        {
            get;
            private set;
        }

        public TicketStatus MatchStatus
        {
            get;
            private set;
        }

    }
}
