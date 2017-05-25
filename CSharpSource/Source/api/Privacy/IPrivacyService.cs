// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Privacy
{
    using global::System.Threading.Tasks;
    using global::System.Collections.Generic;

    internal interface IPrivacyService
    {
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
    }
}
