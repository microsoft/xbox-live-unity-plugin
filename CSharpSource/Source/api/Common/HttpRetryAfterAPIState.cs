// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;

    public class HttpRetryAfterApiState
    {
        public DateTime RetryAfterTime { get; set; }
        public Exception Exception { get; set; }
        public XboxLiveHttpResponse HttpCallResponse { get; set; }
    }
}


