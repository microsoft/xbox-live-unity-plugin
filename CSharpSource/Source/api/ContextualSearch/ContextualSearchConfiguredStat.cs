// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.ContextualSearch
{
    public class ContextualSearchConfiguredStat
    {

        public ulong RangeMax
        {
            get;
            private set;
        }

        public ulong RangeMin
        {
            get;
            private set;
        }

        public Dictionary<string, string> ValueToDisplayName
        {
            get;
            private set;
        }

        public ContextualSearchStatVisibility DisplayType
        {
            get;
            private set;
        }

        public bool CanBeSorted
        {
            get;
            private set;
        }

        public bool CanBeFiltered
        {
            get;
            private set;
        }

        public string DisplayName
        {
            get;
            private set;
        }

        public ContextualSearchStatVisibility Visibility
        {
            get;
            private set;
        }

        public string DataType
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
