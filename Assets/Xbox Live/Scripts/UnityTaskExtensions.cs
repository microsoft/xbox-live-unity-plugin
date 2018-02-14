// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Threading.Tasks;

using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public static class UnityTaskExtensions
    {
        public static TaskYieldInstruction AsCoroutine(this Task task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            return new TaskYieldInstruction(task);
        }

        public static TaskYieldInstruction<TResult> AsCoroutine<TResult>(this Task<TResult> task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            return new TaskYieldInstruction<TResult>(task);
        }
    }

    /// <summary>
    /// Creates a unity YieldInstruction which can be returned from a Coroutine 
    /// that completes once the associated task completes.
    /// </summary>
    public class TaskYieldInstruction : CustomYieldInstruction
    {
        private bool taskComplete;

        public TaskYieldInstruction(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            this.Task = task;

            if (task.IsCompleted)
            {
                this.taskComplete = true;
                return;
            }

            // If the task is not complete yet, we need to attach a
            // continuation to mark the task as complete.
            task.ContinueWith(t => this.taskComplete = true);
        }

        public Task Task { get; protected set; }

        public override bool keepWaiting
        {
            get
            {
                if (!this.taskComplete)
                {
                    return true;
                }

                // If the task has completed, but completes with an error
                // this will force the exception to be thrown so that the 
                // coroutine code will at least log it somewhere which 
                // should prevent stuff from getting lost.
                if (this.Task.Exception != null)
                {
                    throw this.Task.Exception;
                }

                return false;
            }
        }
    }

    public class TaskYieldInstruction<TResult> : TaskYieldInstruction
    {
        public TaskYieldInstruction(Task<TResult> task) : base(task)
        {
        }

        public new Task<TResult> Task
        {
            get
            {
                return base.Task as Task<TResult>;
            }

            protected set
            {
                base.Task = value;
            }
        }

        public TResult Result
        {
            get
            {
                return this.Task.Result;
            }
        }
    }
}