// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    public partial class XboxSocialUserGroup : IEnumerable<XboxSocialUser>
    {
        // Properties
        private readonly Dictionary<string, XboxSocialUser> m_users = new Dictionary<string, XboxSocialUser>();
        private List<string> m_trackedUsers = new List<string>();
        
        public XboxLiveUser LocalUser { get; private set; }

        public SocialUserGroupType SocialUserGroupType { get; private set; }

        public PresenceFilter PresenceFilterOfGroup { get; private set; }

        public RelationshipFilter RelationshipFilterOfGroup { get; private set; }
        
        public int Count
        {
            get
            {
                return this.m_users.Count;
            }
        }

        public IList<XboxSocialUser> Users
        {
            get
            {
                return this.m_users.Values.ToList();
            }
        }

        public IList<string> UsersTrackedBySocialUserGroup
        {
            get
            {
                return this.m_trackedUsers;
            }
        }

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

        public IEnumerator<XboxSocialUser> GetEnumerator()
        {
            return this.m_users.Values.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}