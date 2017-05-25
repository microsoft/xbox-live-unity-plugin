// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public class MultiplayerMember
    {

        public string _DeviceToken
        {
            get;
            private set;
        }

        public string Properties
        {
            get;
            private set;
        }

        public string ConnectionAddress
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionMemberStatus Status
        {
            get;
            private set;
        }

        public bool IsInGame
        {
            get;
            private set;
        }

        public bool IsInLobby
        {
            get;
            private set;
        }

        public bool IsLocal
        {
            get;
            private set;
        }

        public string DebugGamertag
        {
            get;
            private set;
        }

        public string XboxUserId
        {
            get;
            private set;
        }

        public string TeamId
        {
            get;
            private set;
        }

        public uint MemberId
        {
            get;
            private set;
        }


        public bool IsMemberOnSameDevice(MultiplayerMember member)
        {
            throw new NotImplementedException();
        }

    }
}
