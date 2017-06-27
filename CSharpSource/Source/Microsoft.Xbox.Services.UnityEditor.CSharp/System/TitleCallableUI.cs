// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
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
            return Task.FromResult(true);
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
            return true;
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
            return Task.FromResult(true);
        }
    }
}