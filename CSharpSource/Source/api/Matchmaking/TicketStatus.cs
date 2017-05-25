// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Matchmaking
{
    public enum TicketStatus : int
    {
        Unknown = 0,
        Expired = 1,
        Searching = 2,
        Found = 3,
        Canceled = 4,
    }

}
