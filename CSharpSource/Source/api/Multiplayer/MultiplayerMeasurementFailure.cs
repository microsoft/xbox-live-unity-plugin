// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayerMeasurementFailure : int
    {
        Unknown = 0,
        None = 1,
        Timeout = 2,
        Latency = 3,
        BandwidthUp = 4,
        BandwidthDown = 5,
        Group = 6,
        Network = 7,
        Episode = 8,
    }

}
