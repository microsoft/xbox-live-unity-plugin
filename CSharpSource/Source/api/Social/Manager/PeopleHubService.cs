// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Text;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class PeopleHubService
    {
        private readonly XboxLiveAppConfiguration appConfig;
        private readonly string peopleHubEndpoint;

        internal PeopleHubService()
        {
            this.appConfig = XboxLive.Instance.AppConfig;
            this.peopleHubEndpoint = appConfig.GetEndpointForService("peoplehub");
        }

        /// <summary>
        /// Get profile information for a user.
        /// </summary>
        /// <param name="user">The user to get profile information for.</param>
        /// <param name="decorations">The additional detail to include in the response</param>
        /// <returns>Social profile information for the user.</returns>
        public Task<XboxSocialUser> GetProfileInfo(XboxLiveUser user, SocialManagerExtraDetailLevel decorations)
        {
            string path = "/users/me/people/xuids(" + user.XboxUserId + ")";

            path += "/decoration/";
            if ((decorations | SocialManagerExtraDetailLevel.None) != SocialManagerExtraDetailLevel.None)
            {
                if (decorations.HasFlag(SocialManagerExtraDetailLevel.TitleHistory))
                {
                    path += "titlehistory(" + this.appConfig.TitleId + "),";
                }

                if (decorations.HasFlag(SocialManagerExtraDetailLevel.PreferredColor))
                {
                    path += "preferredcolor,";
                }
            }
            // We always ask for presence detail. 
            path += "presenceDetail";

            XboxLiveHttpRequest request = XboxLiveHttpRequest.Create(
                HttpMethod.Get,
                this.peopleHubEndpoint,
                path);

            request.ContractVersion = "1";
            request.XboxLiveAPI = XboxLiveAPIName.GetProfileInfo;
            request.CallerContext = "SocialManager";

            return request.GetResponseWithAuth(user)
                .ContinueWith(responseTask =>
                {
                    if (responseTask.IsFaulted)
                    {
                        throw new XboxException("PeopleHub call failed with " + responseTask.Exception);
                    }

                    var response = responseTask.Result;

                    if (response.HttpStatus != 200)
                    {
                        throw new XboxException("PeopleHub call failed with " + response.HttpStatus);
                    }

                    JObject responseBody = JObject.Parse(response.ResponseBodyString);
                    List<XboxSocialUser> users = responseBody["people"].ToObject<List<XboxSocialUser>>();
                    return users[0];
                });
        }

        /// <summary>
        /// Gets the social graph details for a user.
        /// </summary>
        /// <param name="user">The user to request the social graph for.</param>
        /// <param name="decorations">The additional detail to include in the response</param>
        /// <returns>A list of all the users in the given users social graph.</returns>
        public Task<List<XboxSocialUser>> GetSocialGraph(XboxLiveUser user, SocialManagerExtraDetailLevel decorations)
        {
            return this.GetSocialGraph(user, decorations, "social", null);
        }

        /// <summary>
        /// Gets the social graph details for a subset of the users in the given users social graph.
        /// </summary>
        /// <param name="user">The user to request the social graph for.</param>
        /// <param name="decorations">The additional detail to include in the response</param>
        /// <param name="xboxLiveUsers">The users to get social graph details for.</param>
        /// <returns>A list of the social information for the reuested set of users in the given users social graph.</returns>
        public Task<List<XboxSocialUser>> GetSocialGraph(XboxLiveUser user, SocialManagerExtraDetailLevel decorations, IList<string> xboxLiveUsers)
        {
            return this.GetSocialGraph(user, decorations, string.Empty, xboxLiveUsers);
        }

        private Task<List<XboxSocialUser>> GetSocialGraph(XboxLiveUser user, SocialManagerExtraDetailLevel decorations, string relationshipType, IList<string> xboxLiveUsers)
        {
            bool isBatch = xboxLiveUsers != null && xboxLiveUsers.Count > 0;
            string pathAndQuery = this.CreateSocialGraphSubpath(user.XboxUserId, decorations, relationshipType, isBatch);
            XboxLiveHttpRequest request = XboxLiveHttpRequest.Create(
                isBatch ? HttpMethod.Post : HttpMethod.Get,
                this.peopleHubEndpoint,
                pathAndQuery);

            request.ContractVersion = "1";

            if (isBatch)
            {
                JObject postBody = new JObject(new JProperty("xuids", xboxLiveUsers));
                request.RequestBody = postBody.ToString(Formatting.None);
            }

            request.XboxLiveAPI = XboxLiveAPIName.GetSocialGraph;
            request.CallerContext = "SocialManager";

            return request.GetResponseWithAuth(user)
                .ContinueWith(responseTask =>
                {
                    var response = responseTask.Result;
                    JObject responseBody = JObject.Parse(response.ResponseBodyString);
                    List<XboxSocialUser> users = responseBody["people"].ToObject<List<XboxSocialUser>>();
                    return users;
                });
        }

        private string CreateSocialGraphSubpath(string xboxUserId, SocialManagerExtraDetailLevel decorations, string relationshipType, bool isBatch)
        {
            StringBuilder path = new StringBuilder();
            path.Append("/users/xuid(");
            path.Append(xboxUserId);
            path.Append(")/people");
            if (!string.IsNullOrEmpty(relationshipType))
            {
                path.Append("/");
                path.Append(relationshipType);
            }

            if (isBatch)
            {
                path.Append("/batch");
            }

            if ((decorations | SocialManagerExtraDetailLevel.None) != SocialManagerExtraDetailLevel.None)
            {
                path.Append("/decoration/");

                if (decorations.HasFlag(SocialManagerExtraDetailLevel.TitleHistory))
                {
                    path.Append("titlehistory(" + this.appConfig.TitleId + "),");
                }

                if (decorations.HasFlag(SocialManagerExtraDetailLevel.PreferredColor))
                {
                    path.Append("preferredcolor,");
                }

                path.Append("presenceDetail");
            }

            return path.ToString();
        }
    }
}