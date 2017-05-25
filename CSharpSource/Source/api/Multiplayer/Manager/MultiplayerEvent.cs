// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public class MultiplayerEvent
    {

        public MultiplayerSessionType SessionType
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.Multiplayer.Manager.ultiplayerEventArgs EventArgs
        {
            get;
            private set;
        }

        public MultiplayerEventType EventType
        {
            get;
            private set;
        }

        public IntPtr Context
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        public int ErrorCode
        {
            get;
            private set;
        }

    }
}
