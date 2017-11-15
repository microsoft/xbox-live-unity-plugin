// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.Privacy
{
    using global::Microsoft.Xbox.Services.System;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;

    public class PrivacyService : IPrivacyService
    {
        internal IntPtr pCXboxLiveContext;

        internal PrivacyService(IntPtr pCXboxLiveContext)
        {
            this.pCXboxLiveContext = pCXboxLiveContext;
        }

        public Task<IList<string>> GetAvoidListAsync()
        {
            var tcs = new TaskCompletionSource<IList<string>>();
            Task.Run(() =>
            {
                int contextKey = XsapiCallbackContext<object, IList<string>>.CreateContext(null, tcs);

                var xsapiResult = PrivacyGetAvoidList(this.pCXboxLiveContext, GetPrivacyUserListComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);
                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        public Task<IList<string>> GetMuteListAsync()
        {
            var tcs = new TaskCompletionSource<IList<string>>();
            Task.Run(() =>
            {
                int contextKey = XsapiCallbackContext<object, IList<string>>.CreateContext(null, tcs);

                var xsapiResult = PrivacyGetMuteList(this.pCXboxLiveContext, GetPrivacyUserListComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);
                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void GetPrivacyUserListComplete(XSAPI_RESULT_INFO result, IntPtr xboxUserIdList, UInt64 count, IntPtr contextKey)
        {
            XsapiCallbackContext<object, IList<string>> context;
            if (XsapiCallbackContext<object, IList<string>>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.TaskCompletionSource.SetResult(MarshalingHelpers.Utf8StringArrayToStringList(xboxUserIdList, (int)count));
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                context.Dispose();
            }
        }

        public Task<PermissionCheckResult> CheckPermissionWithTargetUserAsync(string permissionId, string targetXboxUserId)
        {
            var tcs = new TaskCompletionSource<PermissionCheckResult>();
            Task.Run(() =>
            {
                var permissionIdPtr = MarshalingHelpers.StringToHGlobalUtf8(permissionId);
                var xuidPtr = MarshalingHelpers.StringToHGlobalUtf8(targetXboxUserId);

                int contextKey;
                var context = XsapiCallbackContext<object, PermissionCheckResult>.CreateContext(null, tcs, out contextKey);
                context.PointersToFree = new List<IntPtr> { permissionIdPtr, xuidPtr };

                var xsapiResult = PrivacyCheckPermissionWithTargetUser(
                    this.pCXboxLiveContext, permissionIdPtr, xuidPtr, CheckPermissionWithTargetUserComplete, 
                    (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void CheckPermissionWithTargetUserComplete(XSAPI_RESULT_INFO result, XSAPI_PRIVACY_PERMISSION_CHECK_RESULT payload, IntPtr contextKey)
        {
            XsapiCallbackContext<object, PermissionCheckResult> context;
            if (XsapiCallbackContext<object, PermissionCheckResult>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    context.TaskCompletionSource.SetResult(new PermissionCheckResult(payload));
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                context.Dispose();
            }
        }

        public Task<List<MultiplePermissionsCheckResult>> CheckMultiplePermissionsWithMultipleTargetUsersAsync(IList<string> permissionIds, IList<string> targetXboxUserIds)
        {
            var tcs = new TaskCompletionSource<List<MultiplePermissionsCheckResult>>();
            Task.Run(() =>
            {
                var permissionIdsArray = MarshalingHelpers.StringListToHGlobalUtf8StringArray(permissionIds);
                var xuidsArray = MarshalingHelpers.StringListToHGlobalUtf8StringArray(targetXboxUserIds);

                int contextKey = XsapiCallbackContext<CheckMultiplePermissionsContext, List<MultiplePermissionsCheckResult>>.CreateContext(
                    new CheckMultiplePermissionsContext
                    {
                        permissionIdsArray = permissionIdsArray,
                        permissionIdsLength = permissionIds.Count,
                        targetXboxUserIdsArray = xuidsArray,
                        targetXboxUserIdsLength = targetXboxUserIds.Count
                    }, tcs);

                var xsapiResult = PrivacyCheckMultiplePermissionsWithMultipleTargetUsers(
                    this.pCXboxLiveContext, permissionIdsArray, (ulong)permissionIds.Count, xuidsArray, (ulong)targetXboxUserIds.Count,
                    CheckMultiplePermissionsWithMultipleTargetUsersComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (xsapiResult != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(xsapiResult));
                }
            });
            return tcs.Task;
        }

        private void CheckMultiplePermissionsWithMultipleTargetUsersComplete(XSAPI_RESULT_INFO result, IntPtr resultsPtr, UInt64 privacyCheckResultCount, IntPtr contextKey)
        {
            XsapiCallbackContext<CheckMultiplePermissionsContext, List<MultiplePermissionsCheckResult>> context;
            if (XsapiCallbackContext<CheckMultiplePermissionsContext, List<MultiplePermissionsCheckResult>>.TryRemove(contextKey.ToInt32(), out context))
            {
                if (result.errorCode == XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    var results = new List<MultiplePermissionsCheckResult>();

                    var structSize = MarshalingHelpers.SizeOf<XSAPI_PRIVACY_MULTIPLE_PERMISSIONS_CHECK_RESULT>();
                    for (UInt64 i = 0; i < privacyCheckResultCount; ++i)
                    { 
                        results.Add(new MultiplePermissionsCheckResult(resultsPtr));
                        resultsPtr = resultsPtr.Increment(structSize);
                    }
                    context.TaskCompletionSource.SetResult(results);
                }
                else
                {
                    context.TaskCompletionSource.SetException(new XboxException(result));
                }
                MarshalingHelpers.FreeHGlobalUtf8StringArray(context.Context.permissionIdsArray, context.Context.permissionIdsLength);
                MarshalingHelpers.FreeHGlobalUtf8StringArray(context.Context.targetXboxUserIdsArray, context.Context.targetXboxUserIdsLength);
                context.Dispose();
            }
        }

        internal class CheckMultiplePermissionsContext
        {
            public IntPtr permissionIdsArray;
            public int permissionIdsLength;
            public IntPtr targetXboxUserIdsArray;
            public int targetXboxUserIdsLength;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_PRIVACY_GET_USER_LIST_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            IntPtr xboxUserIds,
            UInt64 xboxUserIdsCount,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT PrivacyGetAvoidList(
            IntPtr xboxLiveContext, 
            XSAPI_PRIVACY_GET_USER_LIST_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT PrivacyGetMuteList(
            IntPtr xboxLiveContext, 
            XSAPI_PRIVACY_GET_USER_LIST_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_PRIVACY_CHECK_PERMISSION_WITH_TARGET_USER_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            XSAPI_PRIVACY_PERMISSION_CHECK_RESULT payload,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT PrivacyCheckPermissionWithTargetUser(
            IntPtr xboxLiveContext,
            IntPtr permissionId,
            IntPtr xboxUserId,
            XSAPI_PRIVACY_CHECK_PERMISSION_WITH_TARGET_USER_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_PRIVACY_CHECK_PERMISSION_WITH_MULTIPLE_TARGET_USERS_COMPLETION_ROUTINE(
            XSAPI_RESULT_INFO result,
            IntPtr privacyCheckResults,
            UInt64 privacyCheckResultsCount,
            IntPtr context);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT PrivacyCheckMultiplePermissionsWithMultipleTargetUsers(
            IntPtr xboxLiveContext,
            IntPtr permissionIds,
            UInt64 permissionIdsCount,
            IntPtr xboxUserIds,
            UInt64 xboxUserIdsCount,
            XSAPI_PRIVACY_CHECK_PERMISSION_WITH_MULTIPLE_TARGET_USERS_COMPLETION_ROUTINE completionRoutine,
            IntPtr completionRoutineContext,
            Int64 taskGroupId);
    }
}
