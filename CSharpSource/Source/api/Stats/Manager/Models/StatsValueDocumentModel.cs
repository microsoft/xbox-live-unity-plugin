// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xbox.Services.Statistics.Manager.Models
{
    class StatsValueDocumentModel
    {
        [JsonProperty("$schema")]
        public string Schema
        {
            get { return "http://stats.xboxlive.com/2017-1/schema#"; }
        }

        [JsonProperty("revision")]
        public int Revision { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("stats")]
        public Stats Stats { get; set; }
    }

    public class Stats
    {
        [JsonProperty("title")]
        public Dictionary<string, Stat> Title { get; set; }
    }

    public class Stat
    {
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
