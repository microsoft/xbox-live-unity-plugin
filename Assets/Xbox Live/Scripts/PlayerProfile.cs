using Microsoft.Xbox.Services.Social.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    public class PlayerProfile : MonoBehaviour
    {
        public Themes Theme = Themes.Light;
        public PlayerProfileBackgrounds BackgroundColor = PlayerProfileBackgrounds.RowBackground01;
        public bool IsHighlighted = false;
        public bool IsCurrentPlayer = false;
        public bool ShowScore = false;
        public bool ShowRank = false;
        public Image ProfileBackgroundImage;
        public Text GamerTagText;
        public Image GamerPicImage;
        public Image ScoreBackgroundImage;
        public Image RankBackgroundImage;
        public Image CurrentPlayerIndicator;
        public Text ScoreText;
        public Text RankText;

        private XboxSocialUser xboxLiveSocialUser;

        private void Awake()
        {
            this.StartCoroutine(this.Reload());
        }

        public void UpdateGamerTag(string gamerTag) {
            this.GamerTagText.text = gamerTag;
        }

        public void UpdateScore(bool enableScore, string score) {
            this.ShowScore = enableScore;
            if (this.ShowScore) {
                this.ScoreText.text = score;
            }
        }

        public void UpdateRank(bool enableRank, uint rank)
        {
            this.ShowRank = enableRank;
            if (this.ShowRank)
            {
                this.RankText.text = "" + rank;
            }
        }

        public IEnumerator Reload()
        {
            yield return null;

            Sprite backgroundImageToUse  = null;
            if (!IsHighlighted)
            {
                backgroundImageToUse = ThemeHelper.LoadSprite(this.Theme, this.BackgroundColor.ToString());
            }
            else {
                backgroundImageToUse = ThemeHelper.LoadSprite(this.Theme, "RowBackground-Highlighted");
            }

            if (IsCurrentPlayer)
            {
                this.CurrentPlayerIndicator.enabled = true;
                this.CurrentPlayerIndicator.sprite = ThemeHelper.LoadSprite(this.Theme, "HereMarker");
            }
            else
            {
                this.CurrentPlayerIndicator.enabled = false;
            }
            this.ProfileBackgroundImage.sprite = backgroundImageToUse;
            this.RankBackgroundImage.sprite = backgroundImageToUse;
            this.ScoreBackgroundImage.sprite = backgroundImageToUse;

            this.RankBackgroundImage.enabled = this.ShowRank;
            this.RankText.enabled = this.ShowRank;
            this.ScoreBackgroundImage.enabled = this.ShowScore;
            this.ScoreText.enabled = this.ShowScore;
        }

        public IEnumerator LoadGamerpic(string gamerpicUrl)
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
                this.GamerPicImage.color = Color.white;
                this.GamerPicImage.sprite = Sprite.Create(t, r, Vector2.zero);
            }
            catch (Exception ex)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.Log("There was an error while loading " + this.GamerTagText.text + "'s gamerpic. Exception: " + ex.Message);
                }
            }
        }


    }
}