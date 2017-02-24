// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Collections;
using System.Threading.Tasks;

using Microsoft.Xbox.Services.Leaderboard;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Leaderboard : UIMonoBehaviour
{
    public string leaderboardName;
    public string displayName;

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

    private TaskYieldInstruction<LeaderboardResult> leaderboardData;
    private ObjectPool entryObjectPool;

    private void Awake()
    {
        this.EnsureEventSystem();

        XboxLive.EnsureConfigured();
        this.headerText.text = this.displayName;
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        this.UpdateButtons();
    }

    public void Refresh()
    {
        this.FirstPage();
    }

    public void NextPage()
    {
        this.currentPage++;
        this.UpdateData(this.leaderboardData.Result.GetNextAsync(this.entryCount));
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

        this.UpdateData(XboxLive.Instance.Context.LeaderboardService.GetLeaderboardAsync(this.leaderboardName, query));
    }

    private void UpdateData(Task<LeaderboardResult> task)
    {
        this.StartCoroutine(this.UpdateData(task.AsCoroutine()));
    }

    private IEnumerator UpdateData(TaskYieldInstruction<LeaderboardResult> data)
    {
        this.leaderboardData = data;
        yield return this.leaderboardData;

        if (this.totalPages == 0)
        {
            this.totalPages = (this.leaderboardData.Result.TotalRowCount - 1) / this.entryCount + 1;
        }

        this.pageText.text = string.Format("Page: {0} / {1}", this.currentPage + 1, this.totalPages);

        while (this.contentPanel.childCount > 0)
        {
            var entry = this.contentPanel.GetChild(0).gameObject;
            this.entryObjectPool.ReturnObject(entry);
        }

        foreach (LeaderboardRow row in this.leaderboardData.Result.Rows)
        {
            GameObject entryObject = this.entryObjectPool.GetObject();
            LeaderboardEntry entry = entryObject.GetComponent<LeaderboardEntry>();

            entry.Data = row;

            entryObject.transform.SetParent(this.contentPanel);
        }

        this.UpdateButtons();
    }

    public void UpdateButtons()
    {
        this.firstButton.interactable = this.previousButton.interactable = XboxLive.IsConfigured && this.currentPage != 0;
        this.nextButton.interactable = this.lastButton.interactable = XboxLive.IsConfigured && this.totalPages > 1 && this.currentPage < this.totalPages - 1;
    }
}