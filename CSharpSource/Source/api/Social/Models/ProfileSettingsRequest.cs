// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xbox.Services.Social.Models
{
    internal class ProfileSettingsRequest
    {
        public ProfileSettingsRequest(IEnumerable<string> xuidList, bool useDefaultSettings)
        {
            if (xuidList == null)
            {
                throw new ArgumentNullException("xuidList");
            }

            userIds = xuidList;

            InitializeSettings(useDefaultSettings);
        }

        public List<string> settings { get; set; }

        public IEnumerable<string> userIds { get; set; }

        private void InitializeSettings(bool useDefaultSettings)
        {
            settings = new List<string>();

            if (useDefaultSettings)
            {
                settings.Add("AppDisplayName");
                settings.Add("AppDisplayPicRaw");
                settings.Add("GameDisplayName");
                settings.Add("GameDisplayPicRaw");
                settings.Add("Gamerscore");
                settings.Add("Gamertag");
            }
        }
    }
}
