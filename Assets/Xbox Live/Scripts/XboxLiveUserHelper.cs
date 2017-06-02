using System.Collections.Generic;

public class XboxLiveUserHelper : Singleton<XboxLiveUserHelper>
{

    public List<XboxLiveUserInfo> XboxLiveUsersInTheScene { get; private set; }

    public XboxLiveUserInfo SingleXboxLiveUser { get; set; }

    public bool SingleUserModeEnabled { get; private set; }

    public bool IsInitialized { get; private set; }

    // Use this for initialization
    public void Initialize()
    {
        this.XboxLiveUsersInTheScene = new List<XboxLiveUserInfo>();
        var xboxLiveUserInstances = FindObjectsOfType<XboxLiveUserInfo>();
        foreach (var xboxLiveUserInstance in xboxLiveUserInstances)
        {
            this.XboxLiveUsersInTheScene.Add(xboxLiveUserInstance);
        }

        this.SingleUserModeEnabled = (xboxLiveUserInstances.Length == 0) || (xboxLiveUserInstances.Length == 1);

        this.IsInitialized = true;
    }

}
