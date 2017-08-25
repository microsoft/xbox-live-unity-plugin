// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

namespace UWPIntegration
{
    using System;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using Microsoft.Xbox;
    using Microsoft.Xbox.Services;
    using Microsoft.Xbox.Services.Privacy;
    using Microsoft.Xbox.Services.Leaderboard;
    using Microsoft.Xbox.Services.Social.Manager;
    using Microsoft.Xbox.Services.Statistics.Manager;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Data;
    using System.Diagnostics;
    using Microsoft.Xbox.Services.System;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private int jumps;
        private int headshots;
        private LeaderboardResult leaderboard;
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

        public IStatsManager StatsManager
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
                    StatName = "jumps"
                };
                this.StatsManager.GetLeaderboard(this.User, query);
            }
        }

        private void socialLeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.User.IsSignedIn)
            {
                LeaderboardQuery query = new LeaderboardQuery
                {
                    MaxItems = 3,
                    SocialGroup = "all",
                    StatName = "headshots"
                };
                this.StatsManager.GetLeaderboard(this.User, query);
            }
        }

        private void WriteGlobalStats_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.StatsManager.SetStatAsInteger(this.User, "jumps", ++this.jumps);
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
        }

        private void WriteSocialStats_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;
            this.StatsManager.SetStatAsInteger(this.User, "headshots", ++this.headshots);
        }

        private void NextLb_Click(object sender, RoutedEventArgs e)
        {
            if (!this.User.IsSignedIn) return;

            if (this.LeaderboardResult.HasNext)
            {
                this.StatsManager.GetLeaderboard(this.User, this.LeaderboardResult.NextQuery);
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

            List<ulong> userIds = new List<ulong>();
            foreach (XboxSocialUser user in XboxSocialUserGroupAll.Users)
            {
                userIds.Add(user.XboxUserId);
            }

            this.XboxSocialUserGroupFromList = this.SocialManager.CreateSocialUserGroupFromList(this.User, userIds);
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
                    var statsDoWorkTask = Task.Run(() => { return this.StatsManager.DoWork(); });
                    List<StatEvent> statsEvents = await statsDoWorkTask;
                    foreach (StatEvent ev in statsEvents)
                    {
                        if (ev.EventType == StatEventType.GetLeaderboardComplete)
                        {
                            LeaderboardResult result = ((LeaderboardResultEventArgs)ev.EventArgs).Result;
                            this.LeaderboardResult = result;

                            NextLbBtn.IsEnabled = result.HasNext;
                        }
                    }

                    var statNames = this.StatsManager.GetStatNames(this.User);
                    if (statNames.Count > 0)
                    {
                        foreach (var stat in statNames)
                        {
                            if (string.Equals(stat, "headshots"))
                            {
                                this.headshots = this.StatsManager.GetStat(this.User, "headshots").AsInteger();
                            }
                            else if (string.Equals(stat, "jumps"))
                            {
                                this.jumps = this.StatsManager.GetStat(this.User, "jumps").AsInteger();
                            }
                        }
                        this.StatsData.Text = string.Join(Environment.NewLine, statNames.Select(n => this.StatsManager.GetStat(this.User, n)).Select(s => $"{s.Name} ({s.Type}) = {s.Value}"));
                    }

                    var socialDoWorkTask = Task.Run(() => { return this.SocialManager.DoWork(); });
                    IList<SocialEvent> socialEvents = await socialDoWorkTask;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}