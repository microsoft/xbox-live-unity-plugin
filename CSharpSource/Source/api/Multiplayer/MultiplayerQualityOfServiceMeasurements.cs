// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerQualityOfServiceMeasurements
    {
        public MultiplayerQualityOfServiceMeasurements(string memberDeviceToken, TimeSpan latency, ulong bandwidthDownInKilobitsPerSecond, ulong bandwidthUpInKilobitsPerSecond, string customJson) {
            var _latency = (long)latency.TotalMilliseconds;
        }

        public string CustomJson
        {
            get;
            private set;
        }

        public ulong BandwidthUpInKilobitsPerSecond
        {
            get;
            private set;
        }

        public ulong BandwidthDownInKilobitsPerSecond
        {
            get;
            private set;
        }

        public TimeSpan Latency
        {
            get;
            private set;
        }

        public string MemberDeviceToken
        {
            get;
            private set;
        }

    }
}
