// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayerInitializationStage : int
    {
        Unknown = 0,
        None = 1,
        Joining = 2,
        Measuring = 3,
        Evaluating = 4,
        Failed = 5,
    }

}
