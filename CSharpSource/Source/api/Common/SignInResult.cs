// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    public class SignInResult
    {
        public SignInResult(SignInStatus status)
        {
            this.Status = status;
        }

        public SignInStatus Status { get; internal set; }
    }
}