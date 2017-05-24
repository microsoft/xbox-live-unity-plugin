// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Social
{
    public class ReputationFeedbackItem
    {
        public ReputationFeedbackItem(string xboxUserId, ReputationFeedbackType reputationFeedbackType, Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference sessionReference, string reasonMessage, string evidenceResourceId) {
        }
        public ReputationFeedbackItem() {
        }

        public string EvidenceResourceId
        {
            get;
            private set;
        }

        public string ReasonMessage
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionReference SessionReference
        {
            get;
            private set;
        }

        public ReputationFeedbackType FeedbackType
        {
            get;
            private set;
        }

        public string XboxUserId
        {
            get;
            private set;
        }

    }
}
