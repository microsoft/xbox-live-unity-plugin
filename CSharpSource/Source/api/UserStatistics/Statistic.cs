// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.UserStatistics
{
    public class Statistic
    {

        public string Value
        {
            get;
            private set;
        }

        public Type StatisticType
        {
            get;
            private set;
        }

        public string StatisticName
        {
            get;
            private set;
        }

    }
}
