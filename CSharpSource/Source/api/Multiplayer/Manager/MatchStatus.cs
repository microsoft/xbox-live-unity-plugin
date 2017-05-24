// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer.Manager
{
    public enum MatchStatus : int
    {
        None = 0,
        SubmittingMatchTicket = 1,
        Searching = 2,
        Found = 3,
        Joining = 4,
        WaitingForRemoteClientsToJoin = 5,
        Measuring = 6,
        UploadingQosMeasurements = 7,
        WaitingForRemoteClientsToUploadQos = 8,
        Evaluating = 9,
        Completed = 10,
        Resubmitting = 11,
        Expired = 12,
        Canceling = 13,
        Canceled = 14,
        Failed = 15,
    }

}
