// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;
    using Microsoft.Xbox.Services.System;

    public class XboxException : Exception
    {
        public XboxException()
        {
        }

        public XboxException(string message) : base(message)
        {
        }

        public XboxException(int HResult, string message) : base(message)
        {
            this.HResult = HResult;
        }

        public XboxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal XboxException(XSAPI_RESULT result)
            : base(string.Format("Xbox Services flat C API returned error code {0}", result.ToString("g")))
        {
        }

        internal XboxException(XSAPI_RESULT_INFO resultInfo)
            : base (string.Format("Xbox Services flat C API return error code {0} with message \"{1}\"", resultInfo.errorCode.ToString("g"), MarshalingHelpers.Utf8ToString(resultInfo.errorMessage)))
        {
        }
    }
}