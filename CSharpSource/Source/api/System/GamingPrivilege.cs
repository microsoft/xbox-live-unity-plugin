// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.System
{
    public enum GamingPrivilege : int
    {
        Broadcast = 190,
        ViewFriendsList = 197,
        GameDVR = 198,
        ShareKinectContent = 199,
        MultiplayerParties = 203,
        CommunicationVoiceIngame = 205,
        CommunicationVoiceSkype = 206,
        CloudGamingManageSession = 207,
        CloudGamingJoinSession = 208,
        CloudSavedGames = 209,
        ShareContent = 211,
        PremiumContent = 214,
        SubscriptionContent = 219,
        SocialNetworkSharing = 220,
        PremiumVideo = 224,
        VideoCommunications = 235,
        PurchaseContent = 245,
        UserCreatedContent = 247,
        ProfileViewing = 249,
        Communications = 252,
        MultiplayerSessions = 254,
        AddFriend = 255,
    }

}
