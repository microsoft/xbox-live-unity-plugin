// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;

    internal class EventQueue : IEnumerable<SocialEvent>
    {
        private readonly Queue<SocialEvent> events = new Queue<SocialEvent>();

        public void Enqueue(InternalSocialEvent internalEvent, XboxLiveUser user)
        {
            if (internalEvent == null) throw new ArgumentNullException("internalEvent");
            if (internalEvent.Type == InternalSocialEventType.Unknown) throw new ArgumentException("Unable to handle Unknown event type.", "internalEvent");

            SocialEventType eventType;
            switch (internalEvent.Type)
            {
                case InternalSocialEventType.UsersAdded:
                    eventType = SocialEventType.UsersAddedToSocialGraph;
                    break;
                case InternalSocialEventType.UsersRemoved:
                    eventType = SocialEventType.UsersRemovedFromSocialGraph;
                    break;
                case InternalSocialEventType.PresenceChanged:
                case InternalSocialEventType.DevicePresenceChanged:
                case InternalSocialEventType.TitlePresenceChanged:
                    eventType = SocialEventType.PresenceChanged;
                    break;
                case InternalSocialEventType.ProfilesChanged:
                    eventType = SocialEventType.ProfilesChanged;
                    break;
                case InternalSocialEventType.SocialRelationshipsChanged:
                    eventType = SocialEventType.SocialRelationshipsChanged;
                    break;
                case InternalSocialEventType.Unknown:
                case InternalSocialEventType.UsersChanged:
                    // These events are not converted into public events.
                    return;
                default:
                    throw new ArgumentOutOfRangeException("internalEvent", internalEvent.Type, null);
            }

            SocialEvent evt = new SocialEvent(eventType, user, internalEvent.UserIdsAffected);
            this.events.Enqueue(evt);
        }

        public int Count
        {
            get
            {
                return this.events.Count;
            }
        }

        public IEnumerator<SocialEvent> GetEnumerator()
        {
            return this.events.GetEnumerator();
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}