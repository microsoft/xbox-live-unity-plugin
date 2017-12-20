// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    public class ProfileService
    {
        internal ProfileService()
        {
        }

        public Task<XboxUserProfile> GetUserProfileAsync(string xboxUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<XboxUserProfile>> GetUserProfilesAsync(IList<string> xboxUserIds)
        {
            throw new NotImplementedException();
        }

        public Task<IList<XboxUserProfile>> GetUserProfilesForSocialGroupAsync(string socialGroup)
        {
            throw new NotImplementedException();
        }
    }
}