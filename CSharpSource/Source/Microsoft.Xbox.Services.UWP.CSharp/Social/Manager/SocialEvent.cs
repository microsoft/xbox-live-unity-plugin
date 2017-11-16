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
            SocialEvent_c cSocialEvent = Marshal.PtrToStructure<SocialEvent_c>(socialEventPtr);
            EventType = cSocialEvent.EventType;

            User = new XboxLiveUser(cSocialEvent.User);

            try
            {
                SocialManager.SocialUserGroupLoadedArgs_c cArgs = Marshal.PtrToStructure<SocialManager.SocialUserGroupLoadedArgs_c>(cSocialEvent.EventArgs);

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
                    SocialManager.XboxUserIdContainer_c cXuid = Marshal.PtrToStructure<SocialManager.XboxUserIdContainer_c>(cXuidPtr);
                    string xuid = MarshalingHelpers.Utf8ToString(cXuid.XboxUserId);
                    usersAffected.Add(xuid);
                }
            }
            UsersAffected = usersAffected.AsReadOnly();

            ErrorCode = cSocialEvent.ErrorCode;
            ErrorMessge = MarshalingHelpers.Utf8ToString(cSocialEvent.ErrorMessage);
        }
        
        [StructLayout(LayoutKind.Sequential)]
        internal struct SocialEvent_c
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
