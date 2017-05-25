// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.ContextualSearch
{
    public enum ContextualSearchGameClipUriType : int
    {
        None = 0,
        Original = 1,
        Download = 2,
        SmoothStreaming = 3,
        HttpLiveStreaming = 4,
    }

}
