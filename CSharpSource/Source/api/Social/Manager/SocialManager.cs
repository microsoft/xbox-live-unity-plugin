// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.Social.Manager
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;
    using global::System.Threading.Tasks;

    using Microsoft.Xbox.Services.System;
    using Presence;

    public partial class SocialManager
    {
        private static ISocialManager m_instance;

        private static readonly object m_instanceLock = new object();
        
        public static ISocialManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_instanceLock)
                    {
                        if (m_instance == null)
                        {
                            m_instance = XboxLive.UseMockServices ? (ISocialManager)new MockSocialManager() : (ISocialManager)new SocialManager();
                        }
                    }
                }
                return m_instance;
            }
        }
                
    }
}