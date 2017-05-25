// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Shared
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    internal class CallBufferEventArgs<T> : EventArgs
    {
        public CallBufferEventArgs(List<T> elements)
        {
            this.Elements = elements;
        }

        public List<T> Elements { get; private set; }
    }

    /// <summary>
    /// A helper class to throttle calls and buffer call data.  One or more calls made to <see cref="Fire"/> will be
    /// batched together and passed as a single 
    /// </summary>
    internal class CallBufferTimer<T>
    {
        private readonly TimeSpan period;
        private DateTime previousTime;

        private readonly object elementBufferLock = new object();
        private readonly HashSet<T> elementBuffer;
        private TaskCompletionSource<bool> currentCompletionSource = new TaskCompletionSource<bool>();

        /// <summary>
        /// A lock around the current call to prevent multiple callers
        /// </summary>
        private readonly object callLock = new object();

        /// <summary>
        /// A task that represents the current call in progress.
        /// </summary>
        private Task inProgressTask = Task.FromResult(true);

        public event EventHandler<CallBufferEventArgs<T>> Completed;

        /// <summary>
        /// Create a <see cref="CallBufferTimer{T}"/>
        /// </summary>
        /// <param name="period">The minimum duration between triggers of the <see cref="Completed"/> event.</param>
        /// <param name="comparer">If not null, overrides the comparer used to compare elements in the buffer.</param>
        public CallBufferTimer(TimeSpan period, IEqualityComparer<T> comparer = null)
        {
            this.period = period;
            this.elementBuffer = new HashSet<T>(comparer);
        }

        public Task Fire(IList<T> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            if (elements.Count == 0)
            {
                return Task.FromResult(true);
            }

            TaskCompletionSource<bool> tcs;
            lock (this.elementBufferLock)
            {
                foreach (T user in elements)
                {
                    this.elementBuffer.Add(user);
                }
                // Grab the TCS associated with this buffer.  This will complete
                // when all the elements in this buffer have been sent.
                tcs = this.currentCompletionSource;
            }

            this.FireHelper();

            return tcs.Task;
        }

        private void FireHelper()
        {
            // If there's a call in progress, it will queue up a new 
            // task once it's done so we can just return.
            if (!this.inProgressTask.IsCompleted) return;

            TaskCompletionSource<bool> inProgressCompletionSource;

            // Prevent multiple people from initiating calls at the same time.
            lock (this.callLock)
            {
                if (!this.inProgressTask.IsCompleted) return;

                // Grab the current completion source, and mark it as in-progress by
                // setting it's task as the current in progress task.  Note that we 
                // are not swapping out the completion source yet, which means that
                // people can continue to queue elements into this buffer until we 
                // actually execute this call.
                inProgressCompletionSource = this.currentCompletionSource;
                this.inProgressTask = inProgressCompletionSource.Task;
            }

            // Determine if we need to delay at all.
            TimeSpan callDelay = this.period - (DateTime.Now - this.previousTime);
            if (callDelay < TimeSpan.Zero) callDelay = TimeSpan.Zero;

            // Yes, we will 'delay' for zero time in some cases, but without await
            // it would be much uglier to handle both cases explicitly.
            Task.Delay(callDelay).ContinueWith(continuationAction =>
            {
                if (continuationAction.IsFaulted)
                {
                    inProgressCompletionSource.SetException(continuationAction.Exception);
                    return;
                }

                // Copy the current buffer and TCS out and replace them with new ones.
                // Anyone who attempts to add stuff to the buffer after this point will
                // get a new TCS and as a result will wait until the next call occurs.
                List<T> elements;
                lock (this.elementBufferLock)
                {
                    inProgressCompletionSource = this.currentCompletionSource;
                    this.currentCompletionSource = new TaskCompletionSource<bool>();

                    elements = this.elementBuffer.ToList();
                    this.elementBuffer.Clear();
                }

                this.inProgressTask = inProgressCompletionSource.Task;

                if (elements.Count == 0)
                {
                    // No elements to request so complete immediately
                    inProgressCompletionSource.SetResult(true);
                    return;
                }

                try
                {
                    this.previousTime = DateTime.Now;

                    this.OnCompleted(new CallBufferEventArgs<T>(elements));
                    inProgressCompletionSource.SetResult(true);

                    if (this.elementBuffer.Count > 0)
                    {
                        this.FireHelper();
                    }
                }
                catch (Exception e)
                {
                    inProgressCompletionSource.SetException(e);
                }
            });
        }

        protected virtual void OnCompleted(CallBufferEventArgs<T> e)
        {
            var handler = this.Completed;
            if (handler != null) handler(this, e);
        }
    }
}