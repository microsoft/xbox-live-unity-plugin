using Microsoft.Xbox.Services.Social.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xbox.Services
{
    class SocialManagerImpl
    {
        private delegate void SocialManagerAddLocalUser(IntPtr user, SocialManagerExtraDetailLevel extraDetailLevel);
        public void AddLocalUserImpl(XboxLiveUser user, SocialManagerExtraDetailLevel extraDetailLevel)
        {
            XboxLive.Instance.Invoke<SocialManagerAddLocalUser>();
        }
    }
}
