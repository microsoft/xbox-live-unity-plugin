// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.UnitTests.TitleStorage
{
    using global::System;
    using global::System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xbox.Services.TitleStorage;

    [TestClass]
    public class TitleStorageTests: TestBase
    {
        private TitleStorageService titleStorageService;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            this.titleStorageService = new TitleStorageService(IntPtr.Zero);
        }
    }
}
