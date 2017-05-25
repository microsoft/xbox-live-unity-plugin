// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Social
{
    public enum SocialNotificationType : int
    {
        Unknown = 0,
        added = 1,
        changed = 2,
        removed = 3,
    }

}
