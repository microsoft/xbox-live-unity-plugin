// -----------------------------------------------------------------------
//  <copyright file="UnityTaskExtensions.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

using UnityEngine;

public static class UnityTaskExtensions
{
    public static TaskYieldInstruction AsCoroutine(this Task task)
    {
        return new TaskYieldInstruction(task);
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
        task.ContinueWith(t => this.taskComplete = true);
    }

    public override bool keepWaiting
    {
        get
        {
            return !this.taskComplete;
        }
    }
}