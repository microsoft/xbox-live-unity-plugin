// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xbox.Services.System;

namespace Microsoft.Xbox.Services.Shared
{
    interface IXboxWebsocketClient : IDisposable
    {
        Task<object> Connect(
            XboxLiveUser xblUser,
            string uri,
            string subprotocol);

        void Send(string message, Action<bool> onSendComplete);

        void Close();
    }
}
