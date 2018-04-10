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
using System.Collections;

namespace Microsoft.Xbox.Services.Client
{
    [Serializable]
    public class Leaderboard : MonoBehaviour
    {
        [Header("Theme and Display Settings")]
        public Theme Theme = Theme.Light;

        [Header("Stat Configuration")]

        public int PlayerNumber = 1;

        public StatBase stat;

        public LeaderboardTypes leaderboardType;

        [Range(1, 100)]
        public uint entryCount = 10;

        [Header("Controller Configuration")]
        public bool EnableControllerInput = false;

        public int JoystickNumber = 1;

        public XboxControllerButtons NextPageControllerButton;

        public XboxControllerButtons PreviousPageControllerButton;

        public XboxControllerButtons NextViewControllerButton;

        public XboxControllerButtons PrevViewControllerButton;

        public string verticalScrollInputAxis;

        public float scrollSpeedMultiplier = 0.1f;


        [Header("UI References")]
        public Transform contentPanel;

        public ScrollRect scrollRect;

        public Image BackgroundImage;

        public Image HeaderRowImage;

        public Image NextViewImage;

        public Image PrevViewImage;

        public Image NextPageImage;

        public Image PreviousPageImage;

        public FilterManager ViewSelector;

        public Text HeaderText;

        public Text pageText;

        public Button PreviousPageButton;

        public Text PreviousPageText;

        public Button NextPageButton;

        public Text NextPageText;

        public Image TopLine;

        public Image BottomLine;

        private int currentHighlightedEntryPosition = 0;
        private List<PlayerProfile> currentEntries = new List<PlayerProfile>();
        private uint currentPage;
        private uint totalPages;
        private string socialGroup;
        private LeaderboardResult leaderboardData;
        private ObjectPool entryObjectPool;
        private XboxLiveUser xboxLiveUser;
        private string nextControllerButton;
        private string prevControllerButton;
        private string prevViewControllerButton;
        private string nextViewControllerButton;
        private LeaderboardFilter viewFilter = LeaderboardFilter.Default;

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
            this.ViewSelector.Theme = this.Theme;
            if (this.stat == null)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogFormat("Leaderboard '{0}' does not have a stat configured and will not function properly.", this.name);
                }
                return;
            }

            this.HeaderText.text = this.stat.DisplayName.ToUpper();
            this.entryObjectPool = this.GetComponent<ObjectPool>();

            if (EnableControllerInput)
            { 

                if (this.NextPageControllerButton != XboxControllerButtons.None)
                {
                    this.nextControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.NextPageControllerButton);
                    this.NextPageImage.sprite = XboxControllerConverter.GetXboxButtonSpite(this.Theme, this.NextPageControllerButton);
                    this.NextPageImage.SetNativeSize();
                }
                else {
                    this.NextPageImage.enabled = false;
                }

                if (this.PreviousPageControllerButton != XboxControllerButtons.None)
                {
                    this.prevControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.PreviousPageControllerButton);
                    this.PreviousPageImage.sprite = XboxControllerConverter.GetXboxButtonSpite(this.Theme, this.PreviousPageControllerButton);
                    this.PreviousPageImage.SetNativeSize();
                }
                else {
                    this.PreviousPageImage.enabled = false;
                }

                if (this.NextViewControllerButton != XboxControllerButtons.None)
                {
                    this.nextViewControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.NextViewControllerButton);
                    this.NextViewImage.sprite = XboxControllerConverter.GetXboxButtonSpite(this.Theme, this.NextViewControllerButton);
                    this.NextViewImage.SetNativeSize();
                }
                else
                {
                    this.NextViewImage.enabled = false;
                }

                if (this.PrevViewControllerButton != XboxControllerButtons.None)
                {
                    this.prevViewControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.PrevViewControllerButton);
                    this.PrevViewImage.sprite = XboxControllerConverter.GetXboxButtonSpite(this.Theme, this.PrevViewControllerButton);
                    this.PrevViewImage.SetNativeSize();
                }
                else
                {
                    this.PrevViewImage.enabled = false;
                }
            }
            else {
                this.NextViewImage.enabled = false;
                this.PrevViewImage.enabled = false;
                this.NextPageImage.enabled = false;
                this.PreviousPageImage.enabled = false;
            }
        }

        private void Start()
        {
            this.StartCoroutine(this.LoadTheme(this.Theme));

            this.ViewSelector.SelectFilter((int)this.viewFilter);
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
                if (this.currentPage != 0 && !string.IsNullOrEmpty(this.prevControllerButton) && Input.GetKeyDown(this.prevControllerButton))
                {
                    this.PreviousPage();
                }

                if (this.currentPage != this.totalPages && !string.IsNullOrEmpty(this.nextControllerButton) && Input.GetKeyDown(this.nextControllerButton))
                {
                    this.NextPage();
                }

                if (!string.IsNullOrEmpty(this.verticalScrollInputAxis) && Input.GetAxis(this.verticalScrollInputAxis) != 0)
                {
                    var inputValue = Input.GetAxis(this.verticalScrollInputAxis);
                    this.scrollRect.verticalScrollbar.value = this.scrollRect.verticalNormalizedPosition + inputValue * scrollSpeedMultiplier;
                }

                if (!string.IsNullOrEmpty(this.nextViewControllerButton) && Input.GetKeyDown(this.nextViewControllerButton))
                {
                    this.LoadView(((int) this.viewFilter) + 1);
                }

                if (!string.IsNullOrEmpty(this.prevViewControllerButton) && Input.GetKeyDown(this.prevViewControllerButton))
                {
                   this.LoadView(((int) this.viewFilter) - 1);
                }
            }
        }

        public void Refresh()
        {
            this.FirstPage();
        }

        public void NextPage()
        {
            if (this.currentPage + 1 < this.totalPages)
            {
                this.UpdateData(this.currentPage + 1, viewFilter);
            }
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

        public void LoadView(int newFilterNumber) {

            if (newFilterNumber >= Enum.GetNames(typeof(LeaderboardFilter)).Length)
            {
                newFilterNumber = 0;
            }

            if (newFilterNumber < 0)
            {
                newFilterNumber = Enum.GetNames(typeof(LeaderboardFilter)).Length - 1;
            }

            this.viewFilter = (LeaderboardFilter)newFilterNumber;

            this.Clear();
            this.UpdateData(0, this.viewFilter);
            this.ViewSelector.SelectFilter(newFilterNumber);
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
                socialGroup = LeaderboardHelper.GetSocialGroupFromLeaderboardType(this.leaderboardType);
                if (filter == LeaderboardFilter.Default)
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

            this.pageText.text = string.Format("{0} | {1}", displayCurrentPage, Mathf.Max(displayCurrentPage, this.totalPages)); 

            this.Clear();

            IList<string> xuids = new List<string>();
            
            var rowCount = 0;
            foreach (LeaderboardRow row in this.leaderboardData.Rows)
            {
                xuids.Add(row.XboxUserId);

                GameObject entryObject = this.entryObjectPool.GetObject();
                PlayerProfile entry = entryObject.GetComponent<PlayerProfile>();
                entry.Theme = this.Theme;
                entry.IsCurrentPlayer = this.xboxLiveUser != null && row.Gamertag.Equals(this.xboxLiveUser.Gamertag);
                entry.BackgroundColor = ((rowCount % 2 == 0) ? PlayerProfileBackgrounds.RowBackground02 : PlayerProfileBackgrounds.RowBackground01);
                entry.UpdateGamerTag(row.Gamertag);
                entry.UpdateRank(true, row.Rank);
                if (row.Values != null && row.Values.Count > 0)
                {
                    entry.UpdateScore(true, row.Values[0]);
                }
                this.StartCoroutine(entry.Reload());
                entryObject.transform.SetParent(this.contentPanel);
                this.currentEntries.Add(entry);
                rowCount++;
                
                entryObject.transform.localScale = Vector3.one;
            }

            if (xuids.Count > 0)
            {
                userGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromList(this.xboxLiveUser, xuids);
            }

            // Reset the scroll view to the top.
            this.scrollRect.verticalNormalizedPosition = 1;
            this.UpdateButtons();
        }

        public void UpdateButtons()
        {
            this.PreviousPageButton.interactable = this.currentPage != 0;
            this.NextPageButton.interactable = this.totalPages > 1 && this.currentPage < this.totalPages - 1;
        }

        private void Clear() {
            var children = new List<GameObject>();
            for (int i = 0; i < this.contentPanel.childCount; i++)
            {
                GameObject child = this.contentPanel.transform.GetChild(i).gameObject;
                children.Add(child);
            }

            this.currentEntries.Clear();
            this.contentPanel.DetachChildren();

            foreach (var child in children)
            {
                Destroy(child);
            }
        }

        private IEnumerator LoadTheme(Theme theme) {
            yield return null;
            this.HeaderRowImage.sprite = ThemeHelper.LoadSprite(this.Theme, "LabelRowBackground");
            this.BackgroundImage.color = ThemeHelper.GetThemeBackgroundColor(this.Theme);
            var fontColor = ThemeHelper.GetThemeBaseFontColor(this.Theme);
            this.HeaderText.color = fontColor;
            this.pageText.color = ThemeHelper.GetThemeHighlightColor(this.Theme);
            this.NextPageText.color = fontColor;
            this.PreviousPageText.color = fontColor;
            var lineSprite = ThemeHelper.LoadSprite(this.Theme, "Line");
            this.BottomLine.sprite = lineSprite;
            this.TopLine.sprite = lineSprite;
        }

        private void HandleScrolling(int newHighlightedPosition, int oldHighlightedPosition) {

            this.currentEntries[oldHighlightedPosition].IsHighlighted = false;
            this.StartCoroutine(this.currentEntries[oldHighlightedPosition].Reload());


            this.currentEntries[newHighlightedPosition].IsHighlighted = true;
            this.StartCoroutine(this.currentEntries[newHighlightedPosition].Reload());
        }

        private void SetupForAddingUser() {
            this.UpdateButtons();
            SocialManagerComponent.Instance.EventProcessed += this.SocialManagerEventProcessed;
            StatsManagerComponent.Instance.LocalUserAdded += this.LocalUserAdded;
            StatsManagerComponent.Instance.GetLeaderboardCompleted += this.GetLeaderboardCompleted;
            SignInManager.Instance.OnPlayerSignOut(this.PlayerNumber, this.OnPlayerSignOut);
            this.statsAddedLocalUser = false;
            this.socialAddedLocalUser = false;
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

        private void OnEnable()
        {
            this.SetupForAddingUser();

            this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            if (this.xboxLiveUser != null)
            {
                this.statsAddedLocalUser = true;
                this.socialAddedLocalUser = true;
                this.Refresh();
            }
        }

        private void OnDisable()
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
        Default,
        NearestMe
    }
}