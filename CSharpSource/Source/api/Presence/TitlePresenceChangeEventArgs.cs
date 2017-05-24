// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Presence
{
    using global::System;

    public class TitlePresenceChangeEventArgs : EventArgs
    {
        public TitlePresenceState TitleState { get; internal set; }

        public uint TitleId { get; internal set; }

        public string XboxUserId { get; internal set; }
    }
}