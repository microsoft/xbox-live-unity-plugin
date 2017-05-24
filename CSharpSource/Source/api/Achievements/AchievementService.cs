// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Achievements
{
    public class AchievementService
    {

        public Task UpdateAchievementAsync(string xboxUserId, uint titleId, string serviceConfigurationId, string achievementId, uint percentComplete)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAchievementAsync(string xboxUserId, string achievementId, uint percentComplete)
        {
            throw new NotImplementedException();
        }

        public Task<AchievementsResult> GetAchievementsForTitleIdAsync(string xboxUserId, uint titleId, AchievementType type, bool unlockedOnly, AchievementOrderBy orderBy, uint skipItems, uint maxItems)
        {
            throw new NotImplementedException();
        }

        public Task<Achievement> GetAchievementAsync(string xboxUserId, string serviceConfigurationId, string achievementId)
        {
            throw new NotImplementedException();
        }

    }
}
