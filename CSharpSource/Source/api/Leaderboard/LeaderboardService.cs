// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Leaderboard
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Text;
    using global::System.Threading.Tasks;

    public class LeaderboardService : ILeaderboardService
    {
        private const string leaderboardApiContract = "4";

        private static readonly Uri leaderboardsBaseUri = new Uri("https://leaderboards.xboxlive.com");

        private readonly XboxLiveAppConfiguration appConfig;

        internal LeaderboardService()
        {
            this.appConfig = XboxLive.Instance.AppConfig;
        }

        /// <inheritdoc />
        public Task<LeaderboardResult> GetLeaderboardAsync(XboxLiveUser user, LeaderboardQuery query)
        {
            string skipToXboxUserId = null;
            if (query.SkipResultToMe)
            {
                skipToXboxUserId = user.XboxUserId;
            }

            string requestPath = "";
            //if (string.IsNullOrEmpty(query.SocialGroup))
            //{
            //    requestPath = CreateLeaderboardUrlPath(this.appConfig.PrimaryServiceConfigId, query.StatName, query.MaxItems, skipToXboxUserId, query.SkipResultToRank, query.ContinuationToken);
            //}
            //else
            //{
            //    requestPath = CreateSocialLeaderboardUrlPath(this.appConfig.PrimaryServiceConfigId, query.StatName, user.XboxUserId, query.MaxItems, skipToXboxUserId, query.SkipResultToRank, query.ContinuationToken, query.SocialGroup);
            //}

            XboxLiveHttpRequest request = XboxLiveHttpRequest.Create(HttpMethod.Get, leaderboardsBaseUri.ToString(), requestPath);
            request.ContractVersion = leaderboardApiContract;
            request.XboxLiveAPI = XboxLiveAPIName.GetLeaderboardInternal;
            return request.GetResponseWithAuth(user)
                .ContinueWith(
                    responseTask =>
                    {
                        return this.HandleLeaderboardResponse(responseTask, query);
                    });
        }

        internal LeaderboardResult HandleLeaderboardResponse(Task<XboxLiveHttpResponse> responseTask, LeaderboardQuery query)
        {
            XboxLiveHttpResponse response = responseTask.Result;

            if (response.HttpStatus != 200)
            {
                throw new XboxException("Leaderboard request failed with " + response.HttpStatus);
            }

            LeaderboardResponse lbResponse = JsonSerialization.FromJson<LeaderboardResponse>(response.ResponseBodyString);

            IList<LeaderboardColumn> columns = new List<LeaderboardColumn> { lbResponse.LeaderboardInfo.Column };

            IList<LeaderboardRow> rows = new List<LeaderboardRow>();
            foreach (LeaderboardRowResponse row in lbResponse.Rows)
            {
                LeaderboardRow newRow = new LeaderboardRow
                {
                    Gamertag = row.Gamertag,
                    Percentile = row.Percentile,
                    Rank = (uint)row.Rank,
                    XboxUserId = row.XboxUserId,
                    Values = row.Value != null ? new List<string> { row.Value } : row.Values,
                };
                rows.Add(newRow);
            }

            LeaderboardQuery nextQuery = new LeaderboardQuery(query, lbResponse.PagingInfo != null ? lbResponse.PagingInfo.ContinuationToken : null);
            LeaderboardResult result = new LeaderboardResult(rows, columns, lbResponse.LeaderboardInfo.TotalCount);
            return result;
        }

        private static string CreateLeaderboardUrlPath(string serviceConfigurationId, string statName, uint maxItems, string skipToXboxUserId, uint skipToRank, string continuationToken)
        {
            StringBuilder requestPath = new StringBuilder();
            requestPath.AppendFormat("scids/{0}/leaderboards/stat({1})?", serviceConfigurationId, statName);
            AppendQueryParameters(requestPath, maxItems, skipToXboxUserId, skipToRank, continuationToken);

            return requestPath.ToString();
        }

        private static string CreateSocialLeaderboardUrlPath(string serviceConfigurationId, string statName, string xuid, uint maxItems, string skipToXboxUserId, uint skipToRank, string continuationToken, string socialGroup)
        {
            StringBuilder requestPath = new StringBuilder();
            requestPath.AppendFormat("users/xuid({0})/scids/{1}/stats/{2}/people/{3}?", xuid, serviceConfigurationId, statName, socialGroup);
            AppendQueryParameters(requestPath, maxItems, skipToXboxUserId, skipToRank, continuationToken);

            return requestPath.ToString();
        }

        private static void AppendQueryParameters(StringBuilder queryString, uint maxItems, string skipToXboxUserId, uint skipToRank, string continuationToken)
        {
            if (maxItems > 0)
            {
                AppendQueryParameter(queryString, "maxItems", maxItems);
            }

            if (!string.IsNullOrEmpty(skipToXboxUserId) && skipToRank > 0)
            {
                throw new ArgumentException("Cannot provide both user and rank to skip to.");
            }

            if (continuationToken != null)
            {
                AppendQueryParameter(queryString, "continuationToken", continuationToken);
            }
            else if (!string.IsNullOrEmpty(skipToXboxUserId))
            {
                AppendQueryParameter(queryString, "skipToUser", skipToXboxUserId);
            }
            else if (skipToRank > 0)
            {
                AppendQueryParameter(queryString, "skipToRank", skipToRank);
            }

            // Remove the trailing query string bit
            queryString.Remove(queryString.Length - 1, 1);
        }

        private static void AppendQueryParameter(StringBuilder builder, string parameterName, object parameterValue)
        {
            builder.Append(parameterName);
            builder.Append("=");
            builder.Append(parameterValue);
            builder.Append("&");
        }
    }
}