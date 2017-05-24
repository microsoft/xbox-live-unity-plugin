// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Achievements
{
    public class Achievement
    {

        public bool IsRevoked
        {
            get;
            private set;
        }

        public string DeepLink
        {
            get;
            private set;
        }

        public TimeSpan EstimatedUnlockTime
        {
            get;
            private set;
        }

        public IList<AchievementReward> Rewards
        {
            get;
            private set;
        }

        public AchievementTimeWindow Available
        {
            get;
            private set;
        }

        public AchievementParticipationType ParticipationType
        {
            get;
            private set;
        }

        public AchievementType AchievementType
        {
            get;
            private set;
        }

        public string ProductId
        {
            get;
            private set;
        }

        public string LockedDescription
        {
            get;
            private set;
        }

        public string UnlockedDescription
        {
            get;
            private set;
        }

        public bool IsSecret
        {
            get;
            private set;
        }

        public IList<string> PlatformsAvailableOn
        {
            get;
            private set;
        }

        public IList<AchievementMediaAsset> MediaAssets
        {
            get;
            private set;
        }

        public AchievementProgression Progression
        {
            get;
            private set;
        }

        public AchievementProgressState ProgressState
        {
            get;
            private set;
        }

        public IList<AchievementTitleAssociation> TitleAssociations
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string ServiceConfigurationId
        {
            get;
            private set;
        }

        public string Id
        {
            get;
            private set;
        }

    }
}
