// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Social
{
    public enum ReputationFeedbackType : int
    {
        FairPlayKillsTeammates = 0,
        FairPlayCheater = 1,
        FairPlayTampering = 2,
        FairPlayQuitter = 3,
        FairPlayKicked = 4,
        CommunicationsInappropriateVideo = 5,
        CommunicationsAbusiveVoice = 6,
        InappropriateUserGeneratedContent = 7,
        PositiveSkilledPlayer = 8,
        PositiveHelpfulPlayer = 9,
        PositiveHighQualityUserGeneratedContent = 10,
        CommsPhishing = 11,
        CommsPictureMessage = 12,
        CommsSpam = 13,
        CommsTextMessage = 14,
        CommsVoiceMessage = 15,
        FairPlayConsoleBanRequest = 16,
        FairPlayIdler = 17,
        FairPlayUserBanRequest = 18,
        UserContentGamerpic = 19,
        UserContentPersonalinfo = 20,
        FairPlayUnsporting = 21,
        FairPlayLeaderboardCheater = 22,
    }

}
