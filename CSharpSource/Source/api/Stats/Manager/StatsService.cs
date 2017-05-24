// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Threading.Tasks;

    using Newtonsoft.Json;

    public class StatsService
    {
        private readonly XboxLiveAppConfiguration config;
        private readonly JsonSerializerSettings serializerSettings;

        private readonly string statsReadEndpoint;
        private readonly string statsWriteEndpoint;

        internal StatsService()
        {
            this.config = XboxLive.Instance.AppConfig;
            this.serializerSettings = new JsonSerializerSettings();

            this.statsReadEndpoint = config.GetEndpointForService("statsread");
            this.statsWriteEndpoint = config.GetEndpointForService("statswrite");
        }

        public Task UpdateStatsValueDocument(XboxLiveUser user, StatsValueDocument statValuePostDocument)
        {
            string pathAndQuery = PathAndQueryStatSubpath(
                user.XboxUserId,
                this.config.PrimaryServiceConfigId,
                false
            );

            XboxLiveHttpRequest req = XboxLiveHttpRequest.Create(HttpMethod.Post, this.statsWriteEndpoint, pathAndQuery);
            var svdModel = new Models.StatsValueDocumentModel()
            {
                Revision = ++statValuePostDocument.Revision,
                Timestamp = DateTime.Now,
                Stats = new Models.Stats()
                {
                    Title = new Dictionary<string, Models.Stat>()
                }
            };

            svdModel.Stats.Title = statValuePostDocument.Stats.ToDictionary(
                stat => stat.Key,
                stat => new Models.Stat()
                {
                    Value = stat.Value.Value
                });

            req.RequestBody = JsonConvert.SerializeObject(svdModel, serializerSettings);
            req.XboxLiveAPI = XboxLiveAPIName.UpdateStatsValueDocument;
            req.CallerContext = "StatsManager";
            req.RetryAllowed = false;
            return req.GetResponseWithAuth(user);
        }

        public Task<StatsValueDocument> GetStatsValueDocument(XboxLiveUser user)
        {
            string pathAndQuery = PathAndQueryStatSubpath(
                user.XboxUserId,
                this.config.PrimaryServiceConfigId,
                false
            );

            XboxLiveHttpRequest req = XboxLiveHttpRequest.Create(HttpMethod.Get, this.statsReadEndpoint, pathAndQuery);
            req.XboxLiveAPI = XboxLiveAPIName.GetStatsValueDocument;
            req.CallerContext = "StatsManager";
            return req.GetResponseWithAuth(user).ContinueWith(task =>
            {
                XboxLiveHttpResponse response = task.Result;
                var svdModel = JsonConvert.DeserializeObject<Models.StatsValueDocumentModel>(response.ResponseBodyString);
                var svd = new StatsValueDocument(svdModel.Stats.Title, svdModel.Revision)
                {
                    State = StatsValueDocument.StatValueDocumentState.Loaded,
                    User = user
                };
                return svd;
            });
        }

        private static string PathAndQueryStatSubpath(string xuid, string scid, bool userXuidTag)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/stats/users/");
            if (userXuidTag)
            {
                sb.AppendFormat("xuid({0})", xuid);
            }
            else
            {
                sb.Append(xuid);
            }

            sb.AppendFormat("/scids/{0}", scid);

            return sb.ToString();
        }
    }
}