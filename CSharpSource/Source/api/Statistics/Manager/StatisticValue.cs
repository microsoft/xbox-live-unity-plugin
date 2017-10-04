// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    public partial class StatisticValue
    {
        public string Name { get; internal set; }
        public double AsNumber { get; internal set; }
        public long AsInteger { get; internal set; }
        public string AsString { get; internal set; }
        public StatisticDataType DataType { get; internal set; }
        
        // Used for mock services
        internal StatisticValue()
        {

        }
    }
}