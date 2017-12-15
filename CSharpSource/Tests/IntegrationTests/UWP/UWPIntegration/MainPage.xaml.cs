// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace UWPIntegration
{
    using Microsoft.Xbox.Services;
    using Microsoft.Xbox.Services.Leaderboard;
    using Microsoft.Xbox.Services.Privacy;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.Statistics.Manager;
    using Microsoft.Xbox.Services.System;
    using Microsoft.Xbox.Services.TitleStorage;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private long jumps;
        private long headshots;
        private LeaderboardResult leaderboard;
        private TitleStorageBlobMetadata blobMetadata = null;
        private XboxSocialUserGroup xboxSocialUserGroupAll;
        private XboxSocialUserGroup xboxSocialUserGroupAllOnline;
        private XboxSocialUserGroup xboxSocialUserGroupFromList;
        private XboxLiveUser user;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeUser();
            DoWork();
        }

        async void InitializeUser()
        {
            var allUser = await Windows.System.User.FindAllAsync();
            var validSysUser = allUser.Where(user => (user.Type != Windows.System.UserType.LocalGuest || user.Type != Windows.System.UserType.RemoteGuest)).ToList();
            if (validSysUser.Count > 0)
            {
                this.user = new XboxLiveUser(validSysUser[0]);
            }
            else
            {
                this.user = new XboxLiveUser();
            }
        }

        public LeaderboardResult LeaderboardResult
        {
            get
            {
                return this.leaderboard;
            }
            set
            {
                this.leaderboard = value;
                this.OnPropertyChanged();
            }
        }

        public XboxSocialUserGroup XboxSocialUserGroupAll
        {
            get
            {
                return this.xboxSocialUserGroupAll;
            }
            set
            {
                this.xboxSocialUserGroupAll = value;
                this.OnPropertyChanged();
            }
        }

        public XboxSocialUserGroup XboxSocialUserGroupAllOnline
        {
            get
            {
                return this.xboxSocialUserGroupAllOnline;
            }
            set
            {
                this.xboxSocialUserGroupAllOnline = value;
                this.OnPropertyChanged();
            }
        }

        public XboxSocialUserGroup XboxSocialUserGroupFromList
        {
            get
            {
                return this.xboxSocialUserGroupFromList;
            }
            set
            {
                this.xboxSocialUserGroupFromList = value;
                this.OnPropertyChanged();
            }
        }

        public XboxLiveUser User
        {
            get { return this.user; }
        }

        public IStatisticManager StatsManager
        {
            get { return XboxLive.Instance.StatsManager; }
        }

        public ISocialManager SocialManager
        {
            get { return XboxLive.Instance.SocialManager; }
        }

        private async void SignInSilentButton_Click(object sender, RoutedEventArgs e)
        {
            var signInResult = await this.User.SignInSilentlyAsync();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (signInResult.Status == SignInStatus.Success)
                {
                    this.OnPropertyChanged("User");
                    this.StatsManager.AddLocalUser(this.User);
                    this.SocialManager.AddLocalUser(this.User);
                }
            });
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var signInResult = await this.User.SignInAsync();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (signInResult.Status == SignInStatus.Success)
                {
                    this.OnPropertyChanged("User");
                    this.StatsManager.AddLocalUser(this.User);
                    this.SocialManager.AddLocalUser(this.User);
                }
            });
        }

        private void globalLeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.User.IsSignedIn)
            {
                this.StatsManager.RequestFlushToService(this.User, true);
                this.StatsManager.DoWork();

                LeaderboardQuery query = new LeaderboardQuery
                {
                    MaxItems = 3,
                };
                this.StatsManager.GetLeaderboard(this.User, "jumps", query);
            }
        }

        private void socialLeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.User.IsSignedIn)
            {
                LeaderboardQuery query = new LeaderboardQuery
                {
                    MaxItems = 3
                };
                this.StatsManager.GetSocialLeaderboard(this.User, "headshots", "all", query);
            }
        }

        private void WriteGlobalStats_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.StatsManager.SetStatisticIntegerData(this.User, "jumps", ++this.jumps);
        }

        private void FlushStats_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.StatsManager.RequestFlushToService(this.User);
        }

        private void ShowProfileCard_Click(object sender, RoutedEventArgs e)
        {
            TitleCallableUI.ShowProfileCardUIAsync(this.User, "2814613569642996");
        }

        private async void CheckPrivilege_Click(object sender, RoutedEventArgs e)
        {
            // If you want to see the dialog, change your privacy settings to block 
            // multilayer sessions for this account on the console

            var checkPermission = TitleCallableUI.CheckGamingPrivilegeSilently(this.User, GamingPrivilege.MultiplayerSessions);
            if (!checkPermission)
            {
                // Show UI if CheckPrivilegeSilently fails.
                var result = await TitleCallableUI.CheckGamingPrivilegeWithUI(this.User, GamingPrivilege.MultiplayerSessions, "");
            }
        }

        private async void CheckPermissions_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            var result = await this.User.Services.PrivacyService.CheckPermissionWithTargetUserAsync(PermissionIdConstants.ViewTargetVideoHistory, "2814680291986301");

            string resultText = string.Format("Allowed: {0}", result.IsAllowed);
            if (!result.IsAllowed)
            {
                foreach (var reason in result.DenyReasons)
                {
                    resultText += string.Format("\tReason: {0}", reason.Reason);
                }
            }
            this.PrivacyData.Text = resultText;
        }
        private async void CheckMultiplePermissions_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            List<string> permissionIds = new List<string>();
            permissionIds.Add(PermissionIdConstants.ViewTargetVideoHistory);
            permissionIds.Add(PermissionIdConstants.ViewTargetMusicStatus);
            permissionIds.Add(PermissionIdConstants.ViewTargetGameHistory);

            List<string> xuids = new List<string>();
            xuids.Add("2814680291986301");
            xuids.Add("2814634309691161");
            var result = await this.User.Services.PrivacyService.CheckMultiplePermissionsWithMultipleTargetUsersAsync(permissionIds, xuids);

            string resultText = "";
            foreach (var multiplePermissionsResult in result)
            {
                resultText += string.Format("Xuid: {0}", multiplePermissionsResult.XboxUserId);
                foreach (var permissionResult in multiplePermissionsResult.Items)
                {
                    resultText += string.Format("\tPermission {0} allowed: {1}", permissionResult.PermissionRequested, permissionResult.IsAllowed);
                    if (!permissionResult.IsAllowed)
                    {
                        foreach (var reason in permissionResult.DenyReasons)
                        {
                            resultText += string.Format("\tReason: {0}", reason.Reason);
                        }
                    }
                }
                resultText += "\n";
            }
            this.PrivacyData.Text = resultText;
        }

        private async void GetAvoidList_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            var result = await this.user.Services.PrivacyService.GetAvoidListAsync();

            string resultText = "Avoided Xuids: ";
            foreach (var xuid in result)
            {
                resultText += xuid + "\t";
            }
            this.PrivacyData.Text = resultText;
        }

        private async void GetMuteList_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            var result = await this.user.Services.PrivacyService.GetMuteListAsync();

            string resultText = "Muted Xuids: ";
            foreach (var xuid in result)
            {
                resultText += xuid + "\t";
            }
            this.PrivacyData.Text = resultText;
        }

        private void WriteSocialStats_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.StatsManager.SetStatisticIntegerData(this.User, "headshots", ++this.headshots);
        }

        private void NextLb_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            if (this.LeaderboardResult.HasNext)
            {
                LeaderboardQuery nextQuery = this.LeaderboardResult.GetNextQuery();
                this.StatsManager.GetLeaderboard(this.User, nextQuery.StatName, nextQuery);
            }
        }

        private void createSocialUserGroupAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.XboxSocialUserGroupAll = this.SocialManager.CreateSocialUserGroupFromFilters(this.User, PresenceFilter.All, RelationshipFilter.Friends);
        }

        private void createSocialUserGroupAllOnlineButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.XboxSocialUserGroupAllOnline = this.SocialManager.CreateSocialUserGroupFromFilters(this.User, PresenceFilter.AllOnline, RelationshipFilter.Friends);
        }

        private void createSocialUserGroupFromListButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            List<string> userIds = new List<string>();
            foreach (XboxSocialUser user in XboxSocialUserGroupAll.Users)
            {
                userIds.Add(user.XboxUserId);
            }

            this.XboxSocialUserGroupFromList = this.SocialManager.CreateSocialUserGroupFromList(this.User, userIds);
        }
        
        private async void TitleStorageGetQuota_Click(object sender, RoutedEventArgs e)
        {
            var quota = await this.User.Services.TitleStorageService.GetQuotaAsync(
                XboxLive.Instance.AppConfig.ServiceConfigurationId, TitleStorageType.Universal);

            this.TitleStorageData.Text = string.Format("Used bytes = {0}, Quota bytes = {1}", quota.UsedBytes, quota.QuotaBytes);
        }

        private async void TitleStorageGetMetadata_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var metadataResult = await this.User.Services.TitleStorageService.GetBlobMetadataAsync(
                    XboxLive.Instance.AppConfig.ServiceConfigurationId, TitleStorageType.Universal, "path/to/", this.User.XboxUserId);

                var items = metadataResult.Items;
                if (items.Count > 0)
                {
                    this.blobMetadata = items[0];
                    this.TitleStorageData.Text = string.Format("First metadata item has length {0}", items[0].Length);
                }
            }
            catch(Exception)
            {
                this.TitleStorageData.Text = "Exception occurred in GetBlobMetadataAsync";
            }
        }

        private async void TitleStorageUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var metadata = new TitleStorageBlobMetadata(XboxLive.Instance.AppConfig.ServiceConfigurationId, TitleStorageType.Universal, "path/to/newfile.txt", TitleStorageBlobType.Binary, this.user.XboxUserId);

                var bytes = System.Text.Encoding.Unicode.GetBytes("Hello, world!");

                metadata = await this.User.Services.TitleStorageService.UploadBlobAsync(metadata, bytes.ToList(), TitleStorageETagMatchCondition.NotUsed, TitleStorageService.DefaultUploadBlockSize);

                this.TitleStorageData.Text = string.Format("Uploaded file with length {0}", metadata.Length);
            }
            catch (Exception)
            {
                this.TitleStorageData.Text = "Exception occurred in UploadBlobAsync";
            }
        }

        private async void TitleStorageDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.blobMetadata == null)
                {
                    this.TitleStorageData.Text = string.Format("Call GetBlobMetadataAsync first");
                }
                else
                {
                    var downloadResult = await this.User.Services.TitleStorageService.DownloadBlobAsync(this.blobMetadata, TitleStorageETagMatchCondition.NotUsed, null);

                    string text = System.Text.Encoding.Unicode.GetString(downloadResult.BlobBuffer);

                    this.TitleStorageData.Text = string.Format("Downloaded file with content:\n {0}", text);
                }
            }
            catch (Exception)
            {
                this.TitleStorageData.Text = "Exception occurred in DownloadBlobAsync";
            }
        }

        private async void TitleStorageDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var metadata = new TitleStorageBlobMetadata(XboxLive.Instance.AppConfig.ServiceConfigurationId, TitleStorageType.Universal, "path/to/newfile.txt", TitleStorageBlobType.Binary, this.user.XboxUserId);
                await this.User.Services.TitleStorageService.DeleteBlobAsync(metadata, false);

                this.TitleStorageData.Text = string.Format("Successfully deleted blob with path \"path/to/newfile.txt\"", metadata.Length);
            }
            catch (Exception)
            {
                this.TitleStorageData.Text = "Exception occurred in UploadBlobAsync";
            }
        }


        private void RefreshSocialGroups()
        {
            this.XboxSocialUserGroupAll = this.XboxSocialUserGroupAll;
            this.XboxSocialUserGroupAllOnline = this.XboxSocialUserGroupAllOnline;
            this.XboxSocialUserGroupFromList = this.XboxSocialUserGroupFromList;
        }

        async void DoWork()
        {
            while (true)
            {
                if (this.User.IsSignedIn)
                {
                    // Perform the long running do work task on a background thread.
                    IList<StatisticEvent> statsEvents = this.StatsManager.DoWork();
                    foreach (StatisticEvent ev in statsEvents)
                    {
                        if (ev.EventType == StatisticEventType.GetLeaderboardComplete)
                        {
                            LeaderboardResult result = ((LeaderboardResultEventArgs)ev.EventArgs).Result;
                            this.LeaderboardResult = result;

                            NextLbBtn.IsEnabled = result.HasNext;
                        }
                    }

                    var statNames = this.StatsManager.GetStatisticNames(this.User);
                    if (statNames.Count > 0)
                    {
                        foreach (var stat in statNames)
                        {
                            if (string.Equals(stat, "headshots"))
                            {
                                this.headshots = this.StatsManager.GetStatistic(this.User, "headshots").AsInteger;
                            }
                            else if (string.Equals(stat, "jumps"))
                            {
                                this.jumps = this.StatsManager.GetStatistic(this.User, "jumps").AsInteger;
                            }
                        }
                        this.StatsData.Text = string.Join(Environment.NewLine, statNames.Select(n => this.StatsManager.GetStatistic(this.User, n)).Select(s => $"{s.Name} ({s.DataType}) = {GetStatValue(s)}"));
                    }

                    IList<SocialEvent> socialEvents = this.SocialManager.DoWork();
                    foreach (SocialEvent ev in socialEvents)
                    {
                        string msg;
                        msg = "SocialEvent: " + ev.EventType.ToString();
                        Debug.WriteLine(msg);
                        RefreshSocialGroups();
                    }
                }

                // don't run again for at least 200 milliseconds
                await Task.Delay(200);
            }
        }

        object GetStatValue(StatisticValue value)
        {
            switch (value.DataType)
            {
                case StatisticDataType.Number:
                    return value.AsInteger;
                case StatisticDataType.String:
                    return value.AsString;
                default:
                    return value.AsNumber;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}