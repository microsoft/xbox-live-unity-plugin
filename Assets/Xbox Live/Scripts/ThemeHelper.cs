using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public enum Theme 
    {
        Light,
        Dark
    }

    public enum PlayerProfileBackgrounds {
        RowBackground01,
        RowBackground02
    }

    /// <summary>
    /// Used to assist with loading the correct sprites from Resources
    /// </summary>
    public class ThemeHelper: MonoBehaviour {
        private static Color[] BackgroundColors;
        private static Color[] HighlightColors;
        private static Color[] BaseFontColors;
        private static Color[] ProfileFontColors;

        private void Awake()
        {
            BackgroundColors = new Color[] {
                new Color(236.0f / 255.0f, 240.0f / 255.0f, 241.0f / 255.0f),
                new Color( 40.0f / 255.0f, 39.0f / 255.0f, 38.0f / 255.0f)
            };

            HighlightColors = new Color[] {
                new Color( 206.0f / 255.0f, 61.0f / 255.0f, 54.0f / 255.0f),
                new Color( 133.0f / 255.0f, 186.0f / 255.0f, 58.0f / 255.0f)
            };

            BaseFontColors = new Color[] {
                new Color(97.0f / 255.0f, 108.0f / 255.0f, 108.0f / 255.0f),
                new Color(155.0f / 255.0f, 183.0f / 255.0f, 204.0f / 255.0f)
            };

            ProfileFontColors = new Color[] {
                new Color(236.0f / 255.0f, 240.0f / 255.0f, 241.0f / 255.0f),
                new Color(155.0f / 255.0f, 183.0f / 255.0f, 204.0f / 255.0f)
            };
        }

        public static Sprite LoadSprite(Theme theme, string spriteName) {
            try
            {
                var spritePath = theme + "/" + spriteName;
                var result = Resources.Load<Sprite>(spritePath);
                return result;
            }
            catch (Exception ex) {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn) {
                    Debug.LogError("An exception occured in LoadSprite: " + ex.Message);
                }
            }

            return null;
        }

        public static Color GetThemeHighlightColor(Theme theme) {
            return HighlightColors[(int)theme];
        }

        public static Color GetThemeBackgroundColor(Theme theme) {
            return BackgroundColors[(int)theme];
        }

        public static Color GetThemeBaseFontColor(Theme theme) {
            return BaseFontColors[(int)theme];
        }

        public static Color GetThemeProfileFontColor(Theme theme)
        {
            return ProfileFontColors[(int)theme];
        }
    }
}
