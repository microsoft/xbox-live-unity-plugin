// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.ContextualSearch
{
    public class ContextualSearchGameClipStat
    {

        public string DeltaValue
        {
            get;
            private set;
        }

        public string MaxValue
        {
            get;
            private set;
        }

        public string MinValue
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

    }
}
