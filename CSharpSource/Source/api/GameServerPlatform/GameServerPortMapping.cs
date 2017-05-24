// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.GameServerPlatform
{
    public class GameServerPortMapping
    {

        public uint ExternalPortNumber
        {
            get;
            private set;
        }

        public uint InternalPortNumber
        {
            get;
            private set;
        }

        public string PortName
        {
            get;
            private set;
        }

    }
}
