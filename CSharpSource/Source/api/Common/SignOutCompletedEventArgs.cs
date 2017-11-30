// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;

    public class SignOutCompletedEventArgs : EventArgs
    {
        public SignOutCompletedEventArgs(IXboxLiveUser user)
        {
            this.User = user;
        }

        // TODO change this to XboxLiveUser instead of IXboxLive user to match WinRT projections
        public IXboxLiveUser User { get; private set; }
    }
}