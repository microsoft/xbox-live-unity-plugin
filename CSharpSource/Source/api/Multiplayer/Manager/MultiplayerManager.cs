// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public class MultiplayerManager
    {

        public bool AutoFillMembersDuringMatchmaking
        {
            get;
            set;
        }

        public Joinability Joinability
        {
            get;
            private set;
        }

        public MultiplayerGameSession GameSession
        {
            get;
            private set;
        }

        public MultiplayerLobbySession LobbySession
        {
            get;
            private set;
        }

        public TimeSpan EstimatedMatchWaitTime
        {
            get;
            private set;
        }

        public MatchStatus MatchStatus
        {
            get;
            private set;
        }

        public static MultiplayerManager SingletonInstance
        {
            get;
            private set;
        }


        public void Initialize(string lobbySessionTemplateName)
        {
            throw new NotImplementedException();
        }

        public void FindMatch(string hopperName, string attributes, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void CancelMatch()
        {
            throw new NotImplementedException();
        }

        public void JoinLobby(global::System.Object eventArgs, Microsoft.Xbox.Services.XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public void JoinLobby(string handleId, Microsoft.Xbox.Services.XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public void JoinGameFromLobby(string sessionTemplateName)
        {
            throw new NotImplementedException();
        }

        public void JoinGame(string sessionName, string sessionTemplateName, string[] xboxUserIds)
        {
            throw new NotImplementedException();
        }

        public void JoinGame(string sessionName, string sessionTemplateName)
        {
            throw new NotImplementedException();
        }

        public void LeaveGame()
        {
            throw new NotImplementedException();
        }

        public IList<MultiplayerEvent> DoWork()
        {
            throw new NotImplementedException();
        }

        public void SetJoinInProgress(Joinability value)
        {
            throw new NotImplementedException();
        }

        public void SetQualityOfServiceMeasurements(Microsoft.Xbox.Services.Multiplayer.MultiplayerQualityOfServiceMeasurements[] measurements)
        {
            throw new NotImplementedException();
        }

    }
}
