using Microsoft.Xbox.Services.Shared.Presence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Presence
{
    public class PresenceWriter: IPresenceWriter
    {
        private const string PresenceWriterApiContract = "3";
        private static readonly Uri UserPresenceBaseUri = new Uri("https://userpresence.xboxlive.com");
        private static readonly string HeartBeatFromServiceHeaderName = "X-Heartbeat-After";

        private static IPresenceWriter instance;
        private static readonly object instanceLock = new object();

        private const int defaultHeartBeatDelayInMins = 5;
        private bool writerInProgress;
        private bool isCallInProgess;
        private bool stopTimerCalled;
        private Timer timer;
        private Dictionary<XboxLiveUser, UserPresenceWriterStatus> usersBeingTrackedMap;

        private PresenceWriter()
        {
            writerInProgress = false;
            isCallInProgess = false;
            usersBeingTrackedMap = new Dictionary<XboxLiveUser, UserPresenceWriterStatus>();
        }

        internal static IPresenceWriter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = XboxLive.UseMockServices ? new MockPresenceWriter() : (IPresenceWriter) new PresenceWriter();
                        }
                    }
                }
                return instance;
            }
        }

        private void StartTimer() {
            this.timer = new Timer(
                new TimerCallback(this.HandleTimeTrigger),
                null,
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMinutes(0.5));
        }

        public void StartWriter()
        {
            if (!writerInProgress)
            {
                writerInProgress = true;
                this.StartTimer();
            }
        }

        public void StopWriter()
        {
            if (writerInProgress) {
                writerInProgress = false;
                stopTimerCalled = true;
                if (!isCallInProgess)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    var tasksToWaitOn = new List<Task>();
                    foreach (var user in usersBeingTrackedMap.Keys)
                    {
                        tasksToWaitOn.Add(SetPresenceHelper(user, false, new PresenceData(XboxLive.Instance.AppConfig.PrimaryServiceConfigId, null)));
                    }

                    Task.WhenAll(tasksToWaitOn).ContinueWith(x =>
                    {
                        isCallInProgess = false;
                    });
                }
                else {
                    foreach (var user in usersBeingTrackedMap.Keys)
                    {
                        RemoveUser(user);
                    }
                }
            }
        }

        public void HandleTimeTrigger(object obj)
        {
            if (!this.isCallInProgess)
            {
                isCallInProgess = true;
                var tasksToWaitOn = new List<Task>();
                foreach (var user in usersBeingTrackedMap.Keys) {
                    usersBeingTrackedMap[user].HeartBeatIntervalInMins -= 0.5f;
                    if (usersBeingTrackedMap[user].HeartBeatIntervalInMins <= 0) {
                        if (!usersBeingTrackedMap[user].ShouldRemove)
                        {
                            tasksToWaitOn.Add(SetPresenceHelper(user, true, new PresenceData(XboxLive.Instance.AppConfig.PrimaryServiceConfigId, null)));
                        }
                        else {
                            tasksToWaitOn.Add(SetPresenceHelper(user, false, new PresenceData(XboxLive.Instance.AppConfig.PrimaryServiceConfigId, null)));
                        }
                    }
                }

                Task.WhenAll(tasksToWaitOn).ContinueWith(x => {
                    isCallInProgess = false;

                    if (stopTimerCalled) {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                });
            }
        }

        public void AddUser(XboxLiveUser xboxLiveUser) {
            this.usersBeingTrackedMap.Add(xboxLiveUser, new UserPresenceWriterStatus() {
                HeartBeatIntervalInMins = defaultHeartBeatDelayInMins,
                ShouldRemove = false});
        }

        public void RemoveUser(XboxLiveUser xboxLiveUser) {
            this.usersBeingTrackedMap[xboxLiveUser] = new UserPresenceWriterStatus() { ShouldRemove = true };
        }

        private Task SetPresenceHelper(XboxLiveUser user, bool isUserActiveInTitle, PresenceData presenceData) {
            var subQuery = SetPresenceSubPath(user, isUserActiveInTitle);
            var httpRequest = XboxLiveHttpRequest.Create(HttpMethod.Post, UserPresenceBaseUri.ToString(), subQuery);
            httpRequest.ContractVersion = PresenceWriterApiContract;
            httpRequest.XboxLiveAPI = XboxLiveAPIName.SetPresenceHelper;
            httpRequest.RequestBody = JsonSerialization.ToJson(new PresenceTitleRequest(isUserActiveInTitle, presenceData));

            return httpRequest.GetResponseWithAuth(user).ContinueWith(
                responseTask => HandleSetPresenceResponse(user, responseTask.Result));
        }

        private string SetPresenceSubPath(XboxLiveUser user, bool isUserActiveInTitle) {
            var pathBuilder = new StringBuilder();
            pathBuilder.AppendFormat("users/xuid({0})/devices/current/titles/current", user.XboxUserId);
            return pathBuilder.ToString();
        }

        private void HandleSetPresenceResponse(XboxLiveUser xboxLiveUser, XboxLiveHttpResponse httpResponse) {

            if (httpResponse.Headers.ContainsKey(HeartBeatFromServiceHeaderName))
            {
                var heartBeatFromServiceInSecs = int.Parse(httpResponse.Headers[HeartBeatFromServiceHeaderName]);
                usersBeingTrackedMap[xboxLiveUser].HeartBeatIntervalInMins = (heartBeatFromServiceInSecs / 60);
            }
            else {
                usersBeingTrackedMap[xboxLiveUser].HeartBeatIntervalInMins = defaultHeartBeatDelayInMins;
            }

            if (usersBeingTrackedMap[xboxLiveUser].ShouldRemove) {
                usersBeingTrackedMap.Remove(xboxLiveUser);
            }
        }

    }
}
