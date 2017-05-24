// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MockXboxLiveHttpResponse : XboxLiveHttpResponse
    {
        public MockXboxLiveHttpResponse(int httpStatus) : this(httpStatus, null, null, new Dictionary<string, string>())
        {
        }

        public MockXboxLiveHttpResponse(int httpStatus, Dictionary<string, string> headers) : this(httpStatus, null, null, headers)
        {
        }

        [JsonConstructor]
        public MockXboxLiveHttpResponse(int httpStatus, JObject body, string characterSet = null, Dictionary<string, string> headers = null)
        {
            string bodyJson = JsonConvert.SerializeObject(body);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(bodyJson ?? "");
            Stream bodyStream = new MemoryStream(bodyBytes);

            WebHeaderCollection webHeaders = new WebHeaderCollection();
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    webHeaders[header.Key] = header.Value;
                }
            }

            this.Initialize(httpStatus, bodyStream, bodyStream.Length, characterSet ?? "utf-8", webHeaders);
        }
    }
}