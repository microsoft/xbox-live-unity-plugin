// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Xbox.Services.Social.Models
{
    public class ProfileSettingsResponse
    {
        public List<ProfileUser> profileUsers { get; set; }
    }

    public class ProfileUser
    {
        public string id { get; set; }
        public string hostId { get; set; }
        public List<ProfileUserSetting> settings { get; set; }
        public bool isSponsoredUser { get; set; }

        public string Gamertag()
        {
            return GetNamedSetting("gamertag");
        }

        public string Gamerscore()
        {
            return GetNamedSetting("gamerscore");
        }

        public string GameDisplayPic()
        {
            return GetNamedSetting("GameDisplayPicRaw");
        }

        public string GameDisplayName()
        {
            return GetNamedSetting("GameDisplayName");
        }

        public string AppDisplayPic()
        {
            return GetNamedSetting("AppDisplayPicRaw");
        }

        public string AppDisplayName()
        {
            return GetNamedSetting("AppDisplayName");
        }

        private string GetNamedSetting(string name)
        {
            foreach (ProfileUserSetting setting in settings)
            {
                if (setting.id.Trim().ToLower() == name.Trim().ToLower())
                {
                    return setting.value;
                }
            }

            return string.Empty;
        }
    }

    public class ProfileUserSetting
    {
        public string id { get; set; }
        public string value { get; set; }
    }
}
