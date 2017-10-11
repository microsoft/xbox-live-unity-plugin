// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.IO;
    using global::System.ComponentModel;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Presence;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.Statistics.Manager;
    using Microsoft.Xbox.Services.System;

    public partial class XboxLive : IDisposable
    {
        private bool disposed;
        private static XboxLive instance;
        private XboxLiveSettings settings;
        private IStatsManager statsManager;
        private ISocialManager socialManager;
        private IPresenceWriter presenceWriter;

        private static readonly object instanceLock = new object();
        private readonly XboxLiveAppConfiguration appConfig;

#if WINDOWS_UWP
        internal const string FlatCDllName = "Microsoft.Xbox.Services.140.UWP.C.dll";
#else
        // TODO This should change for other unity editor and XDK
        internal const string FlatCDllName = "TODO";
#endif

        private XboxLive()
        {
            this.settings = new XboxLiveSettings();

            try
            {
                this.appConfig = XboxLiveAppConfiguration.Instance;
            }
            catch (FileLoadException)
            {
                this.appConfig = null;
            }

#if WINDOWS_UWP
            var result = XBLGlobalInitialize();
            if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(result);
            }
#endif
        }

        ~XboxLive()
        {
            XBLGlobalCleanup();
        }

        public static XboxLive Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new XboxLive();
                        }
                    }
                }
                return instance;
            }

            private set
            {
                instance = null;
            }
        }

        public IPresenceWriter PresenceWriter {
            get 
            {
                if (Instance.presenceWriter == null) {
                    Instance.presenceWriter = Presence.PresenceWriter.Instance;
                }
                return Instance.presenceWriter;
            }
        }

        public ISocialManager SocialManager
        {
            get
            {
                if (Instance.socialManager == null)
                {
                    Instance.socialManager = Social.Manager.SocialManager.Instance;
                }
                return Instance.socialManager;
            }
        }

        public IStatsManager StatsManager
        {
            get
            {
                if (Instance.statsManager == null)
                {
                    Instance.statsManager = Statistics.Manager.StatsManager.Instance;
                }
                return Instance.statsManager;
            }
        }

        public XboxLiveSettings Settings
        {
            get { return Instance.settings; }
            set { Instance.settings = value; }
        }

        public XboxLiveAppConfiguration AppConfig
        {
            get { return Instance.appConfig; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Instance = null;
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static Int64 DefaultTaskGroupId
        {
            get
            {
                return 0;
            }
        }
        
        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT XBLGlobalInitialize();

        [DllImport(XboxLive.FlatCDllName)]
        private static extern void XBLGlobalCleanup();
    }
}
