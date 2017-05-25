// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;

    public class XboxLiveSettings
    {
        private const int DefaultHttpTimeoutWindowInSeconds = 20;
        private const int DefaultLongHttpTimeoutInSeconds = 5 * 60;
        private const int DefaultRetryDelayInSeconds = 2;

        public XboxLiveSettings()
        {
            this.DiagnosticsTraceLevel = XboxServicesDiagnosticsTraceLevel.Off;
            this.HttpTimeoutWindow = TimeSpan.FromSeconds(DefaultHttpTimeoutWindowInSeconds);
            this.LongHttpTimeout = TimeSpan.FromSeconds(DefaultLongHttpTimeoutInSeconds);
            this.HttpRetryDelay = TimeSpan.FromSeconds(DefaultRetryDelayInSeconds);
        }

        public bool UseCoreDispatcherForEventRouting { get; set; }

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

        //public TimeSpan WebsocketTimeoutWindow { get; set; }

        //public event EventHandler<XboxLiveLogCallEventArgs> LogCallRouted;

        //public bool EnableServiceCallRoutedEvents { get; set; }

        //public void DisableAssertsForMaximumNumberOfWebsocketsActivated(XboxLiveContextRecommendedSetting setting)
        //{
        //    throw new NotImplementedException();
        //}
    }
}