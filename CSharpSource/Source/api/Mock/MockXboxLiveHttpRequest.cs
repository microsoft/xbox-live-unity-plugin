// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Diagnostics;
    using global::System.IO;
    using global::System.Threading.Tasks;

    using Newtonsoft.Json;

    public class MockXboxLiveHttpRequest : XboxLiveHttpRequest
    {
        public static string MockDataPath;

        public MockXboxLiveHttpRequest(string method, string serverName, string pathQueryFragment) : base(method, serverName, pathQueryFragment)
        {
        }

        public override Task<XboxLiveHttpResponse> GetResponseWithoutAuth()
        {
            // Save the mock data out for testing.
            string requestData = JsonConvert.SerializeObject(this, Formatting.Indented);

            string outputDir = @"C:\Temp\MockData";
            if(!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string outputPath = Path.Combine(outputDir, "data.txt");
            using (var stream = this.GetWriteStream(outputPath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(requestData);
                }
            }
            return Task.FromResult(MockXboxLiveData.GetMockResponse(this));
        }

        // This is used because there are times when multiple requests are issued at the same time
        // As a result the output file becomes locked for the first request resulting in the other
        // requests being unable to edit the file.
        private FileStream GetWriteStream(string path, int timeoutMs = 1000)
        {
            var time = Stopwatch.StartNew();
            while (time.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    return new FileStream(path, FileMode.Append, FileAccess.Write);
                }
                catch (IOException)
                {
                }
            }

            throw new TimeoutException(string.Format("Failed to get a write handle to {0} within {1} ms.", path, timeoutMs));
        }
    }
}