// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.GameServerPlatform
{
    public class AllocationResult
    {

        public string SecureDeviceAddress
        {
            get;
            private set;
        }

        public IList<GameServerPortMapping> PortMappings
        {
            get;
            private set;
        }

        public string Region
        {
            get;
            private set;
        }

        public string SessionHostId
        {
            get;
            private set;
        }

        public string HostName
        {
            get;
            private set;
        }

        public GameServerFulfillmentState FulfillmentState
        {
            get;
            private set;
        }

    }
}
