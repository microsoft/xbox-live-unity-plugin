// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerSessionConstants
    {

        public string MeasurementServerAddressesJson
        {
            get;
            private set;
        }

        public MultiplayerPeerToHostRequirements PeerToHostRequirements
        {
            get;
            private set;
        }

        public MultiplayerPeerToPeerRequirements PeerToPeerRequirements
        {
            get;
            private set;
        }

        public MultiplayerMemberInitialization MemberInitialization
        {
            get;
            private set;
        }

        public MultiplayerManagedInitialization ManagedInitialization
        {
            get;
            private set;
        }

        public bool EnableMetricsCustom
        {
            get;
            private set;
        }

        public bool EnableMetricsBandwidthUp
        {
            get;
            private set;
        }

        public bool EnableMetricsBandwidthDown
        {
            get;
            private set;
        }

        public bool EnableMetricsLatency
        {
            get;
            private set;
        }

        public bool CapabilitiesArbitration
        {
            get;
            private set;
        }

        public bool CapabilitiesSearchable
        {
            get;
            private set;
        }

        public bool CapabilitiesTeam
        {
            get;
            private set;
        }

        public bool CapabilitiesUserAuthorizationStyle
        {
            get;
            private set;
        }

        public bool CapabilitiesCrossplay
        {
            get;
            private set;
        }

        public bool CapabilitiesConnectionRequiredForActiveMember
        {
            get;
            private set;
        }

        public bool CapabilitiesLarge
        {
            get;
            private set;
        }

        public bool CapabilitiesGameplay
        {
            get;
            private set;
        }

        public bool CapabilitiesSuppressPresenceActivityCheck
        {
            get;
            private set;
        }

        public bool CapabilitiesConnectivity
        {
            get;
            private set;
        }

        public TimeSpan ForfeitTimeout
        {
            get;
            private set;
        }

        public TimeSpan ArbitrationTimeout
        {
            get;
            private set;
        }

        public TimeSpan SessionEmptyTimeout
        {
            get;
            private set;
        }

        public TimeSpan MemberReadyTimeout
        {
            get;
            private set;
        }

        public TimeSpan MemberInactiveTimeout
        {
            get;
            private set;
        }

        public TimeSpan MemberReservationTimeout
        {
            get;
            private set;
        }

        public string CustomConstantsJson
        {
            get;
            private set;
        }

        public IList<string> InitiatorXboxUserIds
        {
            get;
            private set;
        }

        public MultiplayerSessionVisibility MultiplayerSessionVisibility
        {
            get;
            set;
        }

        public uint MaxMembersInSession
        {
            get;
            set;
        }

    }
}
