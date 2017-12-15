// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services
{
    using global::System;

    public class XboxLiveLogCallEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public string Category { get; private set; }

        public XboxServicesDiagnosticsTraceLevel Level { get; private set; }
    }
}