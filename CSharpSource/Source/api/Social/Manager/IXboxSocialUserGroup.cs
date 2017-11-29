// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System.Collections.Generic;

    interface IXboxSocialUserGroup
    {
        IList<XboxSocialUser> GetUsersFromXboxUserIds(IList<string> xboxUserIds);
    }
}
