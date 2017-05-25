// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Achievements
{
    public class AchievementMediaAsset
    {

        public string Url
        {
            get;
            private set;
        }

        public AchievementMediaAssetType MediaAssetType
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

    }
}
