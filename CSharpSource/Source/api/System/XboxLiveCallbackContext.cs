// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Context to be stored and recovered during an asyncronous call to the XSAPI native dll
    /// </summary>
    /// <typeparam name="T">Type of any context to be stored (i.e. the service)</typeparam>
    /// <typeparam name="T2">Task completion source used to record that the managed C# API is complete</typeparam>
    internal class XsapiCallbackContext<T, T2> : IDisposable
    {
        public T Context;

        public TaskCompletionSource<T2> TaskCompletionSource;

        /// <summary>
        /// Pointers to WinRT object to which we need a reference to for the native method that need to be released
        /// when the callback is completed. Marshal.Release will be called for each pointer in the list when the context
        /// is disposed.
        /// </summary>
        public List<IntPtr> PointersToRelease { get; set; }

        /// <summary>
        /// Pointers to unmanaged memory that we allocate for a native function call. Marshal.Free will be called
        /// for each pointer in the list when the context is disposed.
        /// </summary>
        public List<IntPtr> PointersToFree = null;

        /// <summary>
        /// Handles to any managed objects that we have pinned/prevented garbage collection on during a native function
        /// call. GCHandle.Free will be called on each handle when the context is disposed.
        /// </summary>
        public List<GCHandle> GCHandlesToFree = null;

        public static XsapiCallbackContext<T, T2> CreateContext(T context, TaskCompletionSource<T2> tcs, out int key)
        {
            var xboxLiveCallbackContext = new XsapiCallbackContext<T, T2>(context, tcs);

            lock (contextsLock)
            {
                key = Interlocked.Increment(ref contextKey);
                contextsMap.Add(key, xboxLiveCallbackContext);
            }
            return xboxLiveCallbackContext;
        }

        public static int CreateContext(T context, TaskCompletionSource<T2> tcs)
        {
            int key;
            CreateContext(context, tcs, out key);
            return key;
        }

        public static bool TryRemove(int contextKey, out XsapiCallbackContext<T, T2> context)
        {
            context = null;

            lock (contextsLock)
            {
                IDisposable genericContext;
                if (contextsMap.TryGetValue(contextKey, out genericContext))
                {
                    context = genericContext as XsapiCallbackContext<T, T2>;
                    if (context != null)
                    {
                        contextsMap.Remove(contextKey);
                        return true;
                    }
                }
            }
            return false;
        }

        protected XsapiCallbackContext(T context, TaskCompletionSource<T2> tcs)
        {
            this.Context = context;
            this.TaskCompletionSource = tcs;
        }

        ~XsapiCallbackContext()
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
            if (GCHandlesToFree != null)
            {
                foreach (var handle in GCHandlesToFree)
                {
                    handle.Free();
                }
            }
            GC.SuppressFinalize(this);
        }

        protected static int contextKey = 0;
        protected static object contextsLock = new object();
        protected static Dictionary<Int32, IDisposable> contextsMap = new Dictionary<Int32, IDisposable>();
    }
}