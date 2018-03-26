// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Client
{
    public enum LeaderboardTypes
    {
        Global,
        Friends,
        Favorite
    }

    public class LeaderboardHelper
    {
        public static string GetSocialGroupFromLeaderboardType(LeaderboardTypes leaderboardType)
        {
            switch (leaderboardType)
            {
                case LeaderboardTypes.Global:
                    return "";
                case LeaderboardTypes.Favorite:
                    return "favorite";
                case LeaderboardTypes.Friends:
                    return "all";
            }

            return null;
        }
    }
}