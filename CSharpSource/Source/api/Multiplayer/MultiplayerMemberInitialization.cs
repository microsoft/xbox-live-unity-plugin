// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerMemberInitialization
    {

        public uint MembersNeededToStart
        {
            get;
            private set;
        }

        public bool ExternalEvaluation
        {
            get;
            private set;
        }

        public TimeSpan EvaluationTimeout
        {
            get;
            private set;
        }

        public TimeSpan MeasurementTimeout
        {
            get;
            private set;
        }

        public TimeSpan JoinTimeout
        {
            get;
            private set;
        }

        public bool MemberInitializationSet
        {
            get;
            private set;
        }

    }
}
