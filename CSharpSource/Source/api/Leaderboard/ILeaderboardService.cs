// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System.Threading.Tasks;

    internal interface ILeaderboardService
    {
        /// <summary>
        /// Get a leaderboard for a user.
        /// </summary>
        /// <param name="user">The specific user to fetch the leaderboard for..</param>
        /// <param name="query">Additional leaderboard query information</param>
        /// <returns>A <see cref="LeaderboardResult"/> object containing the leaderboard data.</returns>
        Task<LeaderboardResult> GetLeaderboardAsync(XboxLiveUser user, LeaderboardQuery query);
    }
}