// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public enum Joinability : int
    {
        None = 0,
        JoinableByFriends = 1,
        InviteOnly = 2,
        DisableWhileGameInProgress = 3,
        Closed = 4,
    }

}
