// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using global::System.Text;

    internal static class MarshalingHelpers
    {
        internal static IntPtr StringToHGlobalUtf8(string str)
        {
            if (str == null)
            {
                return IntPtr.Zero;
            }

            var bytes = Encoding.UTF8.GetBytes(str);
            var pointer = Marshal.AllocHGlobal(bytes.Length + 1);

            Marshal.Copy(bytes, 0, pointer, bytes.Length);
            // Add null terminator
            Marshal.WriteByte(pointer.Increment(bytes.Length), 0);

            return pointer;
        }

        internal static string Utf8ToString(IntPtr utf8)
        {
            if (utf8 == IntPtr.Zero)
            {
                return null;
            }

            List<byte> rawBytes = new List<byte>();
            byte nextByte = byte.MaxValue;

            for (; nextByte != 0; utf8 = utf8.Increment())
            {
                rawBytes.Add(nextByte = Marshal.ReadByte(utf8));
            }
            return Encoding.UTF8.GetString(rawBytes.ToArray(), 0, rawBytes.Count - 1);
        }

        /// <summary>
        /// Creates a List of strings from an unmanaged C-style array of strings.
        /// </summary>
        /// <param name="arrayPtr">Pointer to the C-style string array</param>
        /// <param name="length">Number of strings in the array</param>
        /// <returns></returns>
        internal static IList<string> Utf8StringArrayToStringList(IntPtr arrayPtr, uint length)
        {
            var list = new List<string>((int)length);
            for (int i = 0; i < length; ++i)
            {
                string str = MarshalingHelpers.Utf8ToString(Marshal.ReadIntPtr(arrayPtr));
                list.Add(str);
                arrayPtr = arrayPtr.Increment(IntPtr.Size);
            }
            return list;
        }


        /// <summary>
        /// Returns a C-style array of strings given an IList<string>. The allocated array should later be freed with FreeHGlobalUtf8StringArray
        /// </summary>
        /// <param name="strings">The array of string to allocate</param>
        /// <returns>An IntPtr to the first Utf8String pointer</returns>
        internal static IntPtr StringListToHGlobalUtf8StringArray(IEnumerable<string> strings)
        {
            var firstString = Marshal.AllocHGlobal(IntPtr.Size * strings.Count());
            int offset = 0;
            foreach (var str in strings)
            {
                Marshal.WriteIntPtr(firstString, offset * IntPtr.Size, StringToHGlobalUtf8(str));
                offset++;
            }
            return firstString;
        }

        /// <summary>
        /// Frees a C-style array of strings created with CreateHGlobalUtf8StringArray.
        /// </summary>
        /// <param name="arrayPtr">Pointer to a string array created with CreateHGlobalUtf8StringArray</param>
        /// <param name="length">Number of strings in the array</param>
        internal static void FreeHGlobalUtf8StringArray(IntPtr arrayPtr, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                var stringPtr = Marshal.ReadIntPtr(arrayPtr, IntPtr.Size * i);
                Marshal.FreeHGlobal(stringPtr);
            }
            Marshal.FreeHGlobal(arrayPtr);
        }

        internal static IntPtr Increment(this IntPtr ptr, int offset = 1)
        {
#if DOTNET_3_5

            return new IntPtr(ptr.ToInt64() + offset);
#else
            return IntPtr.Add(ptr, offset);
#endif
        }

        internal static int SizeOf<T>() where T: new()
        {
#if DOTNET_3_5
            return Marshal.SizeOf(new T());
#else
            return Marshal.SizeOf<T>();
#endif
        }

        internal static T PtrToStructure<T>(IntPtr ptr)
        {
#if DOTNET_3_5
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
#else
            return Marshal.PtrToStructure<T>(ptr);
#endif
        }


        internal static DateTimeOffset FromUnixTimeSeconds(long seconds)
        {
#if DOTNET_3_5
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(Convert.ToDouble(seconds));
#else
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
#endif
        }

#if DOTNET_3_5
        /// <summary>
        /// Converts a DateTimeOffset to a time_t to be used in flat C code. A similar extension method
        /// already exists in .NET4.6
        /// </summary>
        /// <param name="dateTimeOffset">The DateTimeOffset to convert</param>
        /// <returns></returns>
        internal static long ToUnixTimeSeconds(this DateTimeOffset dateTimeOffset)
        {
            TimeSpan span = dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return Convert.ToInt64(span.TotalSeconds);
        }
#endif
    }
}