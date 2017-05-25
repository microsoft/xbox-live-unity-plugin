// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.UnitTests
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Shared;

    [TestClass]
    public class CallBufferTimerTests
    {
        [TestMethod]
        public async Task BasicCallback()
        {
            bool completed = false;

            CallBufferTimer<object> timer = new CallBufferTimer<object>(TimeSpan.FromSeconds(1));
            timer.Completed += (sender, o) => { completed = true; };

            var timerTask = timer.Fire(new List<object> { new object() });

            var result = await Task.WhenAny(timerTask, Task.Delay(TimeSpan.FromSeconds(2)));
            if (result != timerTask)
            {
                // This means the delay task completed.
                Assert.Fail("Timer was never called.");
            }

            Assert.IsTrue(completed);
        }

        [TestMethod]
        public async Task ThrottledCallback()
        {
            int completedCount = 0;
            CallBufferTimer<int> timer = new CallBufferTimer<int>(TimeSpan.FromSeconds(1));
            timer.Completed += (sender, o) =>
            {
                string batchedElements = string.Join(", ", o.Elements.Select(e => e.ToString()));
                Debug.WriteLine($"Batched request being made at {DateTime.UtcNow:h:mm:ss.fff} with elements {batchedElements}");

                Interlocked.Increment(ref completedCount);
            };

            List<Task> timerTasks = Enumerable.Range(1, 10).Select(i => timer.Fire(new[] { i }.ToList())).ToList();

            Task delayTask = Task.Delay(TimeSpan.FromSeconds(2));
            timerTasks.Add(delayTask);

            var result = await Task.WhenAny(timerTasks);
            if (result == delayTask)
            {
                // This means the delay task completed.
                Assert.Fail("Timer was never called.");
            }

            Assert.AreEqual(1, completedCount);

            await Task.WhenAll(timerTasks);
        }

        /// <summary>
        /// Verifies that 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task QueuedThrottledCallback()
        {
            int completedCount = 0;
            CallBufferTimer<int> timer = new CallBufferTimer<int>(TimeSpan.FromMilliseconds(250));
            timer.Completed += (sender, o) =>
            {
                Thread.Sleep(100);

                string batchedElements = string.Join(", ", o.Elements.Select(e => e.ToString()));
                Debug.WriteLine($"Batched request being made at {DateTime.UtcNow:h:mm:ss.fff} with elements {batchedElements}");

                Interlocked.Increment(ref completedCount);
            };

            // Create a first batch of 10.  We don't care when these finish, so let the tasks go.
            List<Task> batch1Tasks = Enumerable.Range(1, 5).Select(i => timer.Fire(new[] { i }.ToList())).ToList();

            // Make sure the first batch has started.
            await Task.Delay(251);

            // Start a second batch after the first one is running.
            List<Task> timerTasks = Enumerable.Range(6, 5).Select(i => timer.Fire(new[] { i }.ToList())).ToList();

            Task delayTask = Task.Delay(TimeSpan.FromSeconds(2));
            timerTasks.Add(delayTask);

            var result = await Task.WhenAny(timerTasks);
            if (result == delayTask)
            {
                // This means the delay task completed.
                Assert.Fail("Second batch was never triggered.");
            }

            Debug.WriteLine($"Completed count: {completedCount}");
            Assert.AreEqual(completedCount, 2, "completedCount == 2");
        }
        
        /// <summary>
        /// Verifies that a call to fire will be immediately (or nearly immediately) executed if
        /// no call has been made in the past "period" amount of time.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ExecuteImmediatelyIfPastPeriod()
        {
            int completedCount = 0;
            CallBufferTimer<int> timer = new CallBufferTimer<int>(TimeSpan.FromMilliseconds(50));
            timer.Completed += (sender, o) =>
            {
                string batchedElements = string.Join(", ", o.Elements.Select(e => e.ToString()));
                Debug.WriteLine($"Batched request being made at {DateTime.UtcNow:h:mm:ss.fff} with elements [{batchedElements}]");

                Interlocked.Increment(ref completedCount);
            };

            var timerTask = timer.Fire(new List<int> { 1 });

            await Task.Yield();

            Task delayTask = Task.Delay(TimeSpan.FromSeconds(2));

            var result = await Task.WhenAny(timerTask, delayTask);
            if (result == delayTask)
            {
                // This means the delay task completed.
                Assert.Fail("Second batch was never triggered.");
            }

            Assert.IsTrue(timerTask.IsCompleted);
        }

        /// <summary>
        /// Verifies that if you add an element to the buffer while an existing call is queued
        /// it get's executed with that batch as opposed to the next.
        /// </summary>
        [TestMethod]
        public async Task AddWhileQueued()
        {
            int completedCount = 0;
            CallBufferTimer<int> timer = new CallBufferTimer<int>(TimeSpan.FromMilliseconds(1000));
            timer.Completed += (sender, o) =>
            {
                string batchedElements = string.Join(", ", o.Elements.Select(e => e.ToString()));
                Debug.WriteLine($"Batched request being made at {DateTime.UtcNow:h:mm:ss.fff} with elements [{batchedElements}]");

                Interlocked.Increment(ref completedCount);
            };

            Debug.WriteLine($"Now: {DateTime.UtcNow:O}");

            // Execute and wait for a single call
            await timer.Fire(new List<int> { 1 });

            // Fire another event which will queue up a new timer.
            var timerTask1 = timer.Fire(new List<int> { 2 });

            await Task.Delay(200);

            var timerTask2 = timer.Fire(new List<int> { 3 });

            Assert.AreEqual(timerTask1, timerTask2);

            await timerTask2;
        }
    }
}