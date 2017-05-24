// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using Newtonsoft.Json;

namespace Microsoft.Xbox.Services
{
    internal static class JsonSerialization
    {
        internal static T FromJson<T>(string jsonInput)
        {
            return JsonConvert.DeserializeObject<T>(jsonInput);
        }

        internal static string ToJson<T>(T input)
        {
            return JsonConvert.SerializeObject(input);
        }
    }
}
