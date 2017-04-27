// -----------------------------------------------------------------------
//  <copyright file="Leaderboard.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;

using Microsoft.Xbox.Services.CSharp;
using Microsoft.Xbox.Services.CSharp.Leaderboard;
using Microsoft.Xbox.Services.CSharp.Stats.Manager;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Leaderboard : MonoBehaviour
{
    public StatBase stat;
    public bool isConfigured;

    [Tooltip("This property needs to be set to get an unconfigured leaderboard. Example: \"all\"")]
    public string socialGroup;

    [Range(1, 100)]
    public uint entryCount = 10;

    [HideInInspector]
    public uint currentPage;

    [HideInInspector]
    public uint totalPages;

    [HideInInspector]
    public Text headerText;

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

    [HideInInspector]
    public Transform contentPanel;

    [HideInInspector]
    public ScrollRect scrollRect;

    private LeaderboardResult leaderboardData;
    private ObjectPool entryObjectPool;
    private bool isLocalUserAdded;

    private void Awake()
    {
        this.EnsureEventSystem();

        if (this.stat == null)
        {
            Debug.LogFormat("Leaderboard '{0}' does not have a stat configured and will not function properly.", this.name);
            return;
        }

        this.headerText.text = this.stat.DisplayName;
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        this.UpdateButtons();
        StatsManagerComponent.Instance.LocalUserAdded += this.LocalUserAdded;
        StatsManagerComponent.Instance.GetLeaderboardCompleted += this.GetLeaderboardCompleted;
        this.isLocalUserAdded = false;
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
        this.UpdateData(this.currentPage - 1);
    }

    public void FirstPage()
    {
        this.UpdateData(0);
    }

    public void LastPage()
    {
        this.UpdateData(this.totalPages - 1);
    }

    private void UpdateData(uint newPage)
    {
        if (!this.isLocalUserAdded) return;
        if (this.stat == null) return;

        if (this.isConfigured && string.IsNullOrEmpty(this.socialGroup))
        {
            throw new InvalidOperationException("If you are using a configured leaderboard you must specify a social group.");
        }

        LeaderboardQuery query;
        if (newPage == this.currentPage + 1 && this.leaderboardData != null && this.leaderboardData.HasNext)
        {
            query = this.leaderboardData.NextQuery;
        }
        else
        {
            query = new LeaderboardQuery
            {
                StatName = this.stat.Name,
                SocialGroup = this.socialGroup,
                SkipResultsToRank = this.currentPage * this.entryCount,
                MaxItems = this.entryCount,
            };
        }

        this.currentPage = newPage;
        XboxLive.Instance.StatsManager.GetLeaderboard(XboxLiveComponent.Instance.User, query);
    }

    private void LocalUserAdded(object sender, XboxLiveUserEventArgs e)
    {
        this.isLocalUserAdded = true;
    }

    private void GetLeaderboardCompleted(object sender, XboxLivePrefab.StatEventArgs e)
    {
        if (e.EventData.ErrorInfo != null) return;

        LeaderboardResultEventArgs leaderboardArgs = (LeaderboardResultEventArgs)e.EventData.EventArgs;
        this.LoadResult(leaderboardArgs.Result);
    }

    /// <summary>
    /// Load the leaderboard result data from the service into the view.
    /// </summary>
    /// <param name="result"></param>
    private void LoadResult(LeaderboardResult result)
    {
        if (this.stat == null || this.stat.Name != result.NextQuery.StatName || this.socialGroup != result.NextQuery.SocialGroup) return;

        this.leaderboardData = result;

        if (this.totalPages == 0)
        {
            this.totalPages = (this.leaderboardData.TotalRowCount - 1) / this.entryCount + 1;
        }

        this.pageText.text = string.Format("Page: {0} / {1}", this.currentPage + 1, this.totalPages);

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