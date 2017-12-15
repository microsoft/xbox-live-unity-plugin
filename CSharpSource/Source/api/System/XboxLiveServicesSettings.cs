// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.System
{
    using global::System;

    public class XboxLiveServicesSettings
    {
        public XboxServicesDiagnosticsTraceLevel DiagnosticsTraceLevel { get; set; }

        public static XboxLiveServicesSettings SingletonInstance { get; private set; }

        public event EventHandler<XboxLiveLogCallEventArgs> LogCallRouted;
    }
}