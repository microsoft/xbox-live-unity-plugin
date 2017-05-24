// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Privacy
{
    using Newtonsoft.Json;

    public class PermissionCheckResult
    {
        [JsonProperty("reasons")]
        public IList<PermissionReason> Reasons { get; set; }

        [JsonProperty("permissionRequested")]
        public string PermissionRequested { get; set; }

        [JsonProperty("isAllowed")]
        public bool IsAllowed { get; set; }

    }
}
