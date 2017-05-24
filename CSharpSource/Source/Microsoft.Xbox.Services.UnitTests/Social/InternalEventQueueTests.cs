// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.UnitTests.Social
{
    using global::System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Social.Manager;

    [TestClass]
    public class InternalEventQueueTests
    {
        [TestMethod]
        public void QueueEventObject()
        {
            InternalEventQueue queue = new InternalEventQueue();
            InternalSocialEvent internalEvent = new InternalSocialEvent(InternalSocialEventType.UsersAdded, new List<XboxSocialUser>());
            queue.Enqueue(internalEvent);

            InternalSocialEvent dequeuedEvent;
            Assert.IsTrue(queue.TryDequeue(out dequeuedEvent));

            Assert.AreEqual(internalEvent, dequeuedEvent);
        }
    }
}