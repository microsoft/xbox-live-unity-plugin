// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;

public class XboxLiveUserEventArgs : EventArgs
{
    public XboxLiveUserEventArgs(XboxLiveUser user)
    {
        this.User = user;
    }

    public XboxLiveUser User { get; private set; }
}