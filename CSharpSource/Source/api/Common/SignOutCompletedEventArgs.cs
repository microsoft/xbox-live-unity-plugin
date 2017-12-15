// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;

    public class SignOutCompletedEventArgs : EventArgs
    {
        public SignOutCompletedEventArgs(XboxLiveUser user)
        {
            this.User = user;
        }

        public XboxLiveUser User { get; private set; }
    }
}