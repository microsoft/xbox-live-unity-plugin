// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Presence
{
    public class PresenceBroadcastRecord
    {

        public DateTimeOffset StartTime
        {
            get;
            private set;
        }

        public uint ViewerCount
        {
            get;
            private set;
        }

        public string Provider
        {
            get;
            private set;
        }

        public string Session
        {
            get;
            private set;
        }

        public string BroadcastId
        {
            get;
            private set;
        }

    }
}
