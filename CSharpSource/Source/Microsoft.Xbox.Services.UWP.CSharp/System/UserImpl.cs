// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::AOT;
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;
    using Windows.System;

    internal class UserImpl : IUserImpl
    {
        public event EventHandler SignInCompleted;
        public event EventHandler SignOutCompleted;

        private readonly object userImplLock = new object();

        public bool IsSignedIn { get; private set; }
        public string XboxUserId { get; private set; }
        public string Gamertag { get; private set; }
        public string AgeGroup { get; private set; }
        public string Privileges { get; private set; }
        public string WebAccountId { get; private set; }
        public User CreationContext { get; private set; }
        public IntPtr XboxLiveUserPtr { get; private set; }


        private int signOutHandlerContext;
        private static ConcurrentDictionary<IntPtr, UserImpl> xboxLiveUserInstanceMap = new ConcurrentDictionary<IntPtr, UserImpl>();

        public UserImpl(User systemUser)
        {
            this.CreationContext = systemUser;

            var forceInit = XboxLive.Instance;

            IntPtr xboxLiveUserPtr;
            var result = XboxLiveUserCreateFromSystemUser(
                this.CreationContext == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(this.CreationContext),
                out xboxLiveUserPtr);

            this.XboxLiveUserPtr = xboxLiveUserPtr;

            if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(result);
            }
            Init();
        }

        internal UserImpl(IntPtr cXboxLiveUser)
        {
            XboxLiveUserPtr = cXboxLiveUser;
            Init();
        }

        void Init()
        {
            signOutHandlerContext = AddSignOutCompletedHandler(OnSignOutCompleted);
            xboxLiveUserInstanceMap[XboxLiveUserPtr] = this;
            var appConfig = XboxLiveAppConfiguration.SingletonInstance;
        }

        ~UserImpl()
        {
            RemoveSignOutCompletedHandler(signOutHandlerContext);

            if (null != XboxLiveUserPtr)
            {
                XboxLiveUserDelete(XboxLiveUserPtr);
            }
        }

        [MonoPInvokeCallback(typeof(XSAPI_SIGN_OUT_COMPLETED_HANDLER))]
        private static void OnSignOutCompleted(IntPtr xboxLiveUser_c)
        {
            UserImpl @this = xboxLiveUserInstanceMap[xboxLiveUser_c];
            if (!@this.IsSignedIn)
            {
                return;
            }

            lock (@this.userImplLock)
            {
                @this.IsSignedIn = false;
            }

            @this.SignOutCompleted(@this, new EventArgs());

            lock (@this.userImplLock)
            {
                // Check again on isSignedIn flag, in case users signed in again in signOutHandlers callback,
                // so we don't clean up the properties. 
                if (!@this.IsSignedIn)
                {
                    @this.XboxUserId = null;
                    @this.Gamertag = null;
                    @this.AgeGroup = null;
                    @this.Privileges = null;
                    @this.WebAccountId = null;
                }
            }
        }

        public Task<SignInResult> SignInImpl(bool showUI, bool forceRefresh)
        {
            var tcs = new TaskCompletionSource<SignInResult>();

            Task.Run(() =>
            {
                int contextKey;
                var context = XsapiCallbackContext<UserImpl, SignInResult>.CreateContext(this, tcs, out contextKey);

                XSAPI_RESULT result = XSAPI_RESULT.XSAPI_RESULT_OK;
                if (showUI)
                {
                    var coreDispatcherPtr = Marshal.GetIUnknownForObject(Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher);
                    context.PointersToRelease = new List<IntPtr> { coreDispatcherPtr };

                    result = XboxLiveUserSignInWithCoreDispatcher(
                        XboxLiveUserPtr, 
                        coreDispatcherPtr, 
                        SignInComplete, 
                        (IntPtr)contextKey, 
                        XboxLive.DefaultTaskGroupId);
                }
                else
                {
                    result = XboxLiveUserSignInSilently(
                        XboxLiveUserPtr, 
                        SignInComplete, 
                        (IntPtr)contextKey, 
                        XboxLive.DefaultTaskGroupId);
                }

                if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    throw new XboxException(result);
                }
            });

            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(XSAPI_SIGN_IN_COMPLETION_ROUTINE))]
        private static void SignInComplete(XSAPI_RESULT_INFO result, XSAPI_SIGN_IN_RESULT payload, IntPtr context)
        {
            int contextKey = context.ToInt32();

            XsapiCallbackContext<UserImpl, SignInResult> contextObject;
            if (XsapiCallbackContext<UserImpl, SignInResult>.TryRemove(contextKey, out contextObject))
            {
                UserImpl @this = contextObject.Context;
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    @this.UpdatePropertiesFromXboxLiveUserPtr();
                    @this.SignInCompleted(@this, new EventArgs());

                    contextObject.TaskCompletionSource.SetResult(new SignInResult(payload.status));
                }
                else
                {
                    contextObject.TaskCompletionSource.SetException(new XboxException(result));
                }
                contextObject.Dispose();
            }
        }

        public Task<GetTokenAndSignatureResult> InternalGetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh)
        {
            var tcs = new TaskCompletionSource<GetTokenAndSignatureResult>();

            Task.Run(() =>
            {
                IntPtr pHttpMethod = MarshalingHelpers.StringToHGlobalUtf8(httpMethod);
                IntPtr pUrl = MarshalingHelpers.StringToHGlobalUtf8(url);
                IntPtr pHeaders = MarshalingHelpers.StringToHGlobalUtf8(headers);
                
                IntPtr pBody = IntPtr.Zero;
                if (body != null)
                {
                    Marshal.AllocHGlobal(body.Length + 1);
                    Marshal.Copy(body, 0, pBody, body.Length);
                    Marshal.WriteByte(pBody, body.Length, 0);
                }

                int contextKey;
                var context = XsapiCallbackContext<UserImpl, GetTokenAndSignatureResult>.CreateContext(this, tcs, out contextKey);
                context.PointersToFree = new List<IntPtr> { pHttpMethod, pUrl, pHeaders, pBody };

                var result = XboxLiveUserGetTokenAndSignature(
                    XboxLiveUserPtr, 
                    pHttpMethod, 
                    pUrl, 
                    pHeaders, 
                    pBody,
                    GetTokenAndSignatureComplete, 
                    (IntPtr)contextKey, 
                    XboxLive.DefaultTaskGroupId);

                if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    throw new XboxException(result);
                }
            });

            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(XSAPI_GET_TOKEN_AND_SIGNATURE_COMPLETION_ROUTINE))]
        private static void GetTokenAndSignatureComplete(XSAPI_RESULT_INFO result, XSAPI_TOKEN_AND_SIGNATURE_RESULT payload, IntPtr context)
        {
            int contextKey = context.ToInt32();

            XsapiCallbackContext<UserImpl, GetTokenAndSignatureResult> contextObject;
            if (XsapiCallbackContext<UserImpl, GetTokenAndSignatureResult>.TryRemove(contextKey, out contextObject))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    contextObject.TaskCompletionSource.SetResult(new GetTokenAndSignatureResult
                    {
                        WebAccountId = MarshalingHelpers.Utf8ToString(payload.WebAccountId),
                        Privileges = MarshalingHelpers.Utf8ToString(payload.Privileges),
                        AgeGroup = MarshalingHelpers.Utf8ToString(payload.AgeGroup),
                        Gamertag = MarshalingHelpers.Utf8ToString(payload.Gamertag),
                        XboxUserId = MarshalingHelpers.Utf8ToString(payload.XboxUserId),
                        Signature = MarshalingHelpers.Utf8ToString(payload.Signature),
                        Token = MarshalingHelpers.Utf8ToString(payload.Token)
                    });
                }
                else
                {
                    contextObject.TaskCompletionSource.SetException(new XboxException(result));
                }
                contextObject.Dispose();
            }
        }

        internal void UpdatePropertiesFromXboxLiveUserPtr()
        {
            var xboxLiveUserStruct = Marshal.PtrToStructure<XSAPI_XBOX_LIVE_USER>(XboxLiveUserPtr);

            this.XboxUserId = MarshalingHelpers.Utf8ToString(xboxLiveUserStruct.XboxUserId);
            this.Gamertag = MarshalingHelpers.Utf8ToString(xboxLiveUserStruct.Gamertag);
            this.AgeGroup = MarshalingHelpers.Utf8ToString(xboxLiveUserStruct.AgeGroup);
            this.Privileges = MarshalingHelpers.Utf8ToString(xboxLiveUserStruct.Privileges);
            this.IsSignedIn = Convert.ToBoolean(xboxLiveUserStruct.IsSignedIn);
            this.WebAccountId = MarshalingHelpers.Utf8ToString(xboxLiveUserStruct.WebAccountId);

            if (xboxLiveUserStruct.WindowsSystemUser != IntPtr.Zero)
            {
                var user = Marshal.GetObjectForIUnknown(xboxLiveUserStruct.WindowsSystemUser);
                if (user is Windows.System.User)
                {
                    this.CreationContext = user as User;
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_SIGN_IN_COMPLETION_ROUTINE(XSAPI_RESULT_INFO result, XSAPI_SIGN_IN_RESULT payload, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_GET_TOKEN_AND_SIGNATURE_COMPLETION_ROUTINE(XSAPI_RESULT_INFO result, XSAPI_TOKEN_AND_SIGNATURE_RESULT payload, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_SIGN_OUT_COMPLETED_HANDLER(IntPtr xboxLiveUserPtr);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT XboxLiveUserCreateFromSystemUser(
            IntPtr systemUser, 
            out IntPtr xboxLiveUserPtr);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern void XboxLiveUserDelete(IntPtr xboxLiveUserPtr);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT XboxLiveUserSignInWithCoreDispatcher(
            IntPtr xboxLiveUserPtr, 
            IntPtr coreDispatcher, 
            XSAPI_SIGN_IN_COMPLETION_ROUTINE completionRoutine, 
            IntPtr completionRoutineContext, 
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT XboxLiveUserSignInSilently(
            IntPtr xboxLiveUserPtr, 
            XSAPI_SIGN_IN_COMPLETION_ROUTINE completionRoutine, 
            IntPtr completionRoutineContext, 
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT XboxLiveUserGetTokenAndSignature(
            IntPtr xboxLiveUserPtr, 
            IntPtr httpMethod,
            IntPtr url, 
            IntPtr headers, 
            IntPtr requestBodyString, 
            XSAPI_GET_TOKEN_AND_SIGNATURE_COMPLETION_ROUTINE completionRoutine, 
            IntPtr completionRoutineContext, 
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern Int32 AddSignOutCompletedHandler(XSAPI_SIGN_OUT_COMPLETED_HANDLER handler);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern void RemoveSignOutCompletedHandler(Int32 functionContext);

        [StructLayout(LayoutKind.Sequential)]
        private struct XSAPI_XBOX_LIVE_USER
        {
            public IntPtr XboxUserId;
            public IntPtr Gamertag;
            public IntPtr AgeGroup;
            public IntPtr Privileges;
            [MarshalAsAttribute(UnmanagedType.U1)]
            public bool IsSignedIn;
            public IntPtr WebAccountId;
            public IntPtr WindowsSystemUser;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct XSAPI_SIGN_IN_RESULT
        {
            public SignInStatus status;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XSAPI_TOKEN_AND_SIGNATURE_RESULT
        {
            public IntPtr Token;
            public IntPtr Signature;
            public IntPtr XboxUserId;
            public IntPtr Gamertag;
            public IntPtr XboxUserHash;
            public IntPtr AgeGroup;
            public IntPtr Privileges;
            public IntPtr WebAccountId;
        }
    }
}