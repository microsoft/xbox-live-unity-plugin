// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayerSessionChangeTypes : int
    {
        None = 0,
        Everything = 1,
        HostDeviceTokenChange = 2,
        InitializationStateChange = 4,
        MatchmakingStatusChange = 8,
        MemberListChange = 16,
        MemberStatusChange = 32,
        SessionJoinabilityChange = 64,
        CustomPropertyChange = 128,
        MemberCustomPropertyChange = 256,
        TournamentPropertyChange = 512,
        ArbitrationPropertyChange = 1024,
    }

}
