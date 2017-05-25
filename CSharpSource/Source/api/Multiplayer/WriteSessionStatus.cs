// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum WriteSessionStatus : int
    {
        Unknown = 0,
        AccessDenied = 1,
        Created = 2,
        Conflict = 3,
        HandleNotFound = 4,
        OutOfSync = 5,
        SessionDeleted = 6,
        Updated = 7,
    }

}
