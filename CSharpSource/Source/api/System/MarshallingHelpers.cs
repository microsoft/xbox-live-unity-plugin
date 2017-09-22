// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Text;

    internal static class MarshalingHelpers
    {
        internal static IntPtr StringToHGlobalUtf8(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var pointer = Marshal.AllocHGlobal(bytes.Length + 1);

            Marshal.Copy(bytes, 0, pointer, bytes.Length);

            // Add null terminator
            Marshal.WriteByte(IntPtr.Add(pointer, bytes.Length), 0);
                        
            return pointer;
        }

        internal static string Utf8ToString(IntPtr utf8)
        {
            List<byte> rawBytes = new List<byte>();
            byte nextByte = byte.MaxValue;

            for (; nextByte != 0; utf8 = IntPtr.Add(utf8, 1))
            {
                rawBytes.Add(nextByte = Marshal.ReadByte(utf8));
            }
            return Encoding.UTF8.GetString(rawBytes.ToArray(), 0, rawBytes.Count - 1);
        }
    }
}