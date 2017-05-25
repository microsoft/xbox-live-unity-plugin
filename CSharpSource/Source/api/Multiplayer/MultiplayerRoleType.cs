// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerRoleType
    {
        public MultiplayerRoleType() {
        }

        public Dictionary<string, MultiplayerRoleInfo> Roles
        {
            get;
            set;
        }

        public IList<MutableRoleSetting> MutableRoleSettings
        {
            get;
            private set;
        }

        public bool OwnerManaged
        {
            get;
            private set;
        }

    }
}
