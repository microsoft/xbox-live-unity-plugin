// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.ContextualSearch
{
    public class ContextualSearchGameClipsResult
    {

        public IList<ContextualSearchGameClip> Items
        {
            get;
            private set;
        }


        public Task<ContextualSearchGameClipsResult> GetNextAsync(uint maxItems)
        {
            throw new NotImplementedException();
        }

    }
}
