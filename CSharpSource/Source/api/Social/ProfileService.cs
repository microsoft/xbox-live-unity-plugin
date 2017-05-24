// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    public class ProfileService
    {
        private readonly string profileEndpoint;

        protected XboxLiveAppConfiguration config;

        internal ProfileService()
        {
            this.config = XboxLive.Instance.AppConfig;
            this.profileEndpoint = this.config.GetEndpointForService("profile");
        }

        public Task<XboxUserProfile> GetUserProfileAsync(XboxLiveUser user, string xboxUserId)
        {
            if (string.IsNullOrEmpty(xboxUserId))
            {
                throw new ArgumentException("invalid xboxUserId", "xboxUserId");
            }

            List<string> profiles = new List<string> { xboxUserId };

            return this.GetUserProfilesAsync(user, profiles).ContinueWith(task => task.Result[0]);
        }

        public Task<List<XboxUserProfile>> GetUserProfilesAsync(XboxLiveUser user, List<string> xboxUserIds)
        {
            if (xboxUserIds == null)
            {
                throw new ArgumentNullException("xboxUserIds");
            }
            if (xboxUserIds.Count == 0)
            {
                throw new ArgumentOutOfRangeException("xboxUserIds", "Empty list of user ids");
            }

            if (XboxLive.UseMockServices)
            {
                Random rand = new Random();
                List<XboxUserProfile> outputUsers = new List<XboxUserProfile>(xboxUserIds.Count);
                foreach (string xuid in xboxUserIds)
                {
                    // generate a fake dev gamertag
                    string gamertag = "2 dev " + rand.Next(10000);
                    XboxUserProfile profile = new XboxUserProfile()
                    {
                        XboxUserId = xuid,
                        ApplicationDisplayName = gamertag,
                        ApplicationDisplayPictureResizeUri = new Uri("http://images-eds.xboxlive.com/image?url=z951ykn43p4FqWbbFvR2Ec.8vbDhj8G2Xe7JngaTToBrrCmIEEXHC9UNrdJ6P7KI4AAOijCgOA3.jozKovAH98vieJP1ResWJCw2dp82QtambLRqzQbSIiqrCug0AvP4&format=png"),
                        GameDisplayName = gamertag,
                        GameDisplayPictureResizeUri = new Uri("http://images-eds.xboxlive.com/image?url=z951ykn43p4FqWbbFvR2Ec.8vbDhj8G2Xe7JngaTToBrrCmIEEXHC9UNrdJ6P7KI4AAOijCgOA3.jozKovAH98vieJP1ResWJCw2dp82QtambLRqzQbSIiqrCug0AvP4&format=png"),
                        Gamerscore = rand.Next(250000).ToString(),
                        Gamertag = gamertag
                    };

                    outputUsers.Add(profile);
                }

                return Task.FromResult(outputUsers);
            }
            else
            {
                XboxLiveHttpRequest req = XboxLiveHttpRequest.Create(HttpMethod.Post, profileEndpoint, "/users/batch/profile/settings");

                req.ContractVersion = "2";
                req.ContentType = "application/json; charset=utf-8";
                Models.ProfileSettingsRequest reqBodyObject = new Models.ProfileSettingsRequest(xboxUserIds, true);
                req.RequestBody = JsonSerialization.ToJson(reqBodyObject);
                req.XboxLiveAPI = XboxLiveAPIName.GetUserProfiles;
                return req.GetResponseWithAuth(user).ContinueWith(task =>
                {
                    XboxLiveHttpResponse response = task.Result;
                    Models.ProfileSettingsResponse responseBody = new Models.ProfileSettingsResponse();
                    responseBody = JsonSerialization.FromJson<Models.ProfileSettingsResponse>(response.ResponseBodyString);

                    List<XboxUserProfile> outputUsers = new List<XboxUserProfile>();
                    foreach (Models.ProfileUser entry in responseBody.profileUsers)
                    {
                        XboxUserProfile profile = new XboxUserProfile()
                        {
                            XboxUserId = entry.id,
                            Gamertag = entry.Gamertag(),
                            GameDisplayName = entry.GameDisplayName(),
                            GameDisplayPictureResizeUri = new Uri(entry.GameDisplayPic()),
                            ApplicationDisplayName = entry.AppDisplayName(),
                            ApplicationDisplayPictureResizeUri = new Uri(entry.AppDisplayPic()),
                            Gamerscore = entry.Gamerscore()
                        };

                        outputUsers.Add(profile);
                    }

                    return outputUsers;
                });
            }
        }

        public Task<List<XboxUserProfile>> GetUserProfilesForSocialGroupAsync(string socialGroup)
        {
            throw new NotImplementedException();
        }
    }
}