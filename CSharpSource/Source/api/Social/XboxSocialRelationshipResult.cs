// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Social
{
    public class XboxSocialRelationshipResult
    {

        public bool HasNext
        {
            get;
            private set;
        }

        public uint TotalCount
        {
            get;
            private set;
        }

        public IList<XboxSocialRelationship> Items
        {
            get;
            private set;
        }


        public Task<XboxSocialRelationshipResult> GetNextAsync(uint maxItems)
        {
            throw new NotImplementedException();
        }

    }
}
