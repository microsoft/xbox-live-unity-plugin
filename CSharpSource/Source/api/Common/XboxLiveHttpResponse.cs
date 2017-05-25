// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Text;

    public class XboxLiveHttpResponse
    {
        public const string RetryAfterHeader = "Retry-After";
        public const int RetryAfterCapInSeconds = 15;
        public const string ETagHeader = "ETag";
        public const string DateHeader = "Date";

        public TimeSpan RetryAfter { get; private set; }

        public DateTime ResponseReceivedTime { get; private set; }

        public DateTime RequestStartTime { get; private set; }

        public string ETag { get; private set; }

        public string ResponseDate { get; private set; }

        public string ErrorMessage { get; private set; }

        public int ErrorCode { get; private set; }

        public int HttpStatus { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }

        public byte[] ResponseBodyVector { get; private set; }

        public string ResponseBodyString { get; private set; }

        public HttpWebResponse response { get; private set; }

        public string XboxUserId { get; private set; }

        public XboxLiveSettings ContextSettings { get; private set; }

        public string Url { get; private set; }

        public XboxLiveAPIName XboxLiveAPI { get; private set; }

        public string Method { get; private set; }

        public string RequestBody { get; private set; }

        public HttpCallResponseBodyType ResponseBodyType { get; private set; }

        public bool NetworkFailure { get; private set; }

        internal XboxLiveHttpResponse()
        {
        }

        internal XboxLiveHttpResponse(
            int httpStatusCode,
            bool networkFailure,
            HttpWebResponse response, 
            DateTime responseReceivedTime, 
            DateTime requestStartTime,
            string xboxUserId,
            XboxLiveSettings contextSettings,                        
            string url,
            XboxLiveAPIName xboxLiveAPI,
            string method,
            string requestBody,
            HttpCallResponseBodyType responseBodyType
            )
        {
            this.HttpStatus = httpStatusCode;
            this.NetworkFailure = networkFailure;
            this.response = response;
            this.ResponseReceivedTime = responseReceivedTime;
            this.RequestStartTime = requestStartTime;
            this.XboxUserId = xboxUserId;
            this.ContextSettings = contextSettings;
            this.Url = url;
            this.XboxLiveAPI = xboxLiveAPI;
            this.Method = method;
            this.RequestBody = requestBody;
            this.ResponseBodyType = responseBodyType;

            if (response != null)
            {
                using (Stream body = response.GetResponseStream())
                {
                    this.Initialize((int)response.StatusCode, body, response.ContentLength, "utf-8", response.Headers);
                }
            }
        }

        protected void Initialize(int httpStatus, Stream body, long contentLength, string characterSet, WebHeaderCollection headers)
        {
            if (this.HttpStatus == 0)
            {
                this.HttpStatus = httpStatus;
            }
            this.Headers = new Dictionary<string, string>();

            int vectorSize = contentLength > int.MaxValue ? int.MaxValue : (int)contentLength;
            this.ResponseBodyVector = new byte[vectorSize];
            if (contentLength > 0)
            {
                int totalBytesRead = 0;
                do
                {
                    int bytesRead = body.Read(this.ResponseBodyVector, totalBytesRead, this.ResponseBodyVector.Length - totalBytesRead);

                    // This means we're at the end of the stream.
                    if (bytesRead == 0)
                    {
                        throw new ArgumentException(string.Format("Expected body stream to contain {0} bytes but only read {1} bytes.", contentLength, totalBytesRead), "body");
                    }

                    totalBytesRead += bytesRead;
                }
                while (totalBytesRead < contentLength);


                if (this.ResponseBodyType != HttpCallResponseBodyType.VectorBody)
                {
                    Encoding encoding;
                    switch (characterSet.ToLower())
                    {
                        case "utf-8":
                            encoding = Encoding.UTF8;
                            break;
                        case "ascii":
                            encoding = Encoding.ASCII;
                            break;
                        default:
                            encoding = Encoding.UTF8;
                            break;
                    }

                    this.ResponseBodyString = encoding.GetString(this.ResponseBodyVector);
                }
            }

            for (int i = 0; i < headers.Count; ++i)
            {
                var key = headers.AllKeys[i];
                this.Headers.Add(key, headers[key]);
            }

            string retryAfterInSeconds;
            this.Headers.TryGetValue(RetryAfterHeader, out retryAfterInSeconds);
            if (!string.IsNullOrEmpty(retryAfterInSeconds))
            {
                int numRetryAfterInSeconds = 0;
                int.TryParse(retryAfterInSeconds, out numRetryAfterInSeconds);
                numRetryAfterInSeconds = Math.Min(numRetryAfterInSeconds, RetryAfterCapInSeconds);
                this.RetryAfter = TimeSpan.FromSeconds(numRetryAfterInSeconds);
            }

            string value = string.Empty;
            this.Headers.TryGetValue(ETagHeader, out value);
            this.ETag = value;

            value = string.Empty;
            this.Headers.TryGetValue(DateHeader, out value);
            this.ResponseDate = value;
        }
    }
}

