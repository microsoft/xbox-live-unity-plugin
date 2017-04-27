// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services.CSharp;
using Microsoft.Xbox.Services.CSharp.Stats.Manager;

namespace XboxLivePrefab
{
    public class StatEventArgs : EventArgs
    {
        public StatEventArgs(StatEvent statEvent)
        {
            this.EventData = statEvent;
        }

        public StatEvent EventData { get; private set; }
    }
}