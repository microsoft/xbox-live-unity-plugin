// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Privacy
{
    using global::System.Threading.Tasks;
    using global::System.Collections.Generic;

    internal interface IPrivacyService
    {
        /// <summary>
        /// Get the list of Xbox Live Ids the calling user should avoid during multiplayer matchmaking.
        /// </summary>
        /// <returns>A collection of XboxUserIds that correspond to the calling user's avoid list.</returns>
        Task<IList<string>> GetAvoidListAsync();
        
        /// <summary>
        /// Check a single permission with a single target user.
        /// </summary>
        /// <param name="permissionId">The ID of the permission to check.
        /// See Microsoft::Xbox::Services::Privacy::PermissionIdConstants for the latest options.</param>
        /// <param name="targetXboxUserId">The target user's xbox Live ID for validation</param>
        /// <returns>A <see cref="PermissionCheckResult"/> object containing the result against a single user.</returns>
        Task<PermissionCheckResult> CheckPermissionWithTargetUserAsync(
            string permissionId,
            string targetXboxUserId
            );

        /// <summary>
        /// Check multiple permissions with multiple target users.
        /// </summary>
        /// <param name="permissionIds">The collection of IDs of the permissions to check.
        /// See Microsoft::Xbox::Services::Privacy::PermissionIdConstants for the latest options.</param>
        /// <param name="targetXboxUserIds">The collection of target Xbox user IDs to check permissions against.</param>
        /// <returns>A collection of <see cref="PermissionCheckResult"/> objects containing results of the target xbox user ids.</returns>
        Task<List<MultiplePermissionsCheckResult>> CheckMultiplePermissionsWithMultipleTargetUsersAsync(
            IList<string> permissionIds,
            IList<string> targetXboxUserIds
            );

        /// <summary>
        /// Get the list of Xbox Live Ids that the calling user should not hear (mute) during multiplayer matchmaking.
        /// </summary>
        /// <returns>The collection of Xbox user IDs that represent the mute list for a user.</returns>
        Task<IList<string>> GetMuteListAsync();
    }
}
