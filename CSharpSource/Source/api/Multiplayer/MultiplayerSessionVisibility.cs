// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayerSessionVisibility : int
    {
        Unknown = 0,
        Any = 1,
        Private = 2,
        Visible = 3,
        Full = 4,
        Open = 5,
    }

}
