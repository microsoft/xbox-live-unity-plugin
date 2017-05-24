// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.System
{
    public enum VerifyStringResultCode : int
    {
        Success = 0,
        Offensive = 1,
        TooLong = 2,
        UnknownError = 3,
    }

}
