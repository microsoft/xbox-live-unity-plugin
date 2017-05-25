// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.UserStatistics
{
    public class UserStatisticsService
    {

        //public event EventHandler<StatisticChangeEventArgs> StatisticChanged;


        public Task<UserStatisticsResult> GetSingleUserStatisticsAsync(string xboxUserId, string serviceConfigurationId, string[] statisticNames)
        {
            throw new NotImplementedException();
        }

        public Task<UserStatisticsResult> GetSingleUserStatisticsAsync(string xboxUserId, string serviceConfigurationId, string statisticName)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<UserStatisticsResult>> GetMultipleUserStatisticsAsync(string[] xboxUserIds, string serviceConfigurationId, string[] statisticNames)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<UserStatisticsResult>> GetMultipleUserStatisticsForMultipleServiceConfigurationsAsync(string[] xboxUserIds, Microsoft.Xbox.Services.UserStatistics.RequestedStatistics[] requestedServiceConfigurationStatisticsCollection)
        {
            throw new NotImplementedException();
        }

        public StatisticChangeSubscription SubscribeToStatisticChange(string xboxUserId, string serviceConfigurationId, string statisticName)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromStatisticChange(StatisticChangeSubscription subscription)
        {
            throw new NotImplementedException();
        }

    }
}
