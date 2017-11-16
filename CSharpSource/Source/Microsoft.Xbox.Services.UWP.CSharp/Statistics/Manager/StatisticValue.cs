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
            STATISTIC_VALUE cStatValue = Marshal.PtrToStructure<STATISTIC_VALUE>(statValuePtr);
            Name = MarshalingHelpers.Utf8ToString(cStatValue.Name);
            AsNumber = cStatValue.AsNumber;
            AsInteger = cStatValue.AsInteger;
            AsString = MarshalingHelpers.Utf8ToString(cStatValue.AsString);
            DataType = cStatValue.DataType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct STATISTIC_VALUE
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr Name;

            [MarshalAs(UnmanagedType.R8)]
            public double AsNumber;

            [MarshalAs(UnmanagedType.I8)]
            public long AsInteger;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr AsString;

            [MarshalAs(UnmanagedType.U4)]
            public StatisticDataType DataType;
        }
    }
}
