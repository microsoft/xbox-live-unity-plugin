// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Xbox.Services.Leaderboard;
using Microsoft.Xbox.Services.Statistics.Manager;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Microsoft.Xbox.Services.Social.Manager;
using System.Linq;

namespace Microsoft.Xbox.Services.Client
{
    [Serializable]
    public class Leaderboard : MonoBehaviour
    {
        private string socialGroup;

        public Themes Theme = Themes.Light;

        public StatBase stat;

        public LeaderboardTypes leaderboardType;

        [Range(1, 100)]
        public uint entryCount = 10;

        public Text headerText;
        
        public uint currentPage;
        
        public uint totalPages;
        
        public Text pageText;
        
        public Button firstButton;
        
        public Button previousButton;
        
        public Button nextButton;
        
        public Button lastButton;

        public bool EnableControllerInput = false;

        public int JoystickNumber = 1;

        public XboxControllerButtons FirstPageButton;

        public XboxControllerButtons LastPageButton;

        public XboxControllerButtons NextPageButton;

        public XboxControllerButtons PreviousPageButton;

        public XboxControllerButtons RefreshButton;

        public XboxControllerButtons NextViewButton;

        public XboxControllerButtons PrevViewButton;

        public int PlayerNumber = 1;

        public string verticalScrollInputAxis;

        public Transform contentPanel;

        public ScrollRect scrollRect;

        public Image NextViewImage;

        public Image PrevViewImage;

        public float scrollSpeedMultiplier = 0.1f;

        private LeaderboardResult leaderboardData;
        private ObjectPool entryObjectPool;
        private XboxLiveUser xboxLiveUser;
        private string firstControllerButton;
        private string lastControllerButton;
        private string nextControllerButton;
        private string prevControllerButton;
        private string refreshControllerButton;
        private string prevViewControllerButton;
        private string nextViewControllerButton;
        private LeaderboardFilter viewFilter = LeaderboardFilter.All;

        private bool isLocalUserAdded
        {
            get
            {
                return statsAddedLocalUser && socialAddedLocalUser;
            }
        }
        private bool statsAddedLocalUser, socialAddedLocalUser;
        private XboxSocialUserGroup userGroup;

        private void Awake()
        {
            this.EnsureEventSystem();
            XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();

            if (this.stat == null)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogFormat("Leaderboard '{0}' does not have a stat configured and will not function properly.", this.name);
                }
                return;
            }

            this.headerText.text = this.stat.DisplayName.ToUpper();
            this.entryObjectPool = this.GetComponent<ObjectPool>();
            this.UpdateButtons();
            SocialManagerComponent.Instance.EventProcessed += this.SocialManagerEventProcessed;
            StatsManagerComponent.Instance.LocalUserAdded += this.LocalUserAdded;
            StatsManagerComponent.Instance.GetLeaderboardCompleted += this.GetLeaderboardCompleted;
            SignInManager.Instance.OnPlayerSignOut(this.PlayerNumber, this.OnPlayerSignOut);
            this.statsAddedLocalUser = false;
            this.socialAddedLocalUser = false;

            if (EnableControllerInput)
            {
                if (this.FirstPageButton != XboxControllerButtons.None)
                {
                    this.firstControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.FirstPageButton);
                }

                if (this.LastPageButton != XboxControllerButtons.None)
                {
                    this.lastControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.LastPageButton);
                }

                if (this.NextPageButton != XboxControllerButtons.None)
                {
                    this.nextControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.NextPageButton);
                }

                if (this.PreviousPageButton != XboxControllerButtons.None)
                {
                    this.prevControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.PreviousPageButton);
                }

                if (this.RefreshButton != XboxControllerButtons.None)
                {
                    this.refreshControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.RefreshButton);
                }

                if (this.NextViewButton != XboxControllerButtons.None)
                {
                    this.nextViewControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.NextViewButton);
                    this.NextViewImage.sprite = XboxControllerConverter.GetXboxButtonSpite(this.Theme, this.NextViewButton);
                    this.NextViewImage.SetNativeSize();
                }
                else
                {
                    this.NextViewImage.enabled = false;
                }

                if (this.PrevViewButton != XboxControllerButtons.None)
                {
                    this.prevViewControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.PrevViewButton);
                    this.PrevViewImage.sprite = XboxControllerConverter.GetXboxButtonSpite(this.Theme, this.PrevViewButton);
                    this.PrevViewImage.SetNativeSize();
                }
                else
                {
                   
                }
            }
            else {
                this.NextViewImage.enabled = false;
                this.PrevViewImage.enabled = false;
            }
        }

        private void Start()
        {

            if (this.xboxLiveUser == null)
            {
                this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
                if (this.xboxLiveUser != null)
                {
                    this.statsAddedLocalUser = true;
                    this.socialAddedLocalUser = true;
                    this.UpdateData(0, viewFilter);
                }
            }
        }

        public void RequestFlushToService(bool isHighPriority)
        {
            StatsManagerComponent.Instance.RequestFlushToService(this.xboxLiveUser, isHighPriority);
        }

        void Update()
        {
            if (this.EnableControllerInput)
            {
                if (!string.IsNullOrEmpty(this.refreshControllerButton) && Input.GetKeyDown(this.refreshControllerButton))
                {
                    this.Refresh();
                }

                if (this.currentPage != 0 && !string.IsNullOrEmpty(this.prevControllerButton) && Input.GetKeyDown(this.prevControllerButton))
                {
                    this.PreviousPage();
                }

                if (this.currentPage != this.totalPages && !string.IsNullOrEmpty(this.nextControllerButton) && Input.GetKeyDown(this.nextControllerButton))
                {
                    this.NextPage();
                }

                if (!string.IsNullOrEmpty(this.lastControllerButton) && Input.GetKeyDown(this.lastControllerButton))
                {
                    this.LastPage();
                }

                if (!string.IsNullOrEmpty(this.firstControllerButton) && Input.GetKeyDown(this.firstControllerButton))
                {
                    this.FirstPage();
                }

                if (!string.IsNullOrEmpty(this.verticalScrollInputAxis) && Input.GetAxis(this.verticalScrollInputAxis) != 0)
                {
                    var inputValue = Input.GetAxis(this.verticalScrollInputAxis);
                    this.scrollRect.verticalScrollbar.value = this.scrollRect.verticalNormalizedPosition + inputValue * scrollSpeedMultiplier;
                }
            }
        }

        public void Refresh()
        {
            this.FirstPage();
        }

        public void NextPage()
        {
            this.UpdateData(this.currentPage + 1, viewFilter);
        }

        public void PreviousPage()
        {
            if (this.currentPage > 0)
            {
                this.UpdateData(this.currentPage - 1, viewFilter);
            }
        }

        public void FirstPage()
        {
            this.UpdateData(0, viewFilter);
        }

        public void LastPage()
        {
            this.UpdateData(this.totalPages - 1, viewFilter);
        }

        public void LoadView(int newFilter) {
            this.Clear();
            this.UpdateData(0, (LeaderboardFilter)newFilter);
        }

        private void UpdateData(uint pageNumber, LeaderboardFilter filter)
        {
            this.viewFilter = filter;

            if (!this.isLocalUserAdded)
            {
                return;
            }

            if (this.stat == null)
            {
                return;
            }

            if (this.xboxLiveUser == null)
            {
                this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            }

            LeaderboardQuery query;
            if (pageNumber == this.currentPage + 1 && this.leaderboardData != null && this.leaderboardData.HasNext)
            {
                query = this.leaderboardData.GetNextQuery();
            }
            else
            {
                switch (leaderboardType)
                {
                    case LeaderboardTypes.Global:
                        socialGroup = "";
                        break;
                    case LeaderboardTypes.Favorites:
                        socialGroup = "favorite";
                        break;
                    case LeaderboardTypes.Friends:
                        socialGroup = "all";
                        break;
                }

                if (filter == LeaderboardFilter.All)
                {
                    query = new LeaderboardQuery()
                    {
                        SkipResultToRank = pageNumber == 0 ? 0 : ((pageNumber - 1) * this.entryCount),
                        MaxItems = this.entryCount,
                    };
                } else {

                    query = new LeaderboardQuery()
                    {
                        SkipResultToMe = true,
                        MaxItems = this.entryCount,
                    };
                }
            }

            this.currentPage = pageNumber;
            XboxLive.Instance.StatsManager.GetLeaderboard(this.xboxLiveUser, this.stat.ID, query);
        }

        private void SocialManagerEventProcessed(object sender, SocialEvent socialEvent)
        {
            if (socialEvent.EventType == SocialEventType.LocalUserAdded)
            {
                socialAddedLocalUser = true;
                this.Refresh();
            }
            else if (socialEvent.EventType == SocialEventType.SocialUserGroupLoaded &&
                ((SocialUserGroupLoadedEventArgs)socialEvent.EventArgs).SocialUserGroup == this.userGroup)
            {
                var entries = this.contentPanel.GetComponentsInChildren<PlayerProfile>();
                for (int i = 0; i < entries.Length; i++)
                {
                    XboxSocialUser user = userGroup.Users.FirstOrDefault(x => x.Gamertag == entries[i].GamerTagText.text);
                    if (user != null)
                    {
                        this.StartCoroutine(entries[i].LoadGamerpic(user.DisplayPicRaw + "&w=128"));
                    }
                }
            }
        }

        private void LocalUserAdded(object sender, XboxLiveUserEventArgs e)
        {
            this.statsAddedLocalUser = true;
            this.Refresh();
        }

        private void GetLeaderboardCompleted(object sender, StatEventArgs e)
        {
            if (e.EventData.ErrorCode != 0)
            {
                return;
            }

            LeaderboardResultEventArgs leaderboardArgs = (LeaderboardResultEventArgs)e.EventData.EventArgs;
            this.LoadResult(leaderboardArgs.Result);
        }

        /// <summary>
        /// Load the leaderboard result data from the service into the view.
        /// </summary>
        /// <param name="result"></param>
        private void LoadResult(LeaderboardResult result)
        {
            if (this.stat == null || (result.HasNext && (this.stat.ID != result.GetNextQuery().StatName || this.socialGroup != result.GetNextQuery().SocialGroup)))
            {
                return;
            }

            this.leaderboardData = result;

            uint displayCurrentPage = this.currentPage + 1;
            if (this.leaderboardData.TotalRowCount == 0)
            {
                this.totalPages = 0;
                displayCurrentPage = 0;
            }
            else if (this.totalPages == 0)
            {
                this.totalPages = this.leaderboardData.TotalRowCount / this.entryCount;
            }

            this.pageText.text = string.Format("Page: {0} / {1}", displayCurrentPage, this.totalPages);

            while (this.contentPanel.childCount > 0)
            {
                var entry = this.contentPanel.GetChild(0).gameObject;
                this.entryObjectPool.ReturnObject(entry);
            }

            IList<string> xuids = new List<string>();
            
            var rowCount = 0;
            foreach (LeaderboardRow row in this.leaderboardData.Rows)
            {
                xuids.Add(row.XboxUserId);

                GameObject entryObject = this.entryObjectPool.GetObject();
                PlayerProfile entry = entryObject.GetComponent<PlayerProfile>();
                if (this.xboxLiveUser != null && row.Gamertag.Equals(this.xboxLiveUser.Gamertag)) {
                    entry.IsCurrentPlayer = true;
                }
                if (rowCount == 0)
                {
                    entry.IsHighlighted = true;
                }
                else
                {
                    entry.BackgroundColor = ((rowCount % 2 == 0) ? PlayerProfileBackgrounds.RowBackground02 : PlayerProfileBackgrounds.RowBackground01);
                }

                entry.UpdateGamerTag(row.Gamertag);
                entry.UpdateRank(true, row.Rank);
                if (row.Values != null && row.Values.Count > 0)
                {
                    entry.UpdateScore(true, row.Values[0]);
                }
                entryObject.transform.SetParent(this.contentPanel);
                rowCount++;
            }
            /*
            userGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromList(XboxLiveUserManager.Instance.UserForSingleUserMode.User, xuids);
            */
            // Reset the scroll view to the top.
            this.scrollRect.verticalNormalizedPosition = 1;
            this.UpdateButtons();
        }

        public void UpdateButtons()
        {
            this.firstButton.interactable = this.previousButton.interactable = this.currentPage != 0;
            this.nextButton.interactable = this.lastButton.interactable = this.totalPages > 1 && this.currentPage < this.totalPages - 1;
        }

        private void Clear() {
            var children = new List<GameObject>();
            for (int i = 0; i < this.contentPanel.childCount; i++)
            {
                GameObject child = this.contentPanel.transform.GetChild(i).gameObject;
                children.Add(child);
            }

            this.contentPanel.DetachChildren();

            foreach (var child in children)
            {
                Destroy(child);
            }
        }

        private void OnPlayerSignOut(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus authStatus, string errorMessage)
        {
            if (authStatus == XboxLiveAuthStatus.Succeeded)
            {
                this.xboxLiveUser = null;
                this.Clear();
            }
            else
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(errorMessage);
                }
            }
        }

        private void OnDestroy()
        {
            this.statsAddedLocalUser = false;
            this.socialAddedLocalUser = false;
            if (SocialManagerComponent.Instance != null)
            {
                SocialManagerComponent.Instance.EventProcessed -= this.SocialManagerEventProcessed;
            }

            if (StatsManagerComponent.Instance != null)
            {
                StatsManagerComponent.Instance.LocalUserAdded -= this.LocalUserAdded;
                StatsManagerComponent.Instance.GetLeaderboardCompleted -= this.GetLeaderboardCompleted;
            }

            if (SignInManager.Instance != null)
            {
                SignInManager.Instance.RemoveCallbackFromPlayer(this.PlayerNumber, this.OnPlayerSignOut);
            }
        }
    }

    public enum LeaderboardFilter {
        All,
        NearestMe
    }
}