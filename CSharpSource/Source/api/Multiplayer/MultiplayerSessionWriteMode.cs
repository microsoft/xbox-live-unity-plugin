// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayerSessionWriteMode : int
    {
        CreateNew = 0,
        UpdateOrCreateNew = 1,
        UpdateExisting = 2,
        SynchronizedUpdate = 3,
    }

}
