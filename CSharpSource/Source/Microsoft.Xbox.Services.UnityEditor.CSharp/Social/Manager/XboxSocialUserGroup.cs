// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;

    partial class XboxSocialUserGroup : IXboxSocialUserGroup
    {
        public IList<XboxSocialUser> GetUsersFromXboxUserIds(IList<string> xboxUserIds)
        {
            List<XboxSocialUser> users = new List<XboxSocialUser>();

            foreach (string xboxUserId in xboxUserIds)
            {
                if (m_users.ContainsKey(xboxUserId))
                {
                    users.Add(m_users[xboxUserId]);
                }
                else
                {
                    // todo handle error
                }
            }

            return users;
        }
    }
}
