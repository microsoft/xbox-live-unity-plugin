using Microsoft.Xbox.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignInManager : MonoBehaviour {

    private Dictionary<int, XboxLivePlayerInfo> CurrentPlayers;

    public void Awake()
    {
        CurrentPlayers = new Dictionary<int, XboxLivePlayerInfo>();
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        // Super simple check to determine if configuration is non-empty.  This is not a thorough check to determine if the configuration is valid.
        // A user can easly bypass this check which will just cause them to fail at runtime if they try to use any functionality.
        if (XboxLive.Instance.AppConfig == null || XboxLive.Instance.AppConfig.ServiceConfigurationId == null && Application.isPlaying)
        {
            const string message = "Xbox Live is not configured, but the game is attempting to use Xbox Live functionality.  You must update the configuration in 'Xbox Live > Configuration' before building the game to enable Xbox Live.";
            if (Application.isEditor && XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogWarning(message);
            }
            else
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(message);
                }
            }
        }
    }

    /// <summary>
    /// Adds and signs in a new Xbox Live User and assigns it a player number.
    /// Note: Different platforms support a different number of users. 
    /// AddUser might fail if the player number is not within the range of supported users.
    /// </summary>
    /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
    IEnumerator AddUser(int playerNumber)
    {
        yield return null;
        if (ValidatePlayerNumber(playerNumber, "Add User")) {
            if (!CurrentPlayers.ContainsKey(playerNumber)) {
                var playerInfo = new XboxLivePlayerInfo() {
                    SignInCallbacks = new List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>>(),
                    SignOutCallbacks = new List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>>()
                };
                CurrentPlayers.Add(playerNumber, playerInfo);
            }

            if (CurrentPlayers[playerNumber].XboxLiveUser != null) {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError("Add User Failed");
                }
            }
        }
    }

    /// <summary>
    /// Removes and signs out the Xbox Live User with the assigned player number.
    /// Note: Sign out might not be supported on some platforms.
    /// </summary>
    /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
    IEnumerator RemoveUser(int playerNumber) {
        yield return null;
        
    }

    /// <summary>
    /// Signs out the Xbox Live User with the assigned player number and adds and signs in a new Xbox Live User.
    /// Note: Switching users might not be supported on some platforms.
    /// </summary>
    /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
    IEnumerator SwitchUser(int playerNumber) {
        yield return null;
       
    }

    /// <summary>
    /// Returns the Xbox Live User with the assigned player number if they exist. If no user is signed in for that player number, null is returned.
    /// </summary>
    /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
    /// <returns>The Xbox Live User  with the assigned player number if exists. Null, otherwise.</returns>
    XboxLiveUser GetUser(int playerNumber) { return null; }

    /// <summary>
    /// Returns the Xbox Live User with the assigned xuid if they exist. If no user is signed in for that player number, null is returned.
    /// </summary>
    /// <param name="xuid">The Xuid of the requested Xbox Live User</param>
    /// <returns>The Xbox Live User if exists. Null, otherwise.</returns>
    XboxLiveUser GetUser(string xuid) { return null; }

    /// <summary>
    /// Adds a callback method for when the Xbox Live User with player number signs in.
    /// </summary>
    /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
    /// <param name="callback">A callback method that will be called when the Xbox Live User is signed in. 
    /// The callback method should be using three inputs of type: 
    /// - <see cref="XboxLiveUser"/> which would be the signed in user or null if the sign in failed. 
    /// - <see cref="XboxLiveAuthStatus"/> which is the status of sign in: Succeeded, Failed and Unsupported.
    /// - A string which will contain the error message if sign in failed.</param>
    /// <returns>True if callback was added successfully, false otherwise.</returns>
    bool OnPlayerSignIn(int playerNumber, UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback) { return false; }

    /// <summary>
    /// Adds a callback method for when the Xbox Live User with player number signs out.
    /// </summary>
    /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
    /// <param name="callback">A callback method that will be called when the Xbox Live User is signed out. 
    /// The callback method should be using three inputs of type: 
    /// - <see cref="XboxLiveUser"/> which would be the signed in user or null if the sign out failed. 
    /// - <see cref="XboxLiveAuthStatus"/> which is the status of sign out: Succeeded, Failed and Unsupported.
    /// - A string which will contain the error message if sign out failed.</param>
    /// <returns>True if callback was added successfully, false otherwise.</returns>
    bool OnPlayerSignOut(int playerNumber, UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback) { return false; }
    
    /// <summary>
    /// Returns a list of the currently signed in Xbox Live Users and their assigned player numbers
    /// </summary>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with player number keys and Xbox Live User values.</returns>
    Dictionary<int, XboxLiveUser> GetCurrentPlayers() { return null; }

    /// <summary>
    /// Returns the number of signed in Xbox Live Users.
    /// </summary>
    /// <returns>An integer representing the number of currently signed in players.</returns>
    int GetCurrentNumberOfPlayers() { return 0; }

    /// <summary>
    /// Returns the maximum number of signed in Xbox Live Users supported on current platform.
    /// </summary>
    /// <returns>An integer representing the maximum number of players supported on the current platform.</returns>
    int GetMaximumNumberOfPlayers() {
        if (Application.isEditor) {
            return 16;
        }
        else
        {
#if CONSOLE
            return 16;
#else
            return 1;
#endif
        }
    }

    /// <summary>
    /// Returns the player number assigned to the Xbox Live User with input xuid.
    /// </summary>
    /// <param name="xuid">The xuid of the Xbox Live User</param>
    /// <returns>The player number assigned to the Xbox Live User or -1 if no Xbox Live Users are signed in with that xuid.</returns>
    int GetPlayerNumber(string xuid) { return 0; }

    /// <summary>
    /// Returns the Xbox Live Context for the Xbox Live User with the assigned player number.
    /// </summary>
    /// <param name="playerNumber">The player number.</param>
    /// <returns>The Xbox Live Context for the Xbox Live User with the assigned player number.</returns>
    XboxLiveContextSettings GetXboxLiveContextForPlayer(int playerNumber) { return null; }

    /// <summary>
    /// Validates whether the playerNumber if allowed for the operation requested or not
    /// </summary>
    /// <returns>True if valid. False, otherwise</returns>
    private bool ValidatePlayerNumber(int playerNumber, string operationName, bool signIn = true) {
        if (playerNumber <= 0) {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogError(operationName + " Failed: Player Number needs to be greater than zero.");
            }
            return false;
        }

        if (playerNumber > GetMaximumNumberOfPlayers()) {
            if (XboxLiveServicesSettings.Instance.DebugLogsOn)
            {
                Debug.LogError(operationName + " Failed: Player Number exceeds the maximum number of users allowed on this platform.");
            }
            return false;
        }

        if (CurrentPlayers.ContainsKey(playerNumber)) {
            if (signIn)
            {
                if (CurrentPlayers[playerNumber].XboxLiveUser != null)
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogError(operationName + " Failed: Player " + playerNumber + " already exists.");
                    }
                    NotifyAllCallbacks(
                        playerNumber, 
                        null, 
                        XboxLiveAuthStatus.Invalid, 
                        operationName + " Failed: Player " + playerNumber + " already exists.", 
                        true);
                    return false;
                }
            }
            else {
                if (CurrentPlayers[playerNumber].XboxLiveUser == null)
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogError(operationName + " Failed: Player " + playerNumber + " is already signed out.");
                    }
                    NotifyAllCallbacks(
                        playerNumber,
                        null,
                        XboxLiveAuthStatus.Invalid,
                        operationName + " Failed: Player " + playerNumber + " is already signed out.",
                        false);
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Notifies all callbacks about changes in status
    /// </summary>
    /// <param name="callbackList">A list of the UnityAction callbacks</param>
    /// <param name="xboxLiveUser">The Xbox Live User</param>
    /// <param name="authStatus">The Xbox Live Authentication Status</param>
    /// <param name="errorMessage">the Error Message</param>
    private void NotifyAllCallbacks(
        int playerNumber, 
        XboxLiveUser xboxLiveUser, 
        XboxLiveAuthStatus authStatus, 
        string errorMessage,
        bool signIn = true) {
        if (CurrentPlayers.ContainsKey(playerNumber)) {
            List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>> callbackList = null;
            if (signIn) {
                callbackList = CurrentPlayers[playerNumber].SignInCallbacks;
            }
            else {
                callbackList = CurrentPlayers[playerNumber].SignOutCallbacks;
            }
            foreach (var callback in callbackList)
            {
                callback(xboxLiveUser, authStatus, errorMessage);
            }
        }
    }


    /// <summary>
    /// An object that's used to track the current status of Xbox Live Users 
    /// And update callback methods interested in sign in and sign out of the users
    /// </summary>
    private class XboxLivePlayerInfo {
        public XboxLiveUser XboxLiveUser { get; set; }

        public List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>> SignInCallbacks { get; set; }

        public List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>> SignOutCallbacks { get; set; }

#if ENABLE_WINMD_SUPPORT
        Windows.System.User WindowsSystemUser WindowsUser { get; set; }
#endif
    }
}
