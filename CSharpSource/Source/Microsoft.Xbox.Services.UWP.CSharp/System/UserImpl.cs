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

        private IntPtr m_xboxLiveUser_c;
        private int m_signOutHandlerContext;

        private static ConcurrentDictionary<IntPtr, UserImpl> s_xboxLiveUserInstanceMap = new ConcurrentDictionary<IntPtr, UserImpl>();

        public UserImpl(User systemUser)
        {
            this.CreationContext = systemUser;

            var handle = GCHandle.Alloc(m_xboxLiveUser_c, GCHandleType.Pinned);
            var ppUser = GCHandle.ToIntPtr(handle);

            var xsapiResult = XboxLive.Instance.Invoke<XsapiResult, XboxLiveUserCreateFromSystemUser>(
                this.CreationContext == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(this.CreationContext),
                ppUser
                );

            m_xboxLiveUser_c = Marshal.ReadIntPtr(ppUser);
            handle.Free();

            m_signOutHandlerContext = XboxLive.Instance.Invoke<Int32, AddSignOutCompletedHandler>(
                (SignOutCompletedHandler)OnSignOutCompleted
                );

            s_xboxLiveUserInstanceMap[m_xboxLiveUser_c] = this;

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
            XboxLive.Instance.Invoke<RemoveSignOutCompletedHandler>(
                m_signOutHandlerContext
                );

            if (null != m_xboxLiveUser_c)
            {
                XboxLive.Instance.Invoke<XboxLiveUserDelete>(m_xboxLiveUser_c);
            }
        }

        private static void OnSignOutCompleted(IntPtr xboxLiveUser_c)
        {
            UserImpl @this = s_xboxLiveUserInstanceMap[xboxLiveUser_c];

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

                if (showUI)
                {
                    XboxLive.Instance.Invoke<XsapiResult, XboxLiveUserSignInWithCoreDispatcher>(
                        m_xboxLiveUser_c,
                        coreDispatcherPtr,
                        (SignInCompletionRoutine)SignInComplete,
                        (IntPtr)contextKey,
                        XboxLive.DefaultTaskGroupId
                        );
                }
                else
                {
                    XboxLive.Instance.Invoke<XsapiResult, XboxLiveUserSignInSilently>(
                        m_xboxLiveUser_c,
                        (SignInCompletionRoutine)SignInComplete,
                        (IntPtr)contextKey,
                        XboxLive.DefaultTaskGroupId
                        );
                }
            });

            return tcs.Task;
        }

        private static void SignInComplete(SignInResult_c result, IntPtr context)
        {
            int contextKey = context.ToInt32();

            XboxLiveCallbackContext<UserImpl, SignInResult> contextObject;
            if (XboxLiveCallbackContext<UserImpl, SignInResult>.TryRemove(contextKey, out contextObject))
            {
                UserImpl @this = contextObject.Context;
                if (result.result.errorCode == 0)
                {
                    @this.UpdatePropertiesFromXboxLiveUser_c();
                    @this.SignInCompleted(@this, new EventArgs());

                    contextObject.TaskCompletionSource.SetResult(new SignInResult(result.payload.status));
                }
                else
                {
                    contextObject.TaskCompletionSource.SetException(new Exception(result.result.errorMessage));
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

                XboxLive.Instance.Invoke<XsapiResult, XboxLiveUserGetTokenAndSignature>(
                    m_xboxLiveUser_c,
                    pHttpMethod,
                    pUrl,
                    pHeaders,
                    pBody,
                    (GetTokenAndSignatureCompletionRoutine)GetTokenAndSignatureComplete,
                    (IntPtr)contextKey,
                    XboxLive.DefaultTaskGroupId
                    );
            });

            return tcs.Task;
        }

        private static void GetTokenAndSignatureComplete(TokenAndSignatureResult_c result, IntPtr context)
        {
            int contextKey = context.ToInt32();

            XboxLiveCallbackContext<UserImpl, TokenAndSignatureResult> contextObject;
            if (XboxLiveCallbackContext<UserImpl, TokenAndSignatureResult>.TryRemove(contextKey, out contextObject))
            {
                if (result.result.errorCode == 0)
                {
                    contextObject.TaskCompletionSource.SetResult(new TokenAndSignatureResult
                    {
                        WebAccountId = result.payload.WebAccountId,
                        Privileges = result.payload.Privileges,
                        AgeGroup = result.payload.AgeGroup,
                        Gamertag = result.payload.Gamertag,
                        XboxUserId = result.payload.XboxUserId,
                        Signature = result.payload.Signature,
                        Token = result.payload.Token
                        //TokenRequestResultStatus = tokenResult.ResponseStatus
                    });
                }
                else
                {
                    contextObject.TaskCompletionSource.SetException(new Exception(result.result.errorMessage));
                }
                contextObject.Dispose();
            }
        }

        private void UpdatePropertiesFromXboxLiveUser_c()
        {
            var xboxLiveUser_c = Marshal.PtrToStructure<XboxLiveUser_c>(m_xboxLiveUser_c);

            this.XboxUserId = MarshalingHelpers.Utf8ToString(xboxLiveUser_c.XboxUserId);
            this.Gamertag = MarshalingHelpers.Utf8ToString(xboxLiveUser_c.Gamertag);
            this.AgeGroup = MarshalingHelpers.Utf8ToString(xboxLiveUser_c.AgeGroup);
            this.Privileges = MarshalingHelpers.Utf8ToString(xboxLiveUser_c.Privileges);
            this.IsSignedIn = Convert.ToBoolean(xboxLiveUser_c.IsSignedIn);
            this.WebAccountId = MarshalingHelpers.Utf8ToString(xboxLiveUser_c.WebAccountId);

            if (xboxLiveUser_c.WindowsSystemUser != IntPtr.Zero)
            {
                var user = Marshal.GetObjectForIUnknown(xboxLiveUser_c.WindowsSystemUser);
                if (user is Windows.System.User)
                {
                    this.CreationContext = user as User;
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SignInCompletionRoutine(SignInResult_c result, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GetTokenAndSignatureCompletionRoutine(TokenAndSignatureResult_c result, IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SignOutCompletedHandler(IntPtr xboxLiveUser_c);

        private delegate XsapiResult XboxLiveUserCreateFromSystemUser(IntPtr systemUser, IntPtr ppXboxLiveUser_c);
        private delegate void XboxLiveUserDelete(IntPtr xboxLiveUser_c);
        private delegate XsapiResult XboxLiveUserSignInWithCoreDispatcher(IntPtr xboxLiveUser_c, IntPtr coreDispatcher, SignInCompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);
        private delegate XsapiResult XboxLiveUserSignInSilently(IntPtr xboxLiveUser_c, SignInCompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);
        private delegate XsapiResult XboxLiveUserGetTokenAndSignature(IntPtr xboxLiveUser_c, IntPtr httpMethod, IntPtr url, IntPtr headers, IntPtr requestBodyString, GetTokenAndSignatureCompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);
        private delegate Int32 AddSignOutCompletedHandler(SignOutCompletedHandler handler);
        private delegate void RemoveSignOutCompletedHandler(Int32 functionContext);

        [StructLayout(LayoutKind.Sequential)]
        private struct XboxLiveUser_c
        {
            [MarshalAsAttribute(UnmanagedType.SysInt)]
            public IntPtr XboxUserId;

            [MarshalAsAttribute(UnmanagedType.SysInt)]
            public IntPtr Gamertag;

            [MarshalAsAttribute(UnmanagedType.SysInt)]
            public IntPtr AgeGroup;

            [MarshalAsAttribute(UnmanagedType.SysInt)]
            public IntPtr Privileges;

            [MarshalAsAttribute(UnmanagedType.U1)]
            public byte IsSignedIn;

            [MarshalAsAttribute(UnmanagedType.SysInt)]
            public IntPtr WebAccountId;

            [MarshalAsAttribute(UnmanagedType.SysInt)]
            public IntPtr WindowsSystemUser;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct SignInResultPayload_c
        {
            public SignInStatus status;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SignInResult_c
        {
            public XboxLiveResult result;
            public SignInResultPayload_c payload;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TokenAndSignatureResultPayload_c
        {
            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string Token;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string Signature;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string XboxUserId;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string Gamertag;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string XboxUserHash;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string AgeGroup;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string Privileges;

            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string WebAccountId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TokenAndSignatureResult_c
        {
            public XboxLiveResult result;
            public TokenAndSignatureResultPayload_c payload;
        }
    }
}