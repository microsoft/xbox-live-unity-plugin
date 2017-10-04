
// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Statistics.Manager
{
    public partial class StatisticManager
    {
        private static readonly object instanceLock = new object();
        private static IStatisticManager instance;
        
        internal static IStatisticManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = XboxLive.UseMockServices ? new MockStatisticManager() : (IStatisticManager)new StatisticManager();
                        }
                    }
                }
                return instance;
            }
        }
    }
}