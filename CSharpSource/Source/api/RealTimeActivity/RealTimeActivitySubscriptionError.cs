// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.RealTimeActivity
{
    public enum RealTimeActivitySubscriptionError : int
    {
        NoError = 0,
        JsonError = 1008,
        RTAGenericError = 1500,
        RTASubscriptionLimit = 1501,
        RTAAccessDenied = 1502,
    }

}
