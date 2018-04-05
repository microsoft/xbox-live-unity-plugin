// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.IO;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.Statistics.Manager;
    using Microsoft.Xbox.Services.System;

    public partial class XboxLive : IDisposable
    {
        private bool disposed;
        private static XboxLive instance;
        private IStatisticManager statsManager;
        private ISocialManager socialManager;

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
            try
            {
#if WINDOWS_UWP
                var result = XsapiGlobalInitialize();
                if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    throw new XboxException(result);
                }
#endif
                // TODO flat C APIs for settings
                //this.Settings = null;
                this.appConfig = XboxLiveAppConfiguration.SingletonInstance;
            }
            catch (FileLoadException)
            {
                this.appConfig = null;
            }
        }

        ~XboxLive()
        {
            XsapiGlobalCleanup();
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

        public IStatisticManager StatsManager
        {
            get
            {
                if (Instance.statsManager == null)
                {
                    Instance.statsManager = Statistics.Manager.StatisticManager.Instance;
                }
                return Instance.statsManager;
            }
        }

        //public XboxLiveContextSettings Settings { get; private set; }

        public XboxLiveAppConfiguration AppConfig
        {
            get
            {
                return Instance.appConfig;
            }
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

        internal static Int64 DefaultTaskGroupId
        {
            get
            {
                return 0;
            }
        }
        
        [DllImport(XboxLive.FlatCDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern XSAPI_RESULT XsapiGlobalInitialize();

        [DllImport(XboxLive.FlatCDllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void XsapiGlobalCleanup();
    }
}
