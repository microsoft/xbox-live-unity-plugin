// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.GameServerPlatform
{
    public class GameServerTicketStatus
    {

        public string Region
        {
            get;
            private set;
        }

        public string GameHostId
        {
            get;
            private set;
        }

        public IList<GameServerPortMapping> PortMappings
        {
            get;
            private set;
        }

        public string SecureContext
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public GameServerHostStatus Status
        {
            get;
            private set;
        }

        public string HostName
        {
            get;
            private set;
        }

        public uint TitleId
        {
            get;
            private set;
        }

        public string ClusterId
        {
            get;
            private set;
        }

        public string TicketId
        {
            get;
            private set;
        }

    }
}
