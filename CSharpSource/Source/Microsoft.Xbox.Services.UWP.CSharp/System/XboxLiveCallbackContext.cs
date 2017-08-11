// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    internal class XboxLiveCallbackContext<T, T2> : IDisposable
    {
        public T Context;

        public TaskCompletionSource<T2> TaskCompletionSource;      

        public List<IntPtr> PointersToRelease;
        public List<IntPtr> PointersToFree;

        public static int CreateContext(T context, TaskCompletionSource<T2> taskCompletionSource, List<IntPtr> pointersToRelease = null, List<IntPtr> pointersToFree = null)
        {
            var xboxLiveCallbackContext = new XboxLiveCallbackContext<T, T2>
            {
                Context = context,
                TaskCompletionSource = taskCompletionSource,
                PointersToRelease = pointersToRelease,
                PointersToFree = pointersToFree
            };

            int contextKey = Interlocked.Increment(ref s_contextKey);
            s_contextsMap.TryAdd(contextKey, xboxLiveCallbackContext);

            return contextKey;
        }

        public static bool TryRemove(int contextKey, out XboxLiveCallbackContext<T, T2> context)
        {
            IDisposable genericContext;
            if (s_contextsMap.TryRemove(contextKey, out genericContext))
            {
                context = genericContext as XboxLiveCallbackContext<T, T2>;
                return context != null;
            }
            else
            {
                context = null;
                return false;
            }
        }

        private XboxLiveCallbackContext() { }

        ~XboxLiveCallbackContext()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (PointersToFree != null)
            {
                foreach (var pointer in PointersToFree)
                {
                    if (pointer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pointer);
                    }
                }
                PointersToFree = null;
            }
            if (PointersToRelease != null)
            {
                foreach (var pointer in PointersToRelease)
                {
                    Marshal.Release(pointer);
                }
                PointersToRelease = null;
            }
            GC.SuppressFinalize(this);
        }

        protected static int s_contextKey = 0;
        protected static ConcurrentDictionary<Int32, IDisposable> s_contextsMap = new ConcurrentDictionary<Int32, IDisposable>();
    }
}