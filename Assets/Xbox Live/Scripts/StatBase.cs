// -----------------------------------------------------------------------
//  <copyright file="StatBase.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using UnityEngine;

[Serializable]
public abstract class StatBase : MonoBehaviour
{
    public string Name;

    public abstract string ValueString { get; }
}

[Serializable]
public abstract class StatBase<T> : StatBase
{
    public T Value;

    public override string ValueString
    {
        get
        {
            return this.Value.ToString();
        }
    }

    public abstract void SetValue(T value);

    void Awake()
    {
        // Set the initial stat value.
        this.SetValue(this.Value);
    }
}