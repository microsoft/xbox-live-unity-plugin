// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.UnitTests.Stats
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.Statistics.Manager;

    [TestClass]
    public class StatsManagerTests : TestBase
    {
        [TestMethod]
        public void GetInstance()
        {
            IStatisticManager sm = XboxLive.Instance.StatsManager;
            Assert.IsNotNull(sm);
        }
    }
}
