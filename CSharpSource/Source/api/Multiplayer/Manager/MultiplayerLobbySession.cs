// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public class MultiplayerLobbySession
    {
        public Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionConstants SessionConstants
        {
            get;
            private set;
        }

        public string Properties
        {
            get;
            private set;
        }

        public MultiplayerMember Host
        {
            get;
            private set;
        }

        public IList<MultiplayerMember> Members
        {
            get;
            private set;
        }

        public IList<MultiplayerMember> LocalMembers
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference SessionReference
        {
            get;
            private set;
        }

        public string CorrelationId
        {
            get;
            private set;
        }


        public void AddLocalUser(Microsoft.Xbox.Services.XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public void RemoveLocalUser(Microsoft.Xbox.Services.XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public void SetLocalMemberProperties(Microsoft.Xbox.Services.XboxLiveUser user, string name, string valueJson, IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void DeleteLocalMemberProperties(Microsoft.Xbox.Services.XboxLiveUser user, string name, IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void SetLocalMemberConnectionAddress(Microsoft.Xbox.Services.XboxLiveUser user, string connectionAddress, IntPtr context)
        {
            throw new NotImplementedException();
        }

        public bool IsHost(string xboxUserId)
        {
            throw new NotImplementedException();
        }

        public void SetProperties(string name, string valueJson, IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void SetSynchronizedProperties(string name, string valueJson, IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void SetSynchronizedHost(MultiplayerMember gameHost, IntPtr context)
        {
            throw new NotImplementedException();
        }

        public void InviteFriends(Microsoft.Xbox.Services.XboxLiveUser user, string contextStringId, string customActivationContext)
        {
            throw new NotImplementedException();
        }

        public void InviteUsers(Microsoft.Xbox.Services.XboxLiveUser user, string[] xboxUserIds, string contextStringId, string customActivationContext)
        {
            throw new NotImplementedException();
        }

    }
}
