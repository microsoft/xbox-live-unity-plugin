// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.RealTimeActivity
{
    public enum RealTimeActivitySubscriptionState : int
    {
        Unknown = 0,
        PendingSubscribe = 1,
        Subscribed = 2,
        PendingUnsubscribe = 3,
        Closed = 4,
    }

}
