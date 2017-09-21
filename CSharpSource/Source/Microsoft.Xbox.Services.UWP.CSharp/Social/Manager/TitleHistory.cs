// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Runtime.InteropServices;
    public partial class TitleHistory
    {
        internal TitleHistory(IntPtr titleHistoryPtr)
        {
            SocialManager.TitleHistory_c cTitleHistory = Marshal.PtrToStructure<SocialManager.TitleHistory_c>(titleHistoryPtr);
            HasUserPlayed = Convert.ToBoolean(cTitleHistory.UserHasPlayed);

            // todo test
            LastTimeUserPlayed = DateTimeOffset.FromUnixTimeSeconds(cTitleHistory.LastTimeUserPlayed);
        }
    }
}
