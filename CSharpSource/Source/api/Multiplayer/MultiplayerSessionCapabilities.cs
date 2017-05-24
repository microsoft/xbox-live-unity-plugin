// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerSessionCapabilities
    {
        public MultiplayerSessionCapabilities() {
        }

        public bool HasOwners
        {
            get;
            set;
        }

        public bool Searchable
        {
            get;
            set;
        }

        public bool Arbitration
        {
            get;
            set;
        }

        public bool Team
        {
            get;
            set;
        }

        public bool Crossplay
        {
            get;
            set;
        }

        public bool UserAuthorizationStyle
        {
            get;
            set;
        }

        public bool ConnectionRequiredForActiveMembers
        {
            get;
            set;
        }

        public bool Large
        {
            get;
            set;
        }

        public bool Gameplay
        {
            get;
            set;
        }

        public bool SuppressPresenceActivityCheck
        {
            get;
            set;
        }

        public bool Connectivity
        {
            get;
            set;
        }

    }
}
