// -----------------------------------------------------------------------
//  <copyright file="UnityTaskExtensions.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

using UnityEngine;

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
        this.Task = task;
        task.ContinueWith(t => this.taskComplete = true);
    }

    public Task Task { get; protected set; }

    public override bool keepWaiting
    {
        get
        {
            return !this.taskComplete;
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