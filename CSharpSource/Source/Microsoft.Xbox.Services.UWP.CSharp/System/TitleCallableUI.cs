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
        [StructLayout(LayoutKind.Sequential)]
        private struct CheckGamingPrivilegeResult
        {
            [MarshalAsAttribute(UnmanagedType.U1)]
            public bool hasPrivilege;

            public XboxLiveResult xblResult;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ShowProfileCardUICompletionRoutine(XboxLiveResult result, IntPtr completionRoutineContext);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CheckGamingPrivilegeCompletionRoutine(CheckGamingPrivilegeResult result, IntPtr completionRoutineContext);

        private delegate int TCUIShowProfileCardUI(IntPtr targetXboxUserId, ShowProfileCardUICompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);
        private delegate int TCUICheckGamingPrivilegeSilently(GamingPrivilege privilege, CheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);
        private delegate int TCUICheckGamingPrivilegeWithUI(GamingPrivilege privilege, IntPtr friendlyMessage, CheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext, Int64 taskGroupId);

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
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                var pTargetXboxUserId = Marshal.StringToHGlobalUni(targetXboxUserId);
                int contextKey = XboxLiveCallbackContext<TitleCallableUI, bool>.CreateContext(null, tcs, null, new List<IntPtr> { pTargetXboxUserId });
                
                XboxLive.Instance.Invoke<int, TCUIShowProfileCardUI>(
                    pTargetXboxUserId,
                    (ShowProfileCardUICompletionRoutine)ShowProfileCardUIComplete,
                    (IntPtr)contextKey,
                    XboxLive.DefaultTaskGroupId
                    );
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
                int contextKey = XboxLiveCallbackContext<TitleCallableUI, bool>.CreateContext(null, tcs);

                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeSilently>(
                    privilege,
                    (CheckGamingPrivilegeCompletionRoutine)CheckGamingPrivilegeSilentlyComplete,
                    (IntPtr)contextKey,
                    XboxLive.DefaultTaskGroupId
                    );
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
                var pFriendlyMessage = Marshal.StringToHGlobalUni(friendlyMessage);
                int contextKey = XboxLiveCallbackContext<TitleCallableUI, bool>.CreateContext(null, tcs, null, new List<IntPtr> { pFriendlyMessage });

                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeWithUI>(
                    privilege,
                    pFriendlyMessage,
                    (CheckGamingPrivilegeCompletionRoutine)CheckGamingPrivilegeWithUIComplete,
                    (IntPtr)contextKey,
                    XboxLive.DefaultTaskGroupId
                    );
            });

            return tcs.Task;
        }

        private static void ShowProfileCardUIComplete(XboxLiveResult result, IntPtr completionRoutineContext)
        {
            int contextKey = completionRoutineContext.ToInt32();

            XboxLiveCallbackContext<TitleCallableUI, bool> context;
            if (XboxLiveCallbackContext<TitleCallableUI, bool>.TryRemove(contextKey, out context))
            {
                context.TaskCompletionSource.SetResult(true);
                context.Dispose();
            }
        }

        private static void CheckGamingPrivilegeSilentlyComplete(CheckGamingPrivilegeResult result, IntPtr completionRoutineContext)
        {
            int contextKey = completionRoutineContext.ToInt32();

            XboxLiveCallbackContext<TitleCallableUI, bool> context;
            if (XboxLiveCallbackContext<TitleCallableUI, bool>.TryRemove(contextKey, out context))
            {
                context.TaskCompletionSource.SetResult(result.hasPrivilege);
                context.Dispose();
            }      
        }

        private static void CheckGamingPrivilegeWithUIComplete(CheckGamingPrivilegeResult result, IntPtr completionRoutineContext)
        {
            int contextKey = completionRoutineContext.ToInt32();

            XboxLiveCallbackContext<TitleCallableUI, bool> context;
            if (XboxLiveCallbackContext<TitleCallableUI, bool>.TryRemove(contextKey, out context))
            {
                context.TaskCompletionSource.SetResult(result.hasPrivilege);
                context.Dispose();
            }
        }
    }
}