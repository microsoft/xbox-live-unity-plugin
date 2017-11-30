// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Statistics.Manager;

namespace XboxLivePrefab
{
    public class StatEventArgs : EventArgs
    {
        public StatEventArgs(StatisticEvent statEvent)
        {
            this.EventData = statEvent;
        }

        public StatisticEvent EventData { get; private set; }
    }
}