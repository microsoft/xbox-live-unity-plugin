// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services;

    public class TitleCallableUI
    {
        /// <summary>
        /// Shows UI displaying the profile card for a specified user.
        /// </summary>
        /// <param name="user">XboxLiveUser that identifies the user to show the UI on behalf of.</param>
        /// <param name="targetXboxUserId">The Xbox User ID to show information about.</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// </returns>
        public static Task ShowProfileCardUIAsync(XboxLiveUser user, string targetXboxUserId)
        {
            var tcs = new TaskCompletionSource<object>();

            Task.Run(() =>
            {
                var pTargetXboxUserId = MarshalingHelpers.StringToHGlobalUtf8(targetXboxUserId);

                int contextKey;
                var context = XsapiCallbackContext<object, object>.CreateContext(null, tcs, out contextKey);
                context.PointersToFree = new List<IntPtr> { pTargetXboxUserId };

                var result = TCUIShowProfileCardUI(pTargetXboxUserId, ShowProfileCardUIComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);
                if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(result));
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Checks if the current user has a specific privilege
        /// </summary>
        /// <param name="user">XboxLiveUser that identifies the user to show the UI on behalf of.</param>
        /// <param name="privilege">The privilege to check.</param>
        /// <returns>
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static bool CheckGamingPrivilegeSilently(XboxLiveUser user, GamingPrivilege privilege)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                int contextKey = XsapiCallbackContext<object, bool>.CreateContext(null, tcs);

                var result = TCUICheckGamingPrivilegeSilently(privilege, CheckGamingPrivilegeComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);
                if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(result));
                }
            });

            tcs.Task.Wait();
            return tcs.Task.Result;
        }

        /// <summary>
        /// Checks if the current user has a specific privilege and if it doesn't, it shows UI 
        /// </summary>
        /// <param name="user">XboxLiveUser that identifies the user to show the UI on behalf of.</param>
        /// <param name="privilege">The privilege to check.</param>
        /// <param name="friendlyMessage">Text to display in addition to the stock text about the privilege</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static Task<bool> CheckGamingPrivilegeWithUI(XboxLiveUser user, GamingPrivilege privilege, string friendlyMessage)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                var pFriendlyMessage = MarshalingHelpers.StringToHGlobalUtf8(friendlyMessage);

                int contextKey;
                var context = XsapiCallbackContext<object, bool>.CreateContext(null, tcs, out contextKey);
                context.PointersToFree = new List<IntPtr> { pFriendlyMessage };

                var result = TCUICheckGamingPrivilegeWithUI(privilege, pFriendlyMessage,
                    CheckGamingPrivilegeComplete, (IntPtr)contextKey, XboxLive.DefaultTaskGroupId);

                if (result != XSAPI_RESULT.XSAPI_RESULT_OK)
                {
                    tcs.SetException(new XboxException(result));
                }
            });

            return tcs.Task;
        }

        private static void ShowProfileCardUIComplete(XSAPI_RESULT_INFO result, IntPtr completionRoutineContext)
        {
            int contextKey = completionRoutineContext.ToInt32();

            XsapiCallbackContext<TitleCallableUI, bool> context;
            if (XsapiCallbackContext<TitleCallableUI, bool>.TryRemove(contextKey, out context))
            {
                context.TaskCompletionSource.SetResult(true);
                context.Dispose();
            }
        }

        private static void CheckGamingPrivilegeComplete(XSAPI_RESULT_INFO result, bool hasPrivilege, IntPtr completionRoutineContext)
        {
            int contextKey = completionRoutineContext.ToInt32();

            XsapiCallbackContext<TitleCallableUI, bool> context;
            if (XsapiCallbackContext<TitleCallableUI, bool>.TryRemove(contextKey, out context))
            {
                context.TaskCompletionSource.SetResult(hasPrivilege);
                context.Dispose();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TCUI_CHECK_GAMING_PRIVILEGE_RESULT
        {
            [MarshalAsAttribute(UnmanagedType.U1)]
            public bool hasPrivilege;

            public XSAPI_RESULT_INFO xblResult;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_SHOW_PROFILE_CARD_UI_COMPLETION_ROUTINE(XSAPI_RESULT_INFO result, IntPtr completionRoutineContext);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void XSAPI_TCUI_CHECK_GAMING_PRIVILEGE_RESULT(XSAPI_RESULT_INFO result, bool hasPrivilege, IntPtr completionRoutineContext);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TCUIShowProfileCardUI(
            IntPtr targetXboxUserId, 
            XSAPI_SHOW_PROFILE_CARD_UI_COMPLETION_ROUTINE completionRoutine, 
            IntPtr completionRoutineContext, 
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TCUICheckGamingPrivilegeSilently(
            GamingPrivilege privilege, 
            XSAPI_TCUI_CHECK_GAMING_PRIVILEGE_RESULT completionRoutine, 
            IntPtr completionRoutineContext, 
            Int64 taskGroupId);

        [DllImport(XboxLive.FlatCDllName)]
        private static extern XSAPI_RESULT TCUICheckGamingPrivilegeWithUI(
            GamingPrivilege privilege, 
            IntPtr friendlyMessage, 
            XSAPI_TCUI_CHECK_GAMING_PRIVILEGE_RESULT completionRoutine, 
            IntPtr completionRoutineContext, 
            Int64 taskGroupId);
    }
}