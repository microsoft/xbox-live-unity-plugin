// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    interface ISocialManagerPresenceRecord
    {
        bool IsUserPlayingTitle(uint titleId);
    }
}
