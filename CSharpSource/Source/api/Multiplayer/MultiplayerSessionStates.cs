// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerSessionStates
    {

        public IList<string> Keywords
        {
            get;
            private set;
        }

        public MultiplayerSessionRestriction JoinRestriction
        {
            get;
            private set;
        }

        public uint AcceptedMemberCount
        {
            get;
            private set;
        }

        public string XboxUserId
        {
            get;
            private set;
        }

        public bool IsMyTurn
        {
            get;
            private set;
        }

        public MultiplayerSessionVisibility Visibility
        {
            get;
            private set;
        }

        public MultiplayerSessionStatus Status
        {
            get;
            private set;
        }

        public MultiplayerSessionReference SessionReference
        {
            get;
            private set;
        }

        public DateTimeOffset StartTime
        {
            get;
            private set;
        }

    }
}
