// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    public partial class LeaderboardQuery
    {
        public LeaderboardQuery()
        {
            pImpl = new LeaderboardQueryUnityEditorImpl();
            StatName = "Score";
            HasNext = false;
            SocialGroup = "All";
        }
    }
}
