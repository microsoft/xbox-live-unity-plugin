// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
namespace Microsoft.Xbox.Services
{
    using global::System;
    using global::System.Collections.Generic;

    public class HttpRetryAfterManager
    {
        private static readonly object instanceLock = new object();
        private static HttpRetryAfterManager instance;
        private readonly Dictionary<XboxLiveAPIName, HttpRetryAfterApiState> apiStateMap = new Dictionary<XboxLiveAPIName, HttpRetryAfterApiState>();

        public static HttpRetryAfterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new HttpRetryAfterManager();
                        }
                    }
                }
                return instance;
            }

            private set
            {
                instance = null;
            }
        }

        public void SetState(
            XboxLiveAPIName xboxLiveApi,
            HttpRetryAfterApiState state
        )
        {
            lock (instanceLock)
            {
                this.apiStateMap[xboxLiveApi] = state;
            }
        }

        public void ClearState(
            XboxLiveAPIName xboxLiveApi
        )
        {
            lock (instanceLock)
            {
                this.apiStateMap.Remove(xboxLiveApi);
            }

        }

        public bool GetState(
            XboxLiveAPIName xboxLiveApi,
            out HttpRetryAfterApiState returnValue
        )
        {
            lock (instanceLock)
            {
                return this.apiStateMap.TryGetValue(xboxLiveApi, out returnValue);
            }
        }
    };
}


