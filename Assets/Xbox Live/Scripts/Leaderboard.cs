// -----------------------------------------------------------------------
//  <copyright file="Leaderboard.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Threading.Tasks;

using Microsoft.Xbox.Services.Leaderboard;

using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Leaderboard : MonoBehaviour
{
    public string leaderboardName;
    public string displayName;

    [Range(1, 100)]
    public uint entryCount = 10;

    public uint currentPage = 0;

    public uint totalPages;

    private TaskYieldInstruction<LeaderboardResult> leaderboardData;

    public Text headerText;
    public Text pageText;

    public Button firstButton;
    public Button previousButton;
    public Button nextButton;
    public Button lastButton;

    public Transform contentPanel;

    private ObjectPool entryObjectPool;

    public void Awake()
    {
        XboxLive.EnsureEnabled();
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
        this.firstButton.interactable = this.previousButton.interactable = XboxLive.IsEnabled && this.currentPage != 0;
        this.nextButton.interactable = this.lastButton.interactable = XboxLive.IsEnabled && this.totalPages > 1 && this.currentPage < this.totalPages - 1;
    }
}