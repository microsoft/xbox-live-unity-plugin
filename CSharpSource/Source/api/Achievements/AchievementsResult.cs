// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Achievements
{
    public class AchievementsResult
    {

        public bool HasNext
        {
            get;
            private set;
        }

        public IList<Achievement> Items
        {
            get;
            private set;
        }


        public Task<AchievementsResult> GetNextAsync(uint maxItems)
        {
            throw new NotImplementedException();
        }

    }
}
