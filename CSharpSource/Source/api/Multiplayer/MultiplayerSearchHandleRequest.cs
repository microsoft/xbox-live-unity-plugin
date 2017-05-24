// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerSearchHandleRequest
    {
        public MultiplayerSearchHandleRequest(MultiplayerSessionReference sessionReference) {
        }

        public Dictionary<string, string> StringsMetadata
        {
            get;
            set;
        }

        public Dictionary<string, double> NumbersMetadata
        {
            get;
            set;
        }

        public IList<string> Tags
        {
            get;
            set;
        }

        public MultiplayerSessionReference SessionReference
        {
            get;
            private set;
        }

    }
}
