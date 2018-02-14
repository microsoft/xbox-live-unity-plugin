// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using Microsoft.Xbox.Services.Leaderboard;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
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
                this.gamerpicImage.sprite = null;
            }
        }

        string gamerpicUrl;
        public string GamerpicUrl
        {
            get
            {
                return this.gamerpicUrl;
            }
            set
            {
                this.gamerpicUrl = value;
                StartCoroutine(LoadGamerpic());
            }
        }
        IEnumerator LoadGamerpic()
        {
            var www = new WWW(gamerpicUrl);
            yield return null;

            while (!www.isDone)
            {
                yield return new WaitForSeconds(0.2f);
            }

            try
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    throw new Exception(www.error);
                }

                var t = www.texture;
                var r = new Rect(0, 0, t.width, t.height);
                this.gamerpicImage.color = Color.white;
                this.gamerpicImage.sprite = Sprite.Create(t, r, Vector2.zero);
            }
            catch (Exception ex)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.Log("There was an error while loading " + gamertagText.text + "'s gamerpic. Exception: " + ex.Message);
                }
            }
        }
    }
}