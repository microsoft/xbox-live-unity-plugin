using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    public enum Themes 
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
        public static Sprite LoadSprite(Themes theme, string spriteName) {
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
    }
}
