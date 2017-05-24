// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Diagnostics;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Net;
    using global::System.Reflection;
    using global::System.Runtime.InteropServices;
    using global::System.Text;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    public class XboxLiveHttpRequest
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string SignatureHeaderName = "Signature";
        private const string RangeHeaderName = "Range";
        private const string ContentLengthHeaderName = "Content-Length";
        private const double DefaultHttpTimeoutSeconds = 30.0;
        private const double MinHttpTimeoutSeconds = 5.0;
        private const int HttpStatusCodeTooManyRequests = 429;
        private const double MaxDelayTimeInSec = 60.0;
        private const int MinDelayForHttpInternalErrorInSec = 10;
        private static string userAgentVersion;

        internal XboxLiveHttpRequest(string method, string serverName, string pathQueryFragment)
        {
            this.iterationNumber = 0;
            this.Method = method;
            this.Url = serverName + pathQueryFragment;
            this.contextSettings = XboxLive.Instance.Settings;
            this.webRequest = (HttpWebRequest)WebRequest.Create(new Uri(this.Url));
            this.webRequest.Method = method;
            this.ResponseBodyType = HttpCallResponseBodyType.StringBody;
            this.RetryAllowed = true;

            this.SetCustomHeader("Accept-Language", CultureInfo.CurrentUICulture + "," + CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
#if WINDOWS_UWP
            this.SetCustomHeader("Accept", "*/*");
#else
            this.webRequest.Accept = "*/*";
#endif
            this.SetCustomHeader("Cache-Control", "no-cache");
            this.ContentType = "application/json; charset=utf-8";
        }

        internal readonly XboxLiveSettings contextSettings;
        internal HttpWebRequest webRequest;
        internal readonly Dictionary<string, string> customHeaders = new Dictionary<string, string>();
        internal bool hasPerformedRetryOn401 { get; set; }
        internal uint iterationNumber { get; set; }
        internal DateTime firstCallStartTime { get; set; }
        internal TimeSpan delayBeforeRetry { get; set; }

        public bool LongHttpCall { get; set; }
        public string Method { get; private set; }
        public string Url { get; private set; }
        public string ContractVersion { get; set; }
        public bool RetryAllowed { get; set; } 
        public string ContentType { get; set; }
        public string RequestBody { get; set; }
        public HttpCallResponseBodyType ResponseBodyType { get; set; }
        public XboxLiveUser User { get; private set; }
        public XboxLiveAPIName XboxLiveAPI { get; set; }
        public string CallerContext { get; set; }

        private string Headers
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool isFirstVal = true;
                foreach (var header in this.customHeaders)
                {
                    if (isFirstVal)
                    {
                        isFirstVal = false;
                    }
                    else
                    {
                        sb.AppendLine();
                    }

                    sb.AppendFormat("{0}: {1}", header.Key, header.Value);
                }
                return sb.ToString();
            }
        }

        public Task<XboxLiveHttpResponse> GetResponseWithAuth(XboxLiveUser user)
        {
            TaskCompletionSource<XboxLiveHttpResponse> taskCompletionSource = new TaskCompletionSource<XboxLiveHttpResponse>();
            this.User = user;

            user.GetTokenAndSignatureAsync(this.Method, this.Url, this.Headers).ContinueWith(
                tokenTask =>
                {
                    if (tokenTask.IsFaulted)
                    {
                        taskCompletionSource.SetException(tokenTask.Exception);
                        return;
                    }

                    try
                    {
                        this.SetAuthHeaders(tokenTask.Result);
                        this.SetRequestHeaders();
                        this.InternalGetResponse().ContinueWith(getResponseTask =>
                        {
                            if (getResponseTask.IsFaulted)
                            {
                                taskCompletionSource.SetException(getResponseTask.Exception);
                            }
                            else
                            {
                                taskCompletionSource.SetResult(getResponseTask.Result);
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        taskCompletionSource.SetException(e);
                    }
                });

            return taskCompletionSource.Task;
        }

        private void SetAuthHeaders(TokenAndSignatureResult result)
        {
#if !WINDOWS_UWP
            this.SetCustomHeader(AuthorizationHeaderName, string.Format("XBL3.0 x={0};{1}", result.XboxUserHash, result.Token));
#else
            this.SetCustomHeader(AuthorizationHeaderName, string.Format("{0}", result.Token));
#endif
            this.SetCustomHeader(SignatureHeaderName, result.Signature);
        }

        private void SetRequestHeaders()
        {
            if (!string.IsNullOrEmpty(this.ContractVersion))
            {
                this.SetCustomHeader("x-xbl-contract-version", this.ContractVersion);
            }

            foreach (KeyValuePair<string, string> customHeader in this.customHeaders)
            {
                this.webRequest.Headers[customHeader.Key] = customHeader.Value;
            }
        }

        public virtual Task<XboxLiveHttpResponse> GetResponseWithoutAuth()
        {
            this.SetRequestHeaders();
            return this.InternalGetResponse();
        }

        private void HandleThrottledCalls(XboxLiveHttpResponse httpCallResponse)
        {
            if (string.Equals(XboxLiveAppConfiguration.Instance.Sandbox, "RETAIL", StringComparison.Ordinal) ||
                this.contextSettings.AreAssertsForThrottlingInDevSandboxesDisabled)
                return;

#if DEBUG
            string msg;
            msg = "Xbox Live service call to " + httpCallResponse.Url.ToString() + " was throttled\r\n";
            msg += httpCallResponse.RequestBody;
            msg += "\r\n";
            msg += "You can temporarily disable the assert by calling\r\n";
            msg += "XboxLive.Instance.Settings.DisableAssertsForXboxLiveThrottlingInDevSandboxes()\r\n";
            msg += "Note that this will only disable this assert.  You will still be throttled in all sandboxes.\r\n";
            Debug.WriteLine(msg);
#endif            

            throw new XboxException("Xbox Live service call was throttled.  See Output for more detail");
        }

        private Task<XboxLiveHttpResponse> InternalGetResponse()
        {
            DateTime requestStartTime = DateTime.UtcNow;
            if (this.iterationNumber == 0)
            {
                this.firstCallStartTime = requestStartTime;
            }
            this.iterationNumber++;

            HttpRetryAfterApiState apiState;
            if (HttpRetryAfterManager.Instance.GetState(this.XboxLiveAPI, out apiState))
            {
                if (this.ShouldFastFail(apiState, requestStartTime))
                {
                    return this.HandleFastFail(apiState);
                }
                else
                {
                    HttpRetryAfterManager.Instance.ClearState(this.XboxLiveAPI);
                }
            }

            this.SetUserAgent();

            TaskCompletionSource<XboxLiveHttpResponse> taskCompletionSource = new TaskCompletionSource<XboxLiveHttpResponse>();

            this.WriteRequestBodyAsync().ContinueWith(writeBodyTask =>
            {
                // The explicit cast in the next method should not be necessary, but Visual Studio is complaining
                // that the call is ambiguous.  This removes that in-editor error. 
                Task.Factory.FromAsync(this.webRequest.BeginGetResponse, (Func<IAsyncResult, WebResponse>)this.webRequest.EndGetResponse, null)
                .ContinueWith(getResponseTask =>
                {
                    var httpWebResponse = ExtractHttpWebResponse(getResponseTask);
                    int httpStatusCode = 0;
                    bool networkFailure = false;
                    if (httpWebResponse != null)
                    {
                        httpStatusCode = (int)httpWebResponse.StatusCode;
                    }
                    else
                    {
                        // classify as network failure if there's no HTTP status code and didn't get response
                        networkFailure = getResponseTask.IsFaulted || getResponseTask.IsCanceled;
                    }

                    var httpCallResponse = new XboxLiveHttpResponse(
                        httpStatusCode,
                        networkFailure,
                        httpWebResponse,
                        DateTime.UtcNow,
                        requestStartTime,
                        this.User != null ? this.User.XboxUserId : "",
                        this.contextSettings,
                        this.Url,
                        this.XboxLiveAPI,
                        this.Method,
                        this.RequestBody,
                        this.ResponseBodyType
                        );

                    if (this.ShouldRetry(httpCallResponse))
                    {
                        // Wait and retry call
                        this.RecordServiceResult(httpCallResponse, getResponseTask.Exception);
                        this.RouteServiceCall(httpCallResponse);
                        Sleep(this.delayBeforeRetry);
                        this.webRequest = CloneHttpWebRequest(this.webRequest);
                        this.InternalGetResponse().ContinueWith(retryGetResponseTask =>
                        {
                            if (retryGetResponseTask.IsFaulted)
                            {
                                taskCompletionSource.SetException(retryGetResponseTask.Exception);
                            }
                            else
                            {
                                taskCompletionSource.SetResult(retryGetResponseTask.Result);
                            }
                        });
                    }
                    else if (!networkFailure) // Got HTTP status code
                    {
                        // HTTP 429: TOO MANY REQUESTS errors should return a JSON debug payload 
                        // describing the details about why the call was throttled
                        this.RecordServiceResult(httpCallResponse, getResponseTask.Exception);
                        this.RouteServiceCall(httpCallResponse);

                        if (httpCallResponse.HttpStatus == HttpStatusCodeTooManyRequests) 
                        {
                            this.HandleThrottledCalls(httpCallResponse);
                        }

                        if (getResponseTask.IsFaulted)
                        {
                            taskCompletionSource.SetException(getResponseTask.Exception);
                        }
                        else
                        {
                            taskCompletionSource.SetResult(httpCallResponse);
                        }
                    }
                    else
                    {
                        // Handle network errors

                        // HandleResponseError(); // TODO: extract error from JSON
                        this.RecordServiceResult(httpCallResponse, getResponseTask.Exception);
                        this.RouteServiceCall(httpCallResponse);
                        taskCompletionSource.SetException(getResponseTask.Exception);
                    }
                });
            });

            return taskCompletionSource.Task;
        }

        private bool ShouldFastFail(
            HttpRetryAfterApiState apiState,
            DateTime currentTime
            )
        {
            if (apiState.Exception == null)
            {
                return false;
            }

            TimeSpan remainingTimeBeforeRetryAfter = apiState.RetryAfterTime - currentTime;
            if (remainingTimeBeforeRetryAfter.Ticks <= 0)
            {
                return false;
            }

            DateTime timeoutTime = this.firstCallStartTime + this.contextSettings.HttpTimeoutWindow;

            // If the Retry-After will happen first, just wait till Retry-After is done, and don't fast fail
            if (apiState.RetryAfterTime < timeoutTime)
            {
                Sleep(remainingTimeBeforeRetryAfter);
                return false;
            }
            else
            {
                return true;
            }
        }

        private Task<XboxLiveHttpResponse> HandleFastFail(HttpRetryAfterApiState apiState)
        {
            XboxLiveHttpResponse httpCallResponse = apiState.HttpCallResponse;
            this.RouteServiceCall(httpCallResponse);

            TaskCompletionSource<XboxLiveHttpResponse> taskCompletionSource = new TaskCompletionSource<XboxLiveHttpResponse>();
            taskCompletionSource.SetException(apiState.Exception);
            return taskCompletionSource.Task;
        }

        private void SetUserAgent()
        {
            const string userAgentType = "XboxServicesAPICSharp";
            lock (XboxLive.Instance)
            {
                if (string.IsNullOrEmpty(userAgentVersion))
                {
#if !WINDOWS_UWP
                    userAgentVersion = typeof(XboxLiveHttpRequest).Assembly.GetName().Version.ToString();
#else
                    userAgentVersion = typeof(XboxLiveHttpRequest).GetTypeInfo().Assembly.GetName().Version.ToString();
#endif
                }
            }

            string userAgent = userAgentType + "/" + userAgentVersion;
            if (!string.IsNullOrEmpty(this.CallerContext)) 
            {
                userAgent += " " + this.CallerContext;
            }
            this.SetCustomHeader("UserAgent", userAgent);
        }

        private bool ShouldRetry(XboxLiveHttpResponse httpCallResponse)
        {
            int httpStatus = httpCallResponse.HttpStatus;

            if (!this.RetryAllowed && 
                !(httpStatus == (int)HttpStatusCode.Unauthorized && this.User != null))
            {
                return false;
            }

            if ((httpStatus == (int)HttpStatusCode.Unauthorized && !this.hasPerformedRetryOn401) ||
                httpStatus == (int)HttpStatusCode.RequestTimeout ||
                httpStatus == HttpStatusCodeTooManyRequests ||
                httpStatus == (int)HttpStatusCode.InternalServerError ||
                httpStatus == (int)HttpStatusCode.BadGateway ||
                httpStatus == (int)HttpStatusCode.ServiceUnavailable ||
                httpStatus == (int)HttpStatusCode.GatewayTimeout ||
                httpCallResponse.NetworkFailure
                )
            {
                TimeSpan retryAfter = httpCallResponse.RetryAfter;

                // Compute how much time left before hitting the HttpTimeoutWindow setting.  
                TimeSpan timeElapsedSinceFirstCall = httpCallResponse.ResponseReceivedTime - this.firstCallStartTime;
                TimeSpan remainingTimeBeforeTimeout = this.contextSettings.HttpTimeoutWindow - timeElapsedSinceFirstCall;
                if (remainingTimeBeforeTimeout.TotalSeconds <= MinHttpTimeoutSeconds) // Need at least 5 seconds to bother making a call
                {
                    return false;
                }

                // Based on the retry iteration, delay 2,4,8,16,etc seconds by default between retries
                // Jitter the response between the current and next delay based on system clock
                // Max wait time is 1 minute
                double secondsToWaitMin = Math.Pow(this.contextSettings.HttpRetryDelay.TotalSeconds, this.iterationNumber);
                double secondsToWaitMax = Math.Pow(this.contextSettings.HttpRetryDelay.TotalSeconds, this.iterationNumber + 1);
                double secondsToWaitDelta = secondsToWaitMax - secondsToWaitMin;
                DateTime responseDate = httpCallResponse.ResponseReceivedTime;
                double randTime =
                    (httpCallResponse.ResponseReceivedTime.Minute * 60.0 * 1000.0) +
                    (httpCallResponse.ResponseReceivedTime.Second * 1000.0) +
                    httpCallResponse.ResponseReceivedTime.Millisecond;
                double lerpScaler = (randTime % 10000) / 10000.0; // from 0 to 1 based on clock

                if (XboxLive.UseMockHttp)
                {
                    lerpScaler = 0; // make tests deterministic
                }

                double secondsToWaitUncapped = secondsToWaitMin + secondsToWaitDelta * lerpScaler; // lerp between min & max wait
                double secondsToWait = Math.Min(secondsToWaitUncapped, MaxDelayTimeInSec); // cap max wait to 1 min
                TimeSpan waitTime = TimeSpan.FromSeconds(secondsToWait);
                if (retryAfter.TotalMilliseconds > 0)
                {
                    // Use either the waitTime or Retry-After header, whichever is bigger
                    this.delayBeforeRetry = (waitTime > retryAfter) ? waitTime : retryAfter;
                }
                else
                {
                    this.delayBeforeRetry = waitTime;
                }

                if (remainingTimeBeforeTimeout < this.delayBeforeRetry + TimeSpan.FromSeconds(MinHttpTimeoutSeconds))
                {
                    // Don't bother retrying when out of time
                    return false;
                }
                
                if (httpStatus == (int)HttpStatusCode.InternalServerError)
                {
                    // For 500 - Internal Error, wait at least 10 seconds before retrying.
                    TimeSpan minDelayForHttpInternalError = TimeSpan.FromSeconds(MinDelayForHttpInternalErrorInSec);
                    if (this.delayBeforeRetry < minDelayForHttpInternalError)
                    {
                        this.delayBeforeRetry = minDelayForHttpInternalError;
                    }
                }
                else if (httpStatus == (int)HttpStatusCode.Unauthorized)
                {
                    return this.HandleUnauthorizedError();
                }

                return true;
            }

            return false;
        }

        private bool HandleUnauthorizedError()
        {
            if (this.User != null) // if this is null, it does not need a valid token anyways
            {
                try
                {
                    Task task = this.User.RefreshToken();
                    task.Wait();
                    this.hasPerformedRetryOn401 = true;
                }
                catch (Exception)
                {
                    return false; // if getting a new token failed, then we need to just return the 401 upwards
                }
            }
            else
            {
                this.hasPerformedRetryOn401 = true;
            }

            return true;
        }

        /// <summary>
        /// If a request body has been provided, this will write it to the stream.  If there is no request body a completed task
        /// will be returned.
        /// </summary>
        /// <returns>A task that represents to request body write work.</returns>
        /// <remarks>This is used to make request chaining a little bit easier.</remarks>
        private Task WriteRequestBodyAsync()
        {
            if (string.IsNullOrEmpty(this.RequestBody))
            {
                return Task.FromResult(true);
            }

            this.webRequest.ContentType = this.ContentType;

#if !WINDOWS_UWP
            this.webRequest.ContentLength = this.RequestBody.Length;
#else
            this.webRequest.Headers[ContentLengthHeaderName] = this.RequestBody.Length.ToString();
#endif

            // The explicit cast in the next method should not be necessary, but Visual Studio is complaining
            // that the call is ambiguous.  This removes that in-editor error. 
            return Task.Factory.FromAsync(this.webRequest.BeginGetRequestStream, (Func<IAsyncResult, Stream>)this.webRequest.EndGetRequestStream, null)
                .ContinueWith(t =>
                {
                    using (Stream body = t.Result)
                    {
                        using (StreamWriter sw = new StreamWriter(body))
                        {
                            sw.Write(this.RequestBody);
                            sw.Flush();
                        }
                    }
                });
        }

        public void SetCustomHeader(string headerName, string headerValue)
        {
            if (!this.customHeaders.ContainsKey(headerName))
            {
                this.customHeaders.Add(headerName, headerValue);
            }
            else
            {
                this.customHeaders[headerName] = headerValue;
            }
        }

        public static XboxLiveHttpRequest Create(string httpMethod, string serverName, string pathQueryFragment)
        {
            return XboxLive.UseMockHttp ?
                new MockXboxLiveHttpRequest(httpMethod, serverName, pathQueryFragment) :
                new XboxLiveHttpRequest(httpMethod, serverName, pathQueryFragment);
        }

        public void SetRangeHeader(uint startByte, uint endByte)
        {
            var byteRange = "bytes=" + startByte + "-" + endByte;
#if !WINDOWS_UWP
                this.webRequest.AddRange((int)startByte, (int)endByte);
#else
                this.webRequest.Headers[RangeHeaderName] = byteRange;
#endif
        }

        /// <summary>
        /// Creates a query string out of a list of parameters
        /// </summary>
        /// <param name="paramDictionary">List of Parameters to be added to the query</param>
        /// <returns>a query string that should be appended to the request</returns>
        public static string GetQueryFromParams(Dictionary<string, string> paramDictionary)
        {
            var queryString = new StringBuilder();
            if (paramDictionary.Count > 0)
            {
                queryString.Append("?");
                const string queryDelimiter = "&";
                var firstParameter = true;
                foreach (var paramPair in paramDictionary)
                {
                    if (firstParameter)
                        firstParameter = false;
                    else
                        queryString.Append(queryDelimiter);

                    queryString.Append(string.Format("{0}={1}", paramPair.Key, paramPair.Value));
                }
            }

            return queryString.ToString();
        }

        private void RecordServiceResult(XboxLiveHttpResponse httpCallResponse, Exception exception)
        {
            // Only remember result if there was an error and there was a Retry-After header
            if (this.XboxLiveAPI != XboxLiveAPIName.Unspecified &&
                httpCallResponse.HttpStatus >= 400 //&&
                //httpCallResponse.RetryAfter.TotalSeconds > 0
                )
            {
                DateTime currentTime = DateTime.UtcNow;
                HttpRetryAfterApiState state = new HttpRetryAfterApiState();
                state.RetryAfterTime = currentTime + httpCallResponse.RetryAfter;
                state.HttpCallResponse = httpCallResponse;
                state.Exception = exception;
                HttpRetryAfterManager.Instance.SetState(this.XboxLiveAPI, state);
            }
        }

        private void RouteServiceCall(XboxLiveHttpResponse httpCallResponse)
        {
            // TODO: port route logic
        }

        public static void Sleep(TimeSpan timeSpan)
        {
            // WinRT doesn't have Thread.Sleep, so using ManualResetEvent
            new ManualResetEvent(false).WaitOne(timeSpan);
        }

        private HttpWebResponse ExtractHttpWebResponse(Task<WebResponse> getResponseTask)
        {
            if (getResponseTask.IsFaulted && getResponseTask.Exception != null)
            {
                if (getResponseTask.Exception.InnerException is WebException)
                {
                    WebException e = (WebException)getResponseTask.Exception.InnerException;
                    if (e.Response is HttpWebResponse)
                    {
                        HttpWebResponse w = (HttpWebResponse)e.Response;
                        return w;
                    }
                }

                return null;
            }
            else
            {
                return (HttpWebResponse)getResponseTask.Result;
            }
        }

        public static HttpWebRequest CloneHttpWebRequest(HttpWebRequest original)
        {
            HttpWebRequest clone = (HttpWebRequest)WebRequest.Create(original.RequestUri.AbsoluteUri);
            PropertyInfo[] properties = original.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "ContentLength" && property.Name != "Headers")
                {
                    object value = property.GetValue(original, null);
                    if (property.CanWrite)
                    {
                        property.SetValue(clone, value, null);
                    }
                }
            }
            foreach (var item in original.Headers.AllKeys)
            {
                clone.Headers[item] = original.Headers[item];
            }

            return clone;
        }
    }
}
