// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Presence
{
    public enum PresenceDetailLevel : int
    {
        Default = 0,
        User = 1,
        Device = 2,
        Title = 3,
        All = 4,
    }

}
