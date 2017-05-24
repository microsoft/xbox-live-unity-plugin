// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.System
{
    public class VerifyStringResult
    {

        public string FirstOffendingSubstring
        {
            get;
            private set;
        }

        public VerifyStringResultCode ResultCode
        {
            get;
            private set;
        }

    }
}
