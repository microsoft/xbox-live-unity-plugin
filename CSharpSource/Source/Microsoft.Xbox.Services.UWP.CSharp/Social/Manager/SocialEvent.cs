
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;

    public partial class SocialEvent
    {
        internal SocialEvent(IntPtr socialEventPtr, IList<XboxSocialUserGroup> groups)
        {
            SocialManager.SocialEvent_c cSocialEvent = Marshal.PtrToStructure<SocialManager.SocialEvent_c>(socialEventPtr);
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
                    usersAffected.Add(cXuid.XboxUserId);
                }
            }
            UsersAffected = usersAffected.AsReadOnly();

            ErrorCode = cSocialEvent.ErrorCode;
            ErrorMessge = cSocialEvent.ErrorMessage;
        }
    }
}
