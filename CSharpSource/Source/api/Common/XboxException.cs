// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;

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
    }
}