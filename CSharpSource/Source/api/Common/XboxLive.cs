// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.IO;
    using global::System.ComponentModel;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.Statistics.Manager;

    public partial class XboxLive : IDisposable
    {
        private bool disposed;
        private static XboxLive instance;
        private static IntPtr xsapiNativeDll;
        private XboxLiveSettings settings;
        private IStatsManager statsManager;
        private ISocialManager socialManager;

        private static readonly object instanceLock = new object();
        private readonly XboxLiveAppConfiguration appConfig;

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

            // default for AnyCPU/x86
            string fileName = @"Microsoft.Xbox.Services.140.UWP.C.Win32.dll";
            if (IntPtr.Size == 8)
            {
                // Detect Process is 64bit
                fileName = @"Microsoft.Xbox.Services.140.UWP.C.x64.dll";
            }

            try
            {
                xsapiNativeDll = LoadNativeDll(fileName);
            }
            catch (Exception)
            {
                throw new XboxException("Failed to load " + fileName);
            }
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

        internal static class NativeMethods
        {
            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32")]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        }

        public static IntPtr LoadNativeDll(string fileName)
        {
            IntPtr nativeDll = NativeMethods.LoadLibrary(fileName);
            if (nativeDll == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            return nativeDll;
        }

        public T Invoke<T, T2>(params object[] args)
        {
            IntPtr procAddress = NativeMethods.GetProcAddress(xsapiNativeDll, typeof(T2).Name);
            if (procAddress == IntPtr.Zero)
            {
                return default(T);
            }

            var function = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T2));
            return (T)function.DynamicInvoke(args);
        }

        public void Invoke<T>(params object[] args)
        {
            IntPtr procAddress = NativeMethods.GetProcAddress(xsapiNativeDll, typeof(T).Name);
            if (procAddress == IntPtr.Zero)
            {
                return;
            }

            var function = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T));
            function.DynamicInvoke(args);
        }
    }
}
