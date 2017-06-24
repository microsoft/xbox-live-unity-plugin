// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Leaderboard;
using Microsoft.Xbox.Services.Statistics.Manager;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Leaderboard : MonoBehaviour
{
    private string socialGroup;

    public StatBase stat;

    public LeaderboardTypes leaderboardType;

    [Range(1, 100)]
    public uint entryCount = 10;

    public Text headerText;

    [HideInInspector]
    public uint currentPage;

    [HideInInspector]
    public uint totalPages;

    [HideInInspector]
    public Text pageText;

    [HideInInspector]
    public Button firstButton;

    [HideInInspector]
    public Button previousButton;

    [HideInInspector]
    public Button nextButton;

    [HideInInspector]
    public Button lastButton;

    public string firstControllerButton;

    public string lastControllerButton;

    public string nextControllerButton;

    public string prevControllerButton;

    public string refreshControllerButton;

    public string verticalScrollInputAxis;

    public Transform contentPanel;

    public ScrollRect scrollRect;

    public XboxLiveUserInfo XboxLiveUser;

    public float scrollSpeedMultiplier = 0.1f;

    private LeaderboardResult leaderboardData;
    private ObjectPool entryObjectPool;
    private bool isLocalUserAdded;

    private void Awake()
    {
        this.EnsureEventSystem();
        XboxLiveServicesSettings.EnsureXboxLiveDebugManager();

        if (this.stat == null)
        {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogFormat("Leaderboard '{0}' does not have a stat configured and will not function properly.", this.name);
            }
            return;
        }

        this.headerText.text = this.stat.DisplayName;
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        this.UpdateButtons();
        StatsManagerComponent.Instance.LocalUserAdded += this.LocalUserAdded;
        StatsManagerComponent.Instance.GetLeaderboardCompleted += this.GetLeaderboardCompleted;
        this.isLocalUserAdded = false;
    }

    private void Start()
    {
        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }
    }

    public void RequestFlushToService(bool isHighPriority)
    {
        StatsManagerComponent.Instance.RequestFlushToService(this.XboxLiveUser.User, isHighPriority);
    }

    void Update()
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

    public void Refresh()
    {
        this.FirstPage();
    }

    public void NextPage()
    {
        this.UpdateData(this.currentPage + 1);
    }

    public void PreviousPage()
    {
        if (this.currentPage > 0)
        {
            this.UpdateData(this.currentPage - 1);
        }
    }

    public void FirstPage()
    {
        this.UpdateData(0);
    }

    public void LastPage()
    {
        this.UpdateData(this.totalPages);
    }

    private void UpdateData(uint newPage)
    {
        if (!this.isLocalUserAdded)
        {
            return;
        }

        if (this.stat == null)
        {
            return;
        }

        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }

        LeaderboardQuery query;
        if (newPage == this.currentPage + 1 && this.leaderboardData != null && this.leaderboardData.HasNext)
        {
            query = this.leaderboardData.NextQuery;
        }
        else
        {
            switch (leaderboardType)
            {
                case LeaderboardTypes.Global:
                    socialGroup = null;
                    break;
                case LeaderboardTypes.Favorites:
                    socialGroup = "favorite";
                    break;
                case LeaderboardTypes.Friends:
                    socialGroup = "all";
                    break;
            }

            query = new LeaderboardQuery
            {
                StatName = this.stat.ID,
                SocialGroup = socialGroup,
                SkipResultsToRank = newPage == 0 ? 0 : (this.currentPage * this.entryCount) - 1,
                MaxItems = this.entryCount,
            };

            // Handle last page
            if (this.totalPages > 0 && newPage == this.totalPages)
            {
                query.SkipResultsToRank = (newPage * this.entryCount) - 1;
                newPage -= 1;
            }
        }

        this.currentPage = newPage;
        XboxLive.Instance.StatsManager.GetLeaderboard(this.XboxLiveUser.User, query);
    }

    private void LocalUserAdded(object sender, XboxLiveUserEventArgs e)
    {
        this.isLocalUserAdded = true;
        this.Refresh();
    }

    private void GetLeaderboardCompleted(object sender, XboxLivePrefab.StatEventArgs e)
    {
        if (e.EventData.ErrorInfo != null)
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
        if (this.stat == null || this.stat.ID != result.NextQuery.StatName || this.socialGroup != result.NextQuery.SocialGroup)
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
            this.totalPages = (this.leaderboardData.TotalRowCount - 1) / (this.entryCount + 1);
        }

        this.pageText.text = string.Format("Page: {0} / {1}", displayCurrentPage, this.totalPages);

        while (this.contentPanel.childCount > 0)
        {
            var entry = this.contentPanel.GetChild(0).gameObject;
            this.entryObjectPool.ReturnObject(entry);
        }

        foreach (LeaderboardRow row in this.leaderboardData.Rows)
        {
            GameObject entryObject = this.entryObjectPool.GetObject();
            LeaderboardEntry entry = entryObject.GetComponent<LeaderboardEntry>();

            entry.Data = row;

            entryObject.transform.SetParent(this.contentPanel);
        }

        // Reset the scroll view to the top.
        this.scrollRect.verticalNormalizedPosition = 1;
        this.UpdateButtons();
    }

    public void UpdateButtons()
    {
        this.firstButton.interactable = this.previousButton.interactable = this.currentPage != 0;
        this.nextButton.interactable = this.lastButton.interactable = this.totalPages > 1 && this.currentPage < this.totalPages - 1;
    }
}