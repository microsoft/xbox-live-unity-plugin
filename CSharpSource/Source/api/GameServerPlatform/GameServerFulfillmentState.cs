// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.GameServerPlatform
{
    public enum GameServerFulfillmentState : int
    {
        Unknown = 0,
        Fulfilled = 1,
        Queued = 2,
        Aborted = 3,
    }

}
