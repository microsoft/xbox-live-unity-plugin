// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.ContextualSearch
{
    public class ContextualSearchService
    {

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<ContextualSearchConfiguredStat>> GetConfigurationAsync(uint titleId)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<ContextualSearchBroadcast>> GetBroadcastsAsync(uint titleId, uint skipItems, uint maxItems, string orderByStatName, bool orderAscending, string filterStatName, ContextualSearchFilterOperator filterOperator, string filterStatValue)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<ContextualSearchBroadcast>> GetBroadcastsAsync(uint titleId, uint skipItems, uint maxItems, string orderByStatName, bool orderAscending, string searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<ContextualSearchBroadcast>> GetBroadcastsAsync(uint titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ContextualSearchGameClipsResult> GetGameClipsAsync(uint titleId, uint skipItems, uint maxItems, string orderByStatName, bool orderAscending, string searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task<ContextualSearchGameClipsResult> GetGameClipsAsync(uint titleId, uint skipItems, uint maxItems, string orderByStatName, bool orderAscending, string filterStatName, ContextualSearchFilterOperator filterOperator, string filterStatValue)
        {
            throw new NotImplementedException();
        }

    }
}
