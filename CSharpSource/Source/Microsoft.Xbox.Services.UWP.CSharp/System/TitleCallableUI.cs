// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services;

    public class TitleCallableUI
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct TCUICheckGamingPrivilegeResult
        {
            [MarshalAsAttribute(UnmanagedType.U1)]
            public bool hasPrivilege;

            public XboxLiveResult xblResult;
        }

        private delegate void TCUIShowProfileCardUICompletionRoutine(XboxLiveResult result, IntPtr completionRoutineContext);
        private delegate int TCUIShowProfileCardUI(IntPtr targetXboxUserId, TCUIShowProfileCardUICompletionRoutine completionRoutine, IntPtr completionRoutineContext);

        private delegate void TCUICheckGamingPrivilegeCompletionRoutine(TCUICheckGamingPrivilegeResult result, IntPtr completionRoutineContext);
        private delegate int TCUICheckGamingPrivilegeSilently(GamingPrivilege privilege, TCUICheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext);
        private delegate int TCUICheckGamingPrivilegeWithUI(GamingPrivilege privilege, IntPtr friendlyMessage, TCUICheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext);

        private static TaskCompletionSource<XboxLiveResult> showProfileCardUITCS;
        private static TaskCompletionSource<bool> checkPrivilegeSilentlyTCS;
        private static TaskCompletionSource<bool> checkPrivilegeWithUITCS;

        private static void ShowProfileCardUIComplete(XboxLiveResult result, IntPtr completionRoutineContext)
        {
            Debug.WriteLine("ShowProfileCardUIComplete");
            Debug.WriteLine("errorCode: " + result.errorCode);
            Debug.WriteLine("errorMessage: " + result.errorMessage);

            showProfileCardUITCS.SetResult(result);
        }

        /// <summary>
        /// Shows UI displaying the profile card for a specified user.
        /// </summary>
        /// <param name="user">XboxLiveUser that identifies the user to show the UI on behalf of.</param>
        /// <param name="targetXboxUserId">The Xbox User ID to show information about.</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// </returns>
        public static Task<XboxLiveResult> ShowProfileCardUIAsync(XboxLiveUser user, string targetXboxUserId)
        {
            showProfileCardUITCS = new TaskCompletionSource<XboxLiveResult>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUIShowProfileCardUI>(
                    Marshal.StringToHGlobalUni(targetXboxUserId),
                    (TCUIShowProfileCardUICompletionRoutine)ShowProfileCardUIComplete,
                    (IntPtr)3
                    );
            });

            return showProfileCardUITCS.Task;
        }

        private static void CheckPrivilegeSilentlyComplete(TCUICheckGamingPrivilegeResult result, IntPtr completionRoutineContext)
        {
            Debug.WriteLine("CheckPrivilegeSilentlyComplete");
            Debug.WriteLine("errorCode: " + result.xblResult.errorCode);
            Debug.WriteLine("errorMessage: " + result.xblResult.errorMessage);

            checkPrivilegeSilentlyTCS.SetResult(result.hasPrivilege);
        }

        /// <summary>
        /// Checks if the current user has a specific privilege
        /// </summary>
        /// /// /// <param name="user">XboxLiveUser that identifies the user to show the UI on behalf of.</param>
        /// <param name="privilege">The privilege to check.</param>
        /// <returns>
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static bool CheckPrivilegeSilently(XboxLiveUser user, GamingPrivilege privilege)
        {
            checkPrivilegeSilentlyTCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeSilently>(
                    privilege,
                    //null,
                    (TCUICheckGamingPrivilegeCompletionRoutine)CheckPrivilegeSilentlyComplete,
                    (IntPtr)3
                    );
            });

            checkPrivilegeSilentlyTCS.Task.Wait();
            return checkPrivilegeSilentlyTCS.Task.Result;
        }

        private static void CheckPrivilegeWithUIComplete(TCUICheckGamingPrivilegeResult result, IntPtr completionRoutineContext)
        {
            Debug.WriteLine("CheckPrivilegeWithUIComplete");
            Debug.WriteLine("errorCode: " + result.xblResult.errorCode);
            Debug.WriteLine("errorMessage: " + result.xblResult.errorMessage);

            checkPrivilegeWithUITCS.SetResult(result.hasPrivilege);
        }

        /// <summary>
        /// Checks if the current user has a specific privilege and if it doesn't, it shows UI 
        /// </summary>
        /// /// <param name="user">XboxLiveUser that identifies the user to show the UI on behalf of.</param>
        /// <param name="privilege">The privilege to check.</param>
        /// <param name="friendlyMessage">Text to display in addition to the stock text about the privilege</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static Task<bool> CheckPrivilegeWithUIAsync(XboxLiveUser user, GamingPrivilege privilege, string friendlyMessage)
        {
            checkPrivilegeWithUITCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeWithUI>(
                    privilege,
                    Marshal.StringToHGlobalUni(friendlyMessage),
                    (TCUICheckGamingPrivilegeCompletionRoutine)CheckPrivilegeWithUIComplete,
                    (IntPtr)3
                    );
            });

            return checkPrivilegeWithUITCS.Task;
        }
    }
}