// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Runtime.InteropServices;
    using Microsoft.Xbox.Services.Privacy;
    using Microsoft.Xbox.Services.System;
    using Microsoft.Xbox.Services.TitleStorage;

    public partial class XboxLiveContext
    {
        public PrivacyService PrivacyService { get; private set; }
        public TitleStorageService TitleStorageService { get; private set; }
        
        public XboxLiveContext(XboxLiveUser user)
        {
#if WINDOWS_UWP
            IntPtr xboxLiveContext;
            var xsapiResult = XboxLiveContextCreate(user.PCXboxLiveUser, out xboxLiveContext);

            if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(xsapiResult);
            }
            XboxLiveContextPtr = xboxLiveContext;

            this.TitleStorageService = new TitleStorageService(XboxLiveContextPtr);
            this.PrivacyService = new PrivacyService(XboxLiveContextPtr);
#else
            // TODO MockServices and/or MockHttp
            this.TitleStorageService = null;
            this.PrivacyService = null;
#endif
        }

        ~XboxLiveContext()
        {
            XboxLiveContextDelete(XboxLiveContextPtr);
        }

        internal IntPtr XboxLiveContextPtr { get; private set; }


        [DllImport(XboxLive.FlatCDllName)]
        internal static extern XSAPI_RESULT XboxLiveContextCreate(IntPtr xboxLiveUser, out IntPtr xboxLiveContext);

        [DllImport(XboxLive.FlatCDllName)]
        internal static extern void XboxLiveContextDelete(IntPtr xboxLiveContext);
    }
}