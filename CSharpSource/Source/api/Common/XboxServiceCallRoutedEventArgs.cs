// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services
{
    public class XboxServiceCallRoutedEventArgs : EventArgs
    {

        public string XboxUserId
        {
            get;
            private set;
        }

        public TimeSpan ElapsedCallTime
        {
            get;
            private set;
        }

        public DateTimeOffset ResponseTimeUTC
        {
            get;
            private set;
        }

        public DateTimeOffset RequestTimeUTC
        {
            get;
            private set;
        }

        public string FullResponseToString
        {
            get;
            private set;
        }

        public uint HttpStatus
        {
            get;
            private set;
        }

        public string Signature
        {
            get;
            private set;
        }

        public string Token
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string ResponseBody
        {
            get;
            private set;
        }

        public string ResponseHeaders
        {
            get;
            private set;
        }

        public uint ResponseCount
        {
            get;
            private set;
        }

        public HttpCallRequestMessage RequestBody
        {
            get;
            private set;
        }

        public string RequestHeaders
        {
            get;
            private set;
        }

        public Uri Url
        {
            get;
            private set;
        }

        public string HttpMethod
        {
            get;
            private set;
        }

    }
}
