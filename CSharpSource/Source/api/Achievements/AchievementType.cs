// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Achievements
{
    public enum AchievementType : int
    {
        Unknown = 0,
        All = 1,
        Persistent = 2,
        Challenge = 3,
    }

}
