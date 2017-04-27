// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using Microsoft.Xbox.Services.CSharp.Leaderboard;

using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public Button entryButton;
    public Image gamerpicImage;
    public Text rankText;
    public Text gamertagText;
    public Text valueText;

    private LeaderboardRow data;

    public LeaderboardRow Data
    {
        get
        {
            return this.data;
        }
        set
        {
            this.data = value;
            this.gamertagText.text = this.data.Gamertag;
            this.rankText.text = this.data.Rank.ToString();
            this.valueText.text = this.data.Values[0];
        }
    }
}