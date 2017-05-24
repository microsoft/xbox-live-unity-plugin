// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public enum MultiplayerEventType : int
    {
        UserAdded = 0,
        UserRemoved = 1,
        MemberJoined = 2,
        MemberLeft = 3,
        MemberPropertyChanged = 4,
        LocalMemberPropertyWriteCompleted = 5,
        LocalMemberConnectionAddressWriteCompleted = 6,
        SessionPropertyChanged = 7,
        SessionPropertyWriteCompleted = 8,
        SessionSynchronizedPropertyWriteCompleted = 9,
        HostChanged = 10,
        SynchronizedHostWriteCompleted = 11,
        JoinabilityStateChanged = 12,
        PerformQosMeasurements = 13,
        FindMatchCompleted = 14,
        JoinGameCompleted = 15,
        LeaveGameCompleted = 16,
        JoinLobbyCompleted = 17,
        ClientDisconnectedFromMultiplayerService = 18,
        InviteSent = 19,
        TournamentRegistrationStateChanged = 20,
        TournamentGameSessionReady = 21,
        ArbitrationComplete = 22,
    }

}
