// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.Presence;

    internal class InternalSocialEvent
    {
        private readonly TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

        private InternalSocialEvent(InternalSocialEventType type)
        {
            this.Type = type;
        }

        private InternalSocialEvent() : this(InternalSocialEventType.Unknown)
        {
        }

        public InternalSocialEvent(InternalSocialEventType eventType, IList<XboxSocialUser> usersAffected) : this(eventType)
        {
            this.UsersAffected = usersAffected;
            IList<ulong> userList = new List<ulong>();
            foreach (XboxSocialUser user in usersAffected)
            {
                userList.Add(user.XboxUserId);
            }

            this.UserIdsAffected = userList;
        }

        public InternalSocialEvent(InternalSocialEventType eventType, DevicePresenceChangeEventArgs devicePresenceArgs) : this(eventType)
        {
            this.DevicePresenceArgs = devicePresenceArgs;
        }

        public InternalSocialEvent(InternalSocialEventType eventType, TitlePresenceChangeEventArgs titlePresenceArgs) : this(eventType)
        {
            this.TitlePresenceArgs = titlePresenceArgs;
        }

        public InternalSocialEvent(InternalSocialEventType eventType, object errorInfo, IList<ulong> userList) : this(eventType)
        {
            this.UserIdsAffected = userList;
        }

        public InternalSocialEvent(InternalSocialEventType eventType, IList<ulong> userAddList) : this(eventType, userAddList, null)
        {
            this.UserIdsAffected = userAddList;
        }

        internal InternalSocialEventType Type { get; private set; }
        internal IList<XboxSocialUser> UsersAffected { get; private set; }
        internal IList<ulong> UserIdsAffected { get; private set; }
        internal DevicePresenceChangeEventArgs DevicePresenceArgs { get; private set; }
        internal TitlePresenceChangeEventArgs TitlePresenceArgs { get; private set; }

        /// <summary>
        /// A task which completes when the event is processed.
        /// </summary>
        internal Task Task
        {
            get
            {
                return this.taskCompletionSource.Task;
            }
        }

        internal void ProcessEvent()
        {
            // Mark the task as complete.
            this.taskCompletionSource.SetResult(null);
        }
    }
}