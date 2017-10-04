// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;

    public partial class StatisticValue
    {
        internal StatisticValue(IntPtr statValuePtr)
        {
            StatisticValue_c cStatValue = Marshal.PtrToStructure<StatisticValue_c>(statValuePtr);
            Name = cStatValue.Name;
            AsNumber = cStatValue.AsNumber;
            AsInteger = cStatValue.AsInteger;
            AsString = cStatValue.AsString;
            DataType = cStatValue.DataType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StatisticValue_c
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Name;

            [MarshalAs(UnmanagedType.R8)]
            public double AsNumber;

            [MarshalAs(UnmanagedType.I8)]
            public long AsInteger;

            [MarshalAs(UnmanagedType.LPStr)]
            public string AsString;

            [MarshalAs(UnmanagedType.U4)]
            public StatisticDataType DataType;
        }
    }
}
