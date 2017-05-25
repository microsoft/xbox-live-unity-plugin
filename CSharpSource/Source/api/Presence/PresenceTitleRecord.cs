// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services.Presence
{
    using global::System;

    public class PresenceTitleRecord
    {
        public uint TitleId { get; set; }

        public string TitleName { get; set; }

        public PresenceBroadcastRecord BroadcastRecord { get; set; }

        public PresenceTitleViewState TitleViewState { get; set; }

        public string Presence { get; set; }

        public bool IsTitleActive { get; set; }

        public DateTimeOffset LastModifiedDate { get; set; }
    }
}