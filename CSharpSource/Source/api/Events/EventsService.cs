// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Events
{
    public class EventsService
    {

        public void WriteInGameEvent(string eventName, IDictionary<string, Object> dimensions, IDictionary<string, Object> measurements)
        {
            throw new NotImplementedException();
        }

        public void WriteInGameEvent(string eventName)
        {
            throw new NotImplementedException();
        }

    }
}
