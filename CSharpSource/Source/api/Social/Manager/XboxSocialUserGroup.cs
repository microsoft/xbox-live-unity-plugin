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
        
        public IEnumerator<XboxSocialUser> GetEnumerator()
        {
            return this.m_users.Values.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        // Used for mock services
        internal XboxSocialUserGroup(XboxLiveUser localUser, SocialUserGroupType groupType = SocialUserGroupType.Filter, PresenceFilter presence = PresenceFilter.Unknown, RelationshipFilter relationship = RelationshipFilter.Friends, Dictionary<string, XboxSocialUser> users = null, List<string> trackedUsers = null)
        {
            LocalUser = localUser;
            SocialUserGroupType = groupType;
            PresenceFilterOfGroup = presence;
            RelationshipFilterOfGroup = relationship;
            m_users = users == null ? new Dictionary<string, XboxSocialUser>() : users;
            m_trackedUsers = trackedUsers == null ? new List<string>() : trackedUsers;
        }

        public XboxSocialUserGroup()
        {
        }

        internal void UpdateGroup(Dictionary<string, XboxSocialUser> users)
        {
            m_users.Clear();
            foreach (string key in users.Keys)
            {
                m_users[key] = users[key];
            }
        }
    }
}