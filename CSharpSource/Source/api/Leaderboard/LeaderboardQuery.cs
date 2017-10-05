// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    public partial class LeaderboardQuery : ILeaderboardQuery
    {
        public string StatName { get; private set; }

        public string SocialGroup { get; private set; }
                
        public bool HasNext { get; private set; }
    }
}