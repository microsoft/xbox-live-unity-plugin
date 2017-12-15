// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;

    public class XboxLiveContextSettings
    {
        public XboxLiveContextSettings()
        {
        }

        public bool UseCoreDispatcherForEventRouting { get; set; }

        public TimeSpan HttpTimeout { get; set; }

        public TimeSpan HttpTimeoutWindow { get; set; }

        public TimeSpan HttpRetryDelay { get; set; }

        public TimeSpan LongHttpTimeout { get; set; }

        public XboxServicesDiagnosticsTraceLevel DiagnosticsTraceLevel { get; private set; }

        public bool AreAssertsForThrottlingInDevSandboxesDisabled { get; private set; }

        public void DisableAssertsForXboxLiveThrottlingInDevSandboxes(XboxLiveContextThrottleSetting setting)
        {
            if (setting == XboxLiveContextThrottleSetting.ThisCodeNeedsToBeChangedToAvoidThrottling)
            {
                this.AreAssertsForThrottlingInDevSandboxesDisabled = true;
            }
        }

        public TimeSpan WebsocketTimeoutWindow { get; set; }

        public event EventHandler<XboxLiveLogCallEventArgs> LogCallRouted;

        public bool EnableServiceCallRoutedEvents { get; set; }

        public void DisableAssertsForMaximumNumberOfWebsocketsActivated(XboxLiveContextRecommendedSetting setting)
        {
            throw new NotImplementedException();
        }
    }
}