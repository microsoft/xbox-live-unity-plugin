// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayerSessionMemberStatus : int
    {
        Reserved = 0,
        Inactive = 1,
        Ready = 2,
        Active = 3,
    }

}
