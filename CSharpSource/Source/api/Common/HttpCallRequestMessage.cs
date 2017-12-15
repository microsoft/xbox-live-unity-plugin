// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::System;

    public class HttpCallRequestMessage
    {
        public HttpRequestMessageType GetHttpRequestMessageType { get; private set; }

        public Byte[] RequestMessageVector { get; private set; }

        public string RequestMessageString { get; private set; }
    }
}
