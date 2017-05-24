// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.ContextualSearch
{
    public class ContextualSearchBroadcast
    {

        public Dictionary<string, string> CurrentStats
        {
            get;
            private set;
        }

        public DateTimeOffset StartedDate
        {
            get;
            private set;
        }

        public ulong Viewers
        {
            get;
            private set;
        }

        public string BroadcasterIdFromProvider
        {
            get;
            private set;
        }

        public string Provider
        {
            get;
            private set;
        }

        public string XboxUserId
        {
            get;
            private set;
        }

    }
}
