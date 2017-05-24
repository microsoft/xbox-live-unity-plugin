// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.System
{
    public class TitleCallableUI
    {

        public static Task<global::System.Collections.ObjectModel.ReadOnlyCollection<string>> ShowPlayerPickerUI(string promptDisplayText, string[] xboxUserIds, string[] preselectedXboxUserIds, uint minSelectionCount, uint maxSelectionCount)
        {
            throw new NotImplementedException();
        }

        public static Task ShowGameInviteUIAsync(Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference sessionReference, string contextStringId)
        {
            throw new NotImplementedException();
        }

        public static Task ShowProfileCardUIAsync(string targetXboxUserId)
        {
            throw new NotImplementedException();
        }

        public static Task ShowChangeFriendRelationshipUIAsync(string targetXboxUserId)
        {
            throw new NotImplementedException();
        }

        public static Task ShowTitleAchievementsUIAsync(uint titleId)
        {
            throw new NotImplementedException();
        }

        public static bool CheckGamingPrivilegeSilently(GamingPrivilege privilege)
        {
            throw new NotImplementedException();
        }

        public static Task<bool> CheckGamingPrivilegeWithUI(GamingPrivilege privilege, string friendlyMessage)
        {
            throw new NotImplementedException();
        }

        public static Task<global::System.Collections.ObjectModel.ReadOnlyCollection<string>> ShowPlayerPickerUIForUser(string promptDisplayText, string[] xboxUserIds, string[] preselectedXboxUserIds, uint minSelectionCount, uint maxSelectionCount, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public static Task ShowGameInviteUIForUserAsync(Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference sessionReference, string contextStringId, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public static Task ShowProfileCardUIForUserAsync(string targetXboxUserId, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public static Task ShowChangeFriendRelationshipUIForUserAsync(string targetXboxUserId, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public static Task ShowTitleAchievementsUIForUserAsync(uint titleId, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public static bool CheckGamingPrivilegeSilentlyForUser(GamingPrivilege privilege, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

        public static Task<bool> CheckGamingPrivilegeWithUIForUser(GamingPrivilege privilege, string friendlyMessage, XboxLiveUser user)
        {
            throw new NotImplementedException();
        }

    }
}
