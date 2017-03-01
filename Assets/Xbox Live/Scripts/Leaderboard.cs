// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections;
using System.Threading.Tasks;

using Microsoft.Xbox.Services.Leaderboard;
using Microsoft.Xbox.Services.Stats.Manager;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Leaderboard : UIMonoBehaviour
{
    public StatBase Stat;
    public string displayName;
    public bool isConfigured;

    [Tooltip("This property needs to be set to get an unconfigured leaderboard. Example: \"all\"")]
    public string socialGroup;

    [Range(1, 100)]
    public uint entryCount = 10;

    [HideInInspector]
    public uint currentPage = 0;

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

    private LeaderboardResult leaderboardData;
    private ObjectPool entryObjectPool;
    private bool isLocalUserAdded;

    private void Awake()
    {
        this.EnsureEventSystem();

        XboxLive.EnsureConfigured();
        this.headerText.text = this.displayName;
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        this.UpdateButtons();
        StatsManagerComponent.Instance.LocalUserAdded += LocalUserAdded;
        StatsManagerComponent.Instance.GetLeaderboardCompleted += GetLeaderboardCompleted;
        isLocalUserAdded = false;
    }

    private void GetLeaderboardCompleted(object sender, XboxLivePrefab.StatEventArgs e)
    {
        if (e.EventData.ErrorInfo == null)
        {
            LeaderboardResultEventArgs leaderboardArgs = (LeaderboardResultEventArgs)e.EventData.EventArgs;
            UpdateData(leaderboardArgs.Result);
        }
    }

    private void LocalUserAdded(object sender, XboxLiveUserEventArgs e)
    {
        isLocalUserAdded = true;
    }

    public void Refresh()
    {
        this.FirstPage();
    }

    public void NextPage()
    {
        if (this.leaderboardData != null && this.leaderboardData.NextQuery != null && this.leaderboardData.NextQuery.HasNext)
        {
            LeaderboardQuery query = this.leaderboardData.NextQuery;
            this.currentPage++;
            if(isLocalUserAdded)
            {
                if(isConfigured || string.IsNullOrEmpty(socialGroup))
                {
                    StatsManager.Singleton.GetLeaderboard(XboxLive.Instance.User, query.StatName, query);
                }
                else
                {
                    StatsManager.Singleton.GetSocialLeaderboard(XboxLive.Instance.User, query.StatName, socialGroup, query);
                }
            }
        }
    }

    public void PreviousPage()
    {
        this.currentPage--;
        this.UpdateData();
    }

    public void FirstPage()
    {
        this.currentPage = 0;
        this.UpdateData();
    }

    public void LastPage()
    {
        this.currentPage = this.totalPages - 1;
        this.UpdateData();
    }

    private void UpdateData()
    {
        LeaderboardQuery query = new LeaderboardQuery
        {
            SkipResultsToRank = this.currentPage * this.entryCount,
            MaxItems = this.entryCount,
        };
        if(isLocalUserAdded && Stat != null)
        {
            if (isConfigured || string.IsNullOrEmpty(socialGroup))
            {
                StatsManager.Singleton.GetLeaderboard(XboxLive.Instance.User, Stat.Name, query);
            }
            else
            {
                StatsManager.Singleton.GetSocialLeaderboard(XboxLive.Instance.User, Stat.Name, socialGroup, query);
            }
        }
    }

    private void UpdateData(LeaderboardResult data)
    {
        if (Stat != null && 
            (data.NextQuery == null || 
            (data.NextQuery != null && data.NextQuery.StatName == Stat.Name)))
        {
            this.leaderboardData = data;

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
            this.UpdateButtons();
        }
    }

    public void UpdateButtons()
    {
        this.firstButton.interactable = this.previousButton.interactable = XboxLive.IsConfigured && this.currentPage != 0;
        this.nextButton.interactable = this.lastButton.interactable = XboxLive.IsConfigured && this.totalPages > 1 && this.currentPage < this.totalPages - 1;
    }
}