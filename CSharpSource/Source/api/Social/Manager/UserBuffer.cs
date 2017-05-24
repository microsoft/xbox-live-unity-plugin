// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    internal class UserBuffer
    {
        public UserBuffer()
        {
            this.EventQueue = new InternalEventQueue();
            this.SocialUserGraph = new Dictionary<ulong, XboxSocialUser>();
        }

        public UserBuffer(IEnumerable<XboxSocialUser> users)
        {
            this.EventQueue = new InternalEventQueue();
            this.SocialUserGraph = users.ToDictionary(u => u.XboxUserId);
        }

        // TODO: This needs to be synchronized as well
        public Dictionary<ulong, XboxSocialUser> SocialUserGraph { get; private set; }

        public InternalEventQueue EventQueue { get; private set; }

        public bool TryAdd(XboxSocialUser user)
        {
            lock (((global::System.Collections.ICollection)this.SocialUserGraph).SyncRoot)
            {
                XboxSocialUser existingUser;
                if (this.SocialUserGraph.TryGetValue(user.XboxUserId, out existingUser)) return false;

                this.SocialUserGraph[user.XboxUserId] = user;
                return true;
            }
        }

        public bool TryRemove(XboxSocialUser user)
        {
            lock (((global::System.Collections.ICollection)this.SocialUserGraph).SyncRoot)
            {
                if (this.SocialUserGraph.ContainsKey(user.XboxUserId))
                {
                    this.SocialUserGraph.Remove(user.XboxUserId);
                    return true;
                }

                return false;
            }
        }

        public void Enqueue(InternalSocialEvent internalEvent)
        {
            this.EventQueue.Enqueue(internalEvent);
        }

        public bool IsEmpty
        {
            get
            {
                return this.EventQueue.IsEmpty;
            }
        }
    }

    internal class UserBuffersHolder
    {
        public UserBuffersHolder()
        {
            this.Active = new UserBuffer();
            this.Inactive = new UserBuffer();
        }

        public UserBuffersHolder(IList<XboxSocialUser> users)
        {
            this.Active = new UserBuffer(users);
            this.Inactive = new UserBuffer(users);
        }

        public void SwapIfEmpty()
        {
            if (!this.Inactive.IsEmpty) return;

            UserBuffer swapBuffer = this.Active;
            this.Active = this.Inactive;
            this.Inactive = swapBuffer;
        }

        public UserBuffer Active { get; private set; }

        public UserBuffer Inactive { get; private set; }

        public void Initialize(IList<XboxSocialUser> users)
        {
            
        }

        public void AddEvent(InternalSocialEvent internalSocialEvent)
        {
            this.Active.Enqueue(internalSocialEvent);
        }

        public void AddUsers(List<XboxSocialUser> users)
        {
            foreach (XboxSocialUser user in users)
            {
                this.Inactive.TryAdd(user);
            }
        }

        public void RemoveUsers(List<XboxSocialUser> users)
        {
            foreach (XboxSocialUser user in users)
            {
                this.Inactive.TryRemove(user);
            }
        }

        public void RemoveUsers(List<ulong> users)
        {
            foreach (ulong userId in users)
            {
                this.Inactive.SocialUserGraph.Remove(userId);
            }
        }
    }
}