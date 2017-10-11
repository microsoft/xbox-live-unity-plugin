// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
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
        public AuthConfig AuthConfig { get; private set; } // TODO remove this
        public User CreationContext { get; private set; }

        private IntPtr xboxLiveUserPtr;
        private int signOutHandlerContext;

        private static ConcurrentDictionary<IntPtr, UserImpl> xboxLiveUserInstanceMap = new ConcurrentDictionary<IntPtr, UserImpl>();

        public UserImpl(User systemUser)
        {
            this.CreationContext = systemUser;

            var forceInit = XboxLive.Instance;
            var result = XboxLiveUserCreateFromSystemUser(
                this.CreationContext == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(this.CreationContext),
                out xboxLiveUserPtr);

            if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
            {
                throw new XboxException(result);
            }
            
            Init();
        }

        internal UserImpl(IntPtr cXboxLiveUser)
        {
            xboxLiveUserPtr = cXboxLiveUser;
            Init();
        }

        void Init()
        {
            signOutHandlerContext = AddSignOutCompletedHandler(OnSignOutCompleted);
            xboxLiveUserInstanceMap[xboxLiveUserPtr] = this;

            // TODO: This config is broken.
            var appConfig = XboxLiveAppConfiguration.Instance;
            this.AuthConfig = new AuthConfig
            {
                Sandbox = appConfig.Sandbox,
                EnvironmentPrefix = appConfig.EnvironmentPrefix,
                Environment = appConfig.Environment,
                UseCompactTicket = appConfig.UseFirstPartyToken
            };
        }

        ~UserImpl()
        {
            RemoveSignOutCompletedHandler(signOutHandlerContext);

            if (null != xboxLiveUserPtr)
            {
                XboxLiveUserDelete(xboxLiveUserPtr);
            }
        }
        
        internal IntPtr GetPtr() { return xboxLiveUserPtr; }

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
                IntPtr coreDispatcherPtr = default(IntPtr);
                if (showUI)
                {
                    coreDispatcherPtr = Marshal.GetIUnknownForObject(Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher);
                }

                int contextKey = XboxLiveCallbackContext<UserImpl, SignInResult>.CreateContext(
                    this,
                    tcs,
                    showUI ? new List<IntPtr> { coreDispatcherPtr } : null,
                    null);

                XSAPI_RESULT result = XSAPI_RESULT.XSAPI_RESULT_OK;
                if (showUI)
                {
                    result = XboxLiveUserSignInWithCoreDispatcher(
                        xboxLiveUserPtr, 
                        coreDispatcherPtr, 
                        SignInComplete, 
                        (IntPtr)contextKey, 
                        XboxLive.DefaultTaskGroupId);
                }
                else
                {
                    result = XboxLiveUserSignInSilently(
                        xboxLiveUserPtr, 
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

        private static void SignInComplete(SIGN_IN_RESULT result, IntPtr context)
        {
            int contextKey = context.ToInt32();

            XboxLiveCallbackContext<UserImpl, SignInResult> contextObject;
            if (XboxLiveCallbackContext<UserImpl, SignInResult>.TryRemove(contextKey, out contextObject))
            {
                UserImpl @this = contextObject.Context;
                if (result.result.errorCode == 0)
                {
                    @this.UpdatePropertiesFromXboxLiveUserPtr();
                    @this.SignInCompleted(@this, new EventArgs());

                    contextObject.TaskCompletionSource.SetResult(new SignInResult(result.payload.status));
                }
                else
                {
                    contextObject.TaskCompletionSource.SetException(new Exception(MarshalingHelpers.Utf8ToString(result.result.errorMessage)));
                }
                contextObject.Dispose();
            }
        }

        public Task<TokenAndSignatureResult> InternalGetTokenAndSignatureAsync(string httpMethod, string url, string headers, byte[] body, bool promptForCredentialsIfNeeded, bool forceRefresh)
        {
            var tcs = new TaskCompletionSource<TokenAndSignatureResult>();

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

                int contextKey = XboxLiveCallbackContext<UserImpl, TokenAndSignatureResult>.CreateContext(
                    this,
                    tcs, 
                    null,
                    new List<IntPtr> { pHttpMethod, pUrl, pHeaders, pBody });

                var result = XboxLiveUserGetTokenAndSignature(xboxLiveUserPtr, 
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

        private static void GetTokenAndSignatureComplete(TOKEN_AND_SIGNATURE_RESULT result, IntPtr context)
        {
            int contextKey = context.ToInt32();

            XboxLiveCallbackContext<UserImpl, TokenAndSignatureResult> contextObject;
            if (XboxLiveCallbackContext<UserImpl, TokenAndSignatureResult>.TryRemove(contextKey, out contextObject))
            {
                if (result.result.errorCode == 0)
                {
                    contextObject.TaskCompletionSource.SetResult(new TokenAndSignatureResult
                    {
                        WebAccountId = MarshalingHelpers.Utf8ToString(result.payload.WebAccountId),
                        Privileges = MarshalingHelpers.Utf8ToString(result.payload.Privileges),
                        AgeGroup = MarshalingHelpers.Utf8ToString(result.payload.AgeGroup),
                        Gamertag = MarshalingHelpers.Utf8ToString(result.payload.Gamertag),
                        XboxUserId = MarshalingHelpers.Utf8ToString(result.payload.XboxUserId),
                        Signature = MarshalingHelpers.Utf8ToString(result.payload.Signature),
                        Token = MarshalingHelpers.Utf8ToString(result.payload.Token)
                        //TokenRequestResultStatus = tokenResult.ResponseStatus
                    });
                }
                else
                {
                    contextObject.TaskCompletionSource.SetException(new Exception(MarshalingHelpers.Utf8ToString(result.result.errorMessage)));
                }
                contextObject.Dispose();
            }
        }

        internal void UpdatePropertiesFromXboxLiveUserPtr()
        {
            var xboxLiveUserStruct = Marshal.PtrToStructure<XBOX_LIVE_USER>(xboxLiveUserPtr);

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
        private delegate void XSAPI_SIGN_IN_COMPLETION_ROUTINE(SIGN_IN_RESULT result, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_GET_TOKEN_AND_SIGNATURE_COMPLETION_ROUTINE(TOKEN_AND_SIGNATURE_RESULT result, IntPtr context);

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
        private struct XBOX_LIVE_USER
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
        private struct SIGN_IN_RESULT_PAYLOAD
        {
            public SignInStatus status;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SIGN_IN_RESULT
        {
            public XSAPI_RESULT_INFO result;
            public SIGN_IN_RESULT_PAYLOAD payload;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_AND_SIGNATURE_RESULT_PAYLOAD
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

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_AND_SIGNATURE_RESULT
        {
            public XSAPI_RESULT_INFO result;
            public TOKEN_AND_SIGNATURE_RESULT_PAYLOAD payload;
        }
    }
}