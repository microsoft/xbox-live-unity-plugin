// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using static Microsoft.Xbox.Services.Statistics.Manager.StatsManager;

    public class StatValue
    {
        public string Name { get; private set; }
        public double AsNumber { get; private set; }
        public long AsInteger { get; private set; }
        public string AsString { get; private set; }
        public StatValueType Type { get; private set; }
        
        internal StatValue(IntPtr statValuePtr)
        {
            StatValue_c cStatValue = Marshal.PtrToStructure<StatValue_c>(statValuePtr);
            Name = cStatValue.Name;
            AsNumber = cStatValue.AsNumber;
            AsInteger = cStatValue.AsInteger;
            AsString = cStatValue.AsString;
            Type = cStatValue.DataType;
        }
    }
}