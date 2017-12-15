// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Threading.Tasks;

    public class StringService
    {
        public Task<VerifyStringResult> VerifyStringAsync(string stringToVerify)
        {
            throw new NotImplementedException();
        }

        public Task<global::System.Collections.ObjectModel.ReadOnlyCollection<VerifyStringResult>> VerifyStringsAsync(string[] stringsToVerify)
        {
            throw new NotImplementedException();
        }

    }
}
