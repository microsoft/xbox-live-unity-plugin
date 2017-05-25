// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;

    using Newtonsoft.Json;

    public static class MockXboxLiveData
    {
        public static Dictionary<string, MockRequestData> MockResponses { get; set; }

        static MockXboxLiveData()
        {
            MockResponses = new Dictionary<string, MockRequestData>();
        }

        public static void Load(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            string rawData = File.ReadAllText(path);
            MockResponses = JsonConvert.DeserializeObject<Dictionary<string, MockRequestData>>(rawData);
        }

        public static XboxLiveHttpResponse GetMockResponse(XboxLiveHttpRequest request)
        {
            XboxLiveHttpRequestEqualityComparer comparer = new XboxLiveHttpRequestEqualityComparer();
            foreach (var mockData in MockResponses.Values)
            {
                if (comparer.Equals(request, mockData.Request))
                {
                    return mockData.Response;
                }
            }

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "X-XblCorrelationId", Guid.NewGuid().ToString() },
                { "Date", DateTime.UtcNow.ToString("R") },
            };

            return new MockXboxLiveHttpResponse(404, headers);
        }

        public class MockRequestData
        {
            public MockXboxLiveHttpRequest Request { get; set; }
            public MockXboxLiveHttpResponse Response { get; set; }
        }
    }
}