// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerSession
    {
        public MultiplayerSession(string xboxUserId, MultiplayerSessionReference multiplayerSessionReference) {}
        public MultiplayerSession(string xboxUserId, MultiplayerSessionReference multiplayerSessionReference, uint maxMembersInSession, MultiplayerSessionVisibility multiplayerSessionVisibility, string[] initiatorXboxUserIds, string sessionCustomConstantsJson) {}
        public MultiplayerSession(string xboxUserId) {}

        public ulong ChangeNumber
        {
            get;
            private set;
        }

        public string Branch
        {
            get;
            private set;
        }

        public MultiplayerSessionMember CurrentUser
        {
            get;
            private set;
        }

        public MultiplayerSessionChangeTypes SubscribedChangeTypes
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string ServersJson
        {
            get;
            set;
        }

        public uint MembersAccepted
        {
            get;
            private set;
        }

        public MultiplayerSessionMatchmakingServer MatchmakingServer
        {
            get;
            private set;
        }

        public IList<MultiplayerSessionMember> Members
        {
            get;
            private set;
        }

        public MultiplayerSessionRoleTypes SessionRoleTypes
        {
            get;
            private set;
        }

        public MultiplayerSessionProperties SessionProperties
        {
            get;
            private set;
        }

        public MultiplayerSessionConstants SessionConstants
        {
            get;
            private set;
        }

        public MultiplayerSessionReference SessionReference
        {
            get;
            private set;
        }

        public IList<string> HostCandidates
        {
            get;
            private set;
        }

        public WriteSessionStatus WriteStatus
        {
            get;
            private set;
        }

        public uint InitializingEpisode
        {
            get;
            private set;
        }

        public DateTimeOffset InitializingStageStartTime
        {
            get;
            private set;
        }

        public MultiplayerInitializationStage InitializationStage
        {
            get;
            private set;
        }

        public DateTimeOffset DateOfSession
        {
            get;
            private set;
        }

        public DateTimeOffset DateOfNextTimer
        {
            get;
            private set;
        }

        public DateTimeOffset StartTime
        {
            get;
            private set;
        }

        public string SearchHandleId
        {
            get;
            private set;
        }

        public string MultiplayerCorrelationId
        {
            get;
            private set;
        }


        public static MultiplayerSessionChangeTypes CompareMultiplayerSessions(MultiplayerSession currentSession, MultiplayerSession oldSession)
        {
            throw new NotImplementedException();
        }

        public static WriteSessionStatus ConvertHttpStatusToWriteSessionStatus(int httpStatusCode)
        {
            throw new NotImplementedException();
        }


        public void SetMatchmakingResubmit(bool matchResubmit)
        {
            throw new NotImplementedException();
        }

        public void SetServerConnectionStringCandidates(string[] serverConnectionStringCandidates)
        {
            throw new NotImplementedException();
        }

        public void SetSessionChangeSubscription(MultiplayerSessionChangeTypes changeTypes)
        {
            throw new NotImplementedException();
        }

        public void Leave()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserStatus(MultiplayerSessionMemberStatus status)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserSecureDeviceAddressBase64(string value)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserRoleInfo(Dictionary<string, string> roles)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserMembersInGroup(Microsoft.Xbox.Services.Multiplayer.MultiplayerSessionMember[] membersInGroup)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserQualityOfServiceMeasurements(Microsoft.Xbox.Services.Multiplayer.MultiplayerQualityOfServiceMeasurements[] measurements)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserQualityOfServiceServerMeasurementsJson(string valueJson)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentUserMemberCustomPropertyJson(string name, string valueJson)
        {
            throw new NotImplementedException();
        }

        public void DeleteCurrentUserMemberCustomPropertyJson(string name)
        {
            throw new NotImplementedException();
        }

        public void SetMatchmakingTargetSessionConstantsJson(string matchmakingTargetSessionConstants)
        {
            throw new NotImplementedException();
        }

        public void SetSessionCustomPropertyJson(string name, string valueJson)
        {
            throw new NotImplementedException();
        }

        public void DeleteSessionCustomPropertyJson(string name)
        {
            throw new NotImplementedException();
        }

        public void _Init(string xboxUserId, MultiplayerSessionReference multiplayerSessionReference, uint maxMembersInSession, MultiplayerSessionVisibility multiplayerSessionVisibility, string[] initiatorXboxUserIds, string sessionCustomConstantsJson)
        {
            throw new NotImplementedException();
        }

        public void AddMemberReservation(string xboxUserId, string memberCustomConstantsJson, bool initializeRequested)
        {
            throw new NotImplementedException();
        }

        public void AddMemberReservation(string xboxUserId, string memberCustomConstantsJson)
        {
            throw new NotImplementedException();
        }

        public MultiplayerSessionMember Join(string memberCustomConstantsJson, bool initializeRequested, bool joinWithActiveStatus)
        {
            throw new NotImplementedException();
        }

        public MultiplayerSessionMember Join(string memberCustomConstantsJson, bool initializeRequested)
        {
            throw new NotImplementedException();
        }

        public MultiplayerSessionMember Join(string memberCustomConstantsJson)
        {
            throw new NotImplementedException();
        }

        public MultiplayerSessionMember Join()
        {
            throw new NotImplementedException();
        }

        public void SetVisibility(MultiplayerSessionVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public void SetMaxMembersInSession(uint maxMembersInSession)
        {
            throw new NotImplementedException();
        }

        public void SetMutableRoleSettings(Dictionary<string, Microsoft.Xbox.Services.Multiplayer.MultiplayerRoleType> roleTypes)
        {
            throw new NotImplementedException();
        }

        public void SetTimeouts(TimeSpan memberReservedTimeout, TimeSpan memberInactiveTimeout, TimeSpan memberReadyTimeout, TimeSpan sessionEmptyTimeout)
        {
            throw new NotImplementedException();
        }

        public void SetArbitrationTimeouts(TimeSpan arbitrationTimeout, TimeSpan forfeitTimeout)
        {
            throw new NotImplementedException();
        }

        public void SetQualityOfServiceConnectivityMetrics(bool enableLatencyMetric, bool enableBandwidthDownMetric, bool enableBandwidthUpMetric, bool enableCustomMetric)
        {
            throw new NotImplementedException();
        }

        public void SetManagedInitialization(TimeSpan joinTimeout, TimeSpan measurementTimeout, TimeSpan evaluationTimeout, bool autoEvalute, uint membersNeededToStart)
        {
            throw new NotImplementedException();
        }

        public void SetMemberInitialization(TimeSpan joinTimeout, TimeSpan measurementTimeout, TimeSpan evaluationTimeout, bool autoEvalute, uint membersNeededToStart)
        {
            throw new NotImplementedException();
        }

        public void SetPeerToPeerRequirements(TimeSpan latencyMaximum, uint bandwidthMinimumInKilobitsPerSecond)
        {
            throw new NotImplementedException();
        }

        public void SetPeerToHostRequirements(TimeSpan latencyMaximum, uint bandwidthDownMinimumInKilobitsPerSecond, uint bandwidthUpMinimumInKilobitsPerSecond, MultiplayMetrics hostSelectionMetric)
        {
            throw new NotImplementedException();
        }

        public void SetMeasurementServerAddresses(Microsoft.Xbox.Services.GameServerPlatform.QualityOfServiceServer[] measurementServerAddresses)
        {
            throw new NotImplementedException();
        }

        public void SetSessionCapabilities(MultiplayerSessionCapabilities capabilities)
        {
            throw new NotImplementedException();
        }

        public void SetInitializationStatus(bool initializationSucceeded)
        {
            throw new NotImplementedException();
        }

        public void SetHostDeviceToken(string hostDeviceToken)
        {
            throw new NotImplementedException();
        }

        public void SetMatchmakingServerConnectionPath(string serverConnectionPath)
        {
            throw new NotImplementedException();
        }

        public void SetClosed(bool closed)
        {
            throw new NotImplementedException();
        }

    }
}
