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
        RightBumper,
        BackButton,
        StartButton,
        LeftStickClick,
        RightStickClick
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
                case XboxControllerButtons.BackButton: return 6;
                case XboxControllerButtons.StartButton: return 7;
                case XboxControllerButtons.LeftStickClick: return 8;
                case XboxControllerButtons.RightStickClick: return 9;
                case XboxControllerButtons.None:
                default: return -1;
            }
        }
    }
}