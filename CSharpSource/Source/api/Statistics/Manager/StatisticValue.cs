// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using static Microsoft.Xbox.Services.Statistics.Manager.StatisticManager;

    public class StatisticValue
    {
        public string Name { get; internal set; }
        public double AsNumber { get; internal set; }
        public long AsInteger { get; internal set; }
        public string AsString { get; internal set; }
        public StatisticDataType DataType { get; internal set; }
        
        internal StatisticValue(IntPtr statValuePtr)
        {
            StatValue_c cStatValue = Marshal.PtrToStructure<StatValue_c>(statValuePtr);
            Name = cStatValue.Name;
            AsNumber = cStatValue.AsNumber;
            AsInteger = cStatValue.AsInteger;
            AsString = cStatValue.AsString;
            DataType = cStatValue.DataType;
        }

        // Used for mock services
        internal StatisticValue()
        {

        }
    }
}