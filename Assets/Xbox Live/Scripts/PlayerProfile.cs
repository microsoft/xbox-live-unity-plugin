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
        [Header("Profile Settings")]
        public bool IsHighlighted = false;
        public bool IsCurrentPlayer = false;
        public bool ShowScore = false;
        public bool ShowRank = false;

        [Header("Theme Settings")]
        public Theme Theme = Theme.Light;
        public PlayerProfileBackgrounds BackgroundColor = PlayerProfileBackgrounds.RowBackground01;

        [Header("UI Components")]
        public Image ProfileBackgroundImage;
        public Text GamerTagText;
        public Image GamerPicImage;
        public Image ScoreBackgroundImage;
        public Image RankBackgroundImage;
        public Image CurrentPlayerIndicator;
        public Text ScoreText;
        public Text RankText;
        public Image GamerPicMask;

        private static Color[] lightThemeBackgroundColors;
        private static Color[] darkThemeBackgroundColors;

        private void Awake()
        {
            lightThemeBackgroundColors = new Color[] {
                new Color(46.0f / 255.0f, 138.0f / 255.0f, 170.0f / 255.0f),
                new Color(39.0f / 255.0f, 113.0f / 255.0f, 130.0f / 255.0f) };
            darkThemeBackgroundColors = new Color[] {
                new Color(43.0f / 255.0f, 78.0f / 255.0f, 114.0f / 255.0f),
                new Color(31.0f / 255.0f, 53.0f / 255.0f, 68.0f / 255.0f) };

            this.ProfileBackgroundImage.enabled = false;
            this.GamerPicImage.enabled = false;
            this.RankBackgroundImage.enabled = false;
            this.ScoreBackgroundImage.enabled = false;
            this.ScoreText.enabled = false;
            this.RankText.enabled = false;
            this.CurrentPlayerIndicator.enabled = false;
            this.GamerTagText.enabled = false;
            this.GamerPicMask.enabled = false;
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

            var fontColor = this.IsHighlighted? Color.white: ThemeHelper.GetThemeProfileFontColor(this.Theme);
            Sprite backgroundImageToUse  = null;
            if (!IsHighlighted)
            {
                backgroundImageToUse = ThemeHelper.LoadSprite(this.Theme, this.BackgroundColor.ToString());
                switch (this.Theme) {
                    case Theme.Light: this.GamerPicMask.color = lightThemeBackgroundColors[(int)this.BackgroundColor]; break;
                    case Theme.Dark: this.GamerPicMask.color = darkThemeBackgroundColors[(int)this.BackgroundColor]; break;
                }
            }
            else {
                backgroundImageToUse = ThemeHelper.LoadSprite(this.Theme, "RowBackground-Highlighted");
                this.GamerPicMask.color = ThemeHelper.GetThemeHighlightColor(this.Theme); 
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

            this.GamerTagText.color = fontColor;
            this.ScoreText.color = fontColor;
            this.RankText.color = fontColor;

            this.ProfileBackgroundImage.sprite = backgroundImageToUse;
            this.RankBackgroundImage.sprite = backgroundImageToUse;
            this.ScoreBackgroundImage.sprite = backgroundImageToUse;
            
            this.RankBackgroundImage.enabled = this.ShowRank;
            this.RankText.enabled = this.ShowRank;
            this.ScoreBackgroundImage.enabled = this.ShowScore;
            this.ScoreText.enabled = this.ShowScore;

            this.ProfileBackgroundImage.enabled = true;
            this.GamerPicMask.enabled = true;
            this.GamerPicImage.enabled = true;
            this.GamerTagText.enabled = true;
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