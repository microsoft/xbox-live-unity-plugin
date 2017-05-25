// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xbox.Services.System
{
    internal class AuthConfig
    {
        public string Sandbox { get; set; }
        public string XboxLiveRelyingParty { get; set; }
        public string EnvironmentPrefix { get; set; }
        public string Environment { get; set; }
        public bool UseCompactTicket { get; set; }
        public string XboxLiveEndpoint { get; set; }
        public string RPSTicketService { get; set; }
        public string RPSTicketPolicy { get; set; }
        public string UserTokenSiteName { get; set; }
        public AuthConfig()
        {
            XboxLiveEndpoint = "https://xboxlive.com";
            XboxLiveRelyingParty = "https://auth.xboxlive.com";
            UserTokenSiteName = GetEndpointPath("user.auth", "", Environment, false);
            RPSTicketPolicy = UseCompactTicket ? "MBI_SSL" : "DELEGATION";
            RPSTicketService = UseCompactTicket ? UserTokenSiteName : "xbl.signin xbl.friends";
        }

        public static string GetEndpointPath(string serviceName, string EnvironmentPrefix, string Environment, bool appendProtocol = true)
        {
            string endpointPath = "";
            if(appendProtocol)
            {
                endpointPath += "https://";
            }
            endpointPath += EnvironmentPrefix + serviceName + Environment + ".xboxlive.com";
            return endpointPath;
        }
    }
}
