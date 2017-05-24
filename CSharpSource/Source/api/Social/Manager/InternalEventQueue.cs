// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    internal class InternalEventQueue
    {
        private readonly Queue<InternalSocialEvent> eventQueue;
        private const int MaxUsersAffectedPerEvent = 10;

        public InternalEventQueue()
        {
            this.eventQueue = new Queue<InternalSocialEvent>();
        }

        public Task Enqueue(InternalSocialEventType socialEventType, List<XboxSocialUser> userList)
        {
            var numGroups = userList.Count / MaxUsersAffectedPerEvent + 1;

            List<Task> eventTasks = new List<Task>();
            for (int i = 0; i < numGroups; ++i)
            {
                int numUsers = i == numGroups - 1 ? (userList.Count % MaxUsersAffectedPerEvent) : MaxUsersAffectedPerEvent;
                var evt = new InternalSocialEvent(socialEventType, userList.GetRange(i * MaxUsersAffectedPerEvent, numUsers));
                this.Enqueue(evt);
                eventTasks.Add(evt.Task);
            }

            return Task.WhenAll(eventTasks);
        }

        public void Enqueue(InternalSocialEvent socialEvent)
        {
            lock (((global::System.Collections.ICollection)this.eventQueue).SyncRoot)
            {
                this.eventQueue.Enqueue(socialEvent);
            }
        }

        public bool TryDequeue(out InternalSocialEvent internalEvent)
        {
            lock (((global::System.Collections.ICollection)this.eventQueue).SyncRoot)
            {
                internalEvent = this.eventQueue.Count > 0 ? this.eventQueue.Dequeue() : null;
                return internalEvent != null;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.eventQueue.Count == 0;
            }
        }
    }
}