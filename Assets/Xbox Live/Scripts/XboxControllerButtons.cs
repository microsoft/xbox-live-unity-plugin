using UnityEngine;

namespace Microsoft.Xbox.Services.Client
{
    /// <summary>
    /// Enum used for Xbox Controller Buttons
    /// </summary>
    public enum XboxControllerButtons
    {
        None,
        A,
        B,
        X,
        Y,
        LeftBumper,
        RightBumper
    }

    public class XboxControllerConverter
    {
        public static int GetUnityButtonNumber(XboxControllerButtons xboxButton)
        {
            switch (xboxButton)
            {
                case XboxControllerButtons.A: return 0;
                case XboxControllerButtons.B: return 1;
                case XboxControllerButtons.X: return 2;
                case XboxControllerButtons.Y: return 3;
                case XboxControllerButtons.LeftBumper: return 4;
                case XboxControllerButtons.RightBumper: return 5;
                case XboxControllerButtons.None:
                default: return -1;
            }
        }

        public static Sprite GetXboxButtonSpite (Theme theme, XboxControllerButtons xboxButton) {
            var iconName = string.Empty;
            switch (xboxButton) {
                case XboxControllerButtons.A:
                case XboxControllerButtons.B:
                case XboxControllerButtons.X:
                case XboxControllerButtons.Y:
                    iconName = iconName + xboxButton; break;
                case XboxControllerButtons.LeftBumper: iconName = "LB"; break;
                case XboxControllerButtons.RightBumper: iconName = "RB"; break;
                default: iconName = string.Empty; break;
            }

            if (string.IsNullOrEmpty(iconName)) {
                return null;
            }
            else {
                return ThemeHelper.LoadSprite(theme, "Controller/" + iconName);
            }
        }
    }
} 