// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerSessionProperties
    {

        public bool Closed
        {
            get;
            private set;
        }

        public string HostDeviceToken
        {
            get;
            private set;
        }

        public IList<uint> SessionOwnerIndices
        {
            get;
            private set;
        }

        public IList<string> ServerConnectionStringCandidates
        {
            get;
            private set;
        }

        public string MatchmakingServerConnectionString
        {
            get;
            private set;
        }

        public string SessionCustomPropertiesJson
        {
            get;
            private set;
        }

        public string MatchmakingTargetSessionConstantsJson
        {
            get;
            private set;
        }

        public IList<MultiplayerSessionMember> TurnCollection
        {
            get;
            set;
        }

        public MultiplayerSessionRestriction ReadRestriction
        {
            get;
            set;
        }

        public MultiplayerSessionRestriction JoinRestriction
        {
            get;
            set;
        }

        public IList<string> Keywords
        {
            get;
            set;
        }

    }
}
