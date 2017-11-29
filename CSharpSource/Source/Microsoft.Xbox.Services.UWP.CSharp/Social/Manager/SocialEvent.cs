// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    public partial class SocialEvent
    {
        internal SocialEvent(IntPtr socialEventPtr, IList<XboxSocialUserGroup> groups)
        {
            SOCIAL_EVENT cSocialEvent = Marshal.PtrToStructure<SOCIAL_EVENT>(socialEventPtr);
            EventType = cSocialEvent.EventType;

            User = new XboxLiveUser(cSocialEvent.User);

            try
            {
                SocialManager.SOCIAL_USER_GROUP_LOADED_ARGS cArgs = Marshal.PtrToStructure<SocialManager.SOCIAL_USER_GROUP_LOADED_ARGS>(cSocialEvent.EventArgs);

                foreach (XboxSocialUserGroup group in groups)
                {
                    if (cArgs.SocialUserGroup == group.GetPtr())
                    {
                        EventArgs = new SocialUserGroupLoadedEventArgs(group);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // Event args weren't SocialUserGroupLoadedArgs
            }

            List<string> usersAffected = new List<string>();
            if (cSocialEvent.NumOfUsersAffected > 0)
            {
                IntPtr[] cUsersAffected = new IntPtr[cSocialEvent.NumOfUsersAffected];
                Marshal.Copy(cSocialEvent.UsersAffected, cUsersAffected, 0, cSocialEvent.NumOfUsersAffected);
                foreach (IntPtr cXuidPtr in cUsersAffected)
                {
                    SocialManager.XBOX_USER_ID_CONTAINER cXuid = Marshal.PtrToStructure<SocialManager.XBOX_USER_ID_CONTAINER>(cXuidPtr);
                    string xuid = MarshalingHelpers.Utf8ToString(cXuid.XboxUserId);
                    usersAffected.Add(xuid);
                }
            }
            UsersAffected = usersAffected.AsReadOnly();

            ErrorCode = cSocialEvent.ErrorCode;
            ErrorMessge = MarshalingHelpers.Utf8ToString(cSocialEvent.ErrorMessage);
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct SOCIAL_EVENT
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr User;

            [MarshalAs(UnmanagedType.U4)]
            public SocialEventType EventType;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr UsersAffected;

            [MarshalAs(UnmanagedType.I4)]
            public int NumOfUsersAffected;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr EventArgs;

            [MarshalAs(UnmanagedType.I4)]
            public int ErrorCode;

            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr ErrorMessage;
        }
    }
}
