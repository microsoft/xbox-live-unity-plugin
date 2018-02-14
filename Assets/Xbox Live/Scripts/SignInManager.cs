using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_WINMD_SUPPORT
using Windows.System;
#endif

namespace Microsoft.Xbox.Services.Client
{
    public class SignInManager : Singleton<SignInManager>
    {

        private Dictionary<int, XboxLivePlayerInfo> CurrentPlayers;
        private int CurrentNumberOfPlayers = 0;
        private List<int> PlayersPendingSignIn;

        public void Awake()
        {
            CurrentPlayers = new Dictionary<int, XboxLivePlayerInfo>();
            this.PlayersPendingSignIn = new List<int>();
            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            // Super simple check to determine if configuration is non-empty.  This is not a thorough check to determine if the configuration is valid.
            // A user can easly bypass this check which will just cause them to fail at runtime if they try to use any functionality.
            if (!XboxLiveServicesSettings.EnsureXboxLiveServiceConfiguration() && Application.isPlaying)
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
        public IEnumerator SignInPlayer(int playerNumber)
        {
            yield return null;
            if (ValidatePlayerNumber(playerNumber, "Add User", XboxLiveOperationType.SignIn))
            {
                if (!CurrentPlayers.ContainsKey(playerNumber))
                {
                    var newPlayerInfo = new XboxLivePlayerInfo()
                    {
                        SignInCallbacks = new List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>>(),
                        SignOutCallbacks = new List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>>()
                    };
                    this.CurrentPlayers.Add(playerNumber, newPlayerInfo);
                }
                
#if ENABLE_WINMD_SUPPORT
            var playerInfo = this.CurrentPlayers[playerNumber];
            if (this.GetMaximumNumberOfPlayers() > 1)
            {
                var autoPicker = new Windows.System.UserPicker { AllowGuestAccounts = false };
                autoPicker.PickSingleUserAsync().AsTask().ContinueWith(
                        task =>
                        {
                            if (task.Status == TaskStatus.RanToCompletion)
                            {
                                playerInfo.WindowsUser = task.Result;
                                playerInfo.XboxLiveUser = new XboxLiveUser(playerInfo.WindowsUser);
                                XboxLiveUser.SignOutCompleted += XboxLiveUserSignOutCompleted;
                                this.PlayersPendingSignIn.Add(playerNumber);
                                this.StartCoroutine(this.SignInAsync(playerNumber, this.CurrentPlayers[playerNumber]));
                            }
                            else
                            {
                                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                                {
                                    Debug.Log("Exception occured: " + task.Exception.Message);
                                }
                            }
                        });
            }
            else
            {
                var usersTask = Windows.System.User.FindAllAsync().AsTask();
                if (usersTask.Status == TaskStatus.RanToCompletion && usersTask.Result != null && usersTask.Result.Count > 0)
                {
                    var windowsUser = usersTask.Result[0];
                    this.CurrentPlayers[playerNumber].WindowsUser = windowsUser;
                    this.CurrentPlayers[playerNumber].XboxLiveUser = new XboxLiveUser(windowsUser);
                    this.StartCoroutine(this.SignInAsync(playerNumber, this.CurrentPlayers[playerNumber]));
                }
                else
                {
                    this.CurrentPlayers[playerNumber].XboxLiveUser = new XboxLiveUser();
                    XboxLiveUser.SignOutCompleted += XboxLiveUserSignOutCompleted;
                    this.StartCoroutine(this.SignInAsync(playerNumber, this.CurrentPlayers[playerNumber]));
                }
            }
#else
                this.CurrentPlayers[playerNumber].XboxLiveUser = new XboxLiveUser();
                XboxLiveUser.SignOutCompleted += XboxLiveUserSignOutCompleted;
                this.StartCoroutine(this.SignInAsync(playerNumber, this.CurrentPlayers[playerNumber]));
#endif
            }
        }

        /// <summary>
        /// Removes and signs out the Xbox Live User with the assigned player number.
        /// Note: Sign out might not be supported on some platforms.
        /// </summary>
        /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
        public IEnumerator SignOutPlayer(int playerNumber)
        {
            yield return null;
            if (CurrentNumberOfPlayers > 1)
            {
                if (ValidatePlayerNumber(playerNumber, "Remove User", XboxLiveOperationType.SignOut))
                {
                    this.SignOutPlayerHelper(this.CurrentPlayers[playerNumber].XboxLiveUser);
                }
            }
            else
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError("Remove User Failed: At least one player should be signed in. Removing Player " + playerNumber + " would leave zero signed in players.");
                }
                NotifyAllCallbacks(
                    playerNumber,
                    null,
                    XboxLiveAuthStatus.Invalid,
                    "Remove User Failed: At least one player should be signed in. Removing Player " + playerNumber + " would leave zero signed in players.",
                    false);
            }
        }

        /// <summary>
        /// Signs out the Xbox Live User with the assigned player number and adds and signs in a new Xbox Live User.
        /// Note: Switching users might not be supported on some platforms.
        /// </summary>
        /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
        public IEnumerator SwitchUser(int playerNumber)
        {
            yield return null;
            if (ValidatePlayerNumber(playerNumber, "Switch User", XboxLiveOperationType.SignOut))
            {
                yield return this.SignOutPlayer(playerNumber);
                yield return this.SignInPlayer(playerNumber);
            }

        }

        /// <summary>
        /// Returns the Xbox Live User with the assigned player number if they exist. If no user is signed in for that player number, null is returned.
        /// </summary>
        /// <param name="playerNumber">The Player Number that should be assigned to the Xbox Live User</param>
        /// <returns>The Xbox Live User  with the assigned player number if exists. Null, otherwise.</returns>
        public XboxLiveUser GetPlayer(int playerNumber)
        {
            if (ValidatePlayerNumber(playerNumber, "Get User", XboxLiveOperationType.GetUser))
            {
                return this.CurrentPlayers[playerNumber].XboxLiveUser;
            }
            return null;
        }

        /// <summary>
        /// Returns the Xbox Live User with the assigned xuid if they exist. If no user is signed in for that player number, null is returned.
        /// </summary>
        /// <param name="xuid">The Xuid of the requested Xbox Live User</param>
        /// <returns>The Xbox Live User if exists. Null, otherwise.</returns>
        public XboxLiveUser GetPlayer(string xuid)
        {
            foreach (var playerInfo in this.CurrentPlayers)
            {
                if (playerInfo.Value.XboxLiveUser != null && playerInfo.Value.XboxLiveUser.XboxUserId.Equals(xuid))
                {
                    return playerInfo.Value.XboxLiveUser;
                }
            }
            return null;
        }

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
        public bool OnPlayerSignIn(int playerNumber, UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback)
        {
            if (AddCallbackHelper(playerNumber, "OnPlayerSignIn") 
                && !this.CurrentPlayers[playerNumber].SignInCallbacks.Contains(callback))
            {
                this.CurrentPlayers[playerNumber].SignInCallbacks.Add(callback);
                return true;
            }
            return false;
        }

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
        public bool OnPlayerSignOut(int playerNumber, UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback)
        {
            if (AddCallbackHelper(playerNumber, "OnPlayerSignOut")
                && !this.CurrentPlayers[playerNumber].SignOutCallbacks.Contains(callback))
            { 
                this.CurrentPlayers[playerNumber].SignOutCallbacks.Add(callback);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a callback method for when any Xbox Live User is signed in
        /// </summary>
        /// <param name="callback">A callback method that will be called when any Xbox Live User is signed in. 
        /// The callback method should be using three inputs of type: 
        /// - <see cref="XboxLiveUser"/> which would be the signed in user or null if the sign in failed. 
        /// - <see cref="XboxLiveAuthStatus"/> which is the status of sign in: Succeeded, Failed and Unsupported.
        /// - A string which will contain the error message if sign in failed.</param>
        public void OnAnyPlayerSignIn(UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback)
        {
            for (var playerNumber = 1; playerNumber <= this.GetMaximumNumberOfPlayers(); playerNumber++)
            {
                OnPlayerSignIn(playerNumber, callback);
            }
        }

        /// <summary>
        /// Adds a callback method for when any Xbox Live User signs out.
        /// </summary>
        /// <param name="callback">A callback method that will be called when the Xbox Live User is signed out. 
        /// The callback method should be using three inputs of type: 
        /// - <see cref="XboxLiveUser"/> which would be the signed in user or null if the sign out failed. 
        /// - <see cref="XboxLiveAuthStatus"/> which is the status of sign out: Succeeded, Failed and Unsupported.
        /// - A string which will contain the error message if sign out failed.</param>
        public void OnAnyPlayerSignOut(UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback)
        {
            for (var playerNumber = 1; playerNumber <= this.GetMaximumNumberOfPlayers(); playerNumber++)
            {
                OnPlayerSignOut(playerNumber, callback);
            }
        }

        /// <summary>
        /// Removes a callback from sign in and sign out if it exists
        /// so when the player status changes, the object will no longer be notified
        /// </summary>
        /// <param name="playerNumber">The Player Number</param>
        /// <param name="callback">The callback method should be using three inputs of type: 
        /// - <see cref="XboxLiveUser"/> which would be the signed in user or null if the sign out failed. 
        /// - <see cref="XboxLiveAuthStatus"/> which is the status of sign out: Succeeded, Failed and Unsupported.
        /// - A string which will contain the error message if sign out failed.</param>
        public void RemoveCallbackFromPlayer(int playerNumber, UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback)
        {
            if (this.CurrentPlayers.ContainsKey(playerNumber))
            {
                var playerInfo = this.CurrentPlayers[playerNumber];

                if (playerInfo.SignInCallbacks.Contains(callback))
                {
                    playerInfo.SignInCallbacks.Remove(callback);
                }

                if (playerInfo.SignOutCallbacks.Contains(callback))
                {
                    playerInfo.SignOutCallbacks.Remove(callback);
                }
            }
        }

        /// <summary>
        /// Removes a callback from sign in and sign out if it exists
        /// so when any player's status changes, the object will no longer be notified
        /// </summary>
        /// <param name="playerNumber">The Player Number</param>
        /// <param name="callback">The callback method should be using three inputs of type: 
        /// - <see cref="XboxLiveUser"/> which would be the signed in user or null if the sign out failed. 
        /// - <see cref="XboxLiveAuthStatus"/> which is the status of sign out: Succeeded, Failed and Unsupported.
        /// - A string which will contain the error message if sign out failed.</param>
        public void RemoveCallbackFromAllPlayers(UnityAction<XboxLiveUser, XboxLiveAuthStatus, string> callback)
        {
            foreach (var playerNumber in this.GetCurrentPlayers().Keys) {
                this.RemoveCallbackFromPlayer(playerNumber, callback);
            }
        }

        /// <summary>
        /// Returns a list of the currently signed in Xbox Live Users and their assigned player numbers
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with player number keys and Xbox Live User values.</returns>
        public Dictionary<int, XboxLiveUser> GetCurrentPlayers()
        {
            var currentPlayerDictionary = new Dictionary<int, XboxLiveUser>();
            foreach (var playerInfo in this.CurrentPlayers)
            {
                if (playerInfo.Value.XboxLiveUser != null)
                {
                    currentPlayerDictionary.Add(playerInfo.Key, playerInfo.Value.XboxLiveUser);
                }
            }
            return currentPlayerDictionary;
        }

        /// <summary>
        /// Returns the number of signed in Xbox Live Users.
        /// </summary>
        /// <returns>An integer representing the number of currently signed in players.</returns>
        public int GetCurrentNumberOfPlayers()
        {
            return this.GetCurrentPlayers().Count;
        }

        /// <summary>
        /// Returns the maximum number of signed in Xbox Live Users supported on current platform.
        /// </summary>
        /// <returns>An integer representing the maximum number of players supported on the current platform.</returns>
        public int GetMaximumNumberOfPlayers()
        {
            if (Application.isEditor)
            {
                return 16;
            }
            else
            {
                if (SystemInfo.deviceType == DeviceType.Console)
                {
                    return 16;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Returns the player number assigned to the Xbox Live User with input xuid.
        /// </summary>
        /// <param name="xuid">The xuid of the Xbox Live User</param>
        /// <returns>The player number assigned to the Xbox Live User or -1 if no Xbox Live Users are signed in with that xuid.</returns>
        public int GetPlayerNumber(string xuid)
        {
            foreach (var playerInfo in this.CurrentPlayers)
            {
                if (playerInfo.Value.XboxLiveUser != null && playerInfo.Value.XboxLiveUser.XboxUserId.Equals(xuid))
                {
                    return playerInfo.Key;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the Xbox Live Context for the Xbox Live User with the assigned player number.
        /// </summary>
        /// <param name="playerNumber">The player number.</param>
        /// <returns>The Xbox Live Context for the Xbox Live User with the assigned player number.</returns>
        public XboxLiveContext GetXboxLiveContextForPlayer(int playerNumber)
        {
            if (ValidatePlayerNumber(playerNumber, "Get Xbox Live Context", XboxLiveOperationType.GetUser))
            {
                if (this.CurrentPlayers[playerNumber].XboxLiveContext == null) {
                    this.CurrentPlayers[playerNumber].XboxLiveContext =  new XboxLiveContext(this.CurrentPlayers[playerNumber].XboxLiveUser);
                }

                return this.CurrentPlayers[playerNumber].XboxLiveContext;
            }
            return null;
        }

        /// <summary>
        /// Validates whether the playerNumber if allowed for the operation requested or not
        /// </summary>
        /// <returns>True if valid. False, otherwise</returns>
        private bool ValidatePlayerNumber(int playerNumber, string operationName, XboxLiveOperationType operationType)
        {
            if (playerNumber <= 0)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(operationName + " Failed: Player Number needs to be greater than zero.");
                }
                return false;
            }

            if (playerNumber > GetMaximumNumberOfPlayers())
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(operationName + " Failed: Player Number exceeds the maximum number of users allowed on this platform.");
                }
                return false;
            }

            if (operationType == XboxLiveOperationType.SignIn)
            {
                if (CurrentPlayers[playerNumber].XboxLiveUser != null)
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogError(operationName + " Failed: Player " + playerNumber + " is already signed in.");
                    }
                    NotifyAllCallbacks(
                        playerNumber,
                        null,
                        XboxLiveAuthStatus.Invalid,
                        operationName + " Failed: Player " + playerNumber + " is already signed in.",
                        true);
                    return false;
                }
            }

            if (operationType == XboxLiveOperationType.SignOut || operationType == XboxLiveOperationType.GetUser)
            {
                if (!CurrentPlayers.ContainsKey(playerNumber) || CurrentPlayers[playerNumber].XboxLiveUser == null)
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogWarning(operationName + " Failed: Player " + playerNumber + " is not signed in.");
                    }

                    if (operationType == XboxLiveOperationType.SignOut)
                    {
                        NotifyAllCallbacks(
                            playerNumber,
                            null,
                            XboxLiveAuthStatus.Invalid,
                            operationName + " Failed: Player " + playerNumber + " is not signed in.",
                            false);
                    }
                    return false;
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
            bool signIn = true)
        {
            if (CurrentPlayers.ContainsKey(playerNumber))
            {
                List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>> callbackList = null;
                if (signIn)
                {
                    callbackList = CurrentPlayers[playerNumber].SignInCallbacks;
                }
                else
                {
                    callbackList = CurrentPlayers[playerNumber].SignOutCallbacks;
                }
                foreach (var callback in callbackList)
                {
                    callback(xboxLiveUser, authStatus, errorMessage);
                }
            }
        }

        /// <summary>
        /// Removed the xbox live user from StatsManager and Social manager when sign out is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="signOutComlpetedEventArgs"></param>
        private void XboxLiveUserSignOutCompleted(object sender, SignOutCompletedEventArgs signOutComlpetedEventArgs)
        {
            var xboxLiveUser = signOutComlpetedEventArgs.User as XboxLiveUser;
            this.SignOutPlayerHelper(xboxLiveUser);
        }

        /// <summary>
        /// Assists with the handling of user removal
        /// </summary>
        /// <param name="xboxLiveUser">The Xbox Live user being removed</param>
        private void SignOutPlayerHelper(XboxLiveUser xboxLiveUser)
        {
            if (xboxLiveUser != null)
            {
                XboxLive.Instance.StatsManager.RemoveLocalUser(xboxLiveUser);
                XboxLive.Instance.SocialManager.RemoveLocalUser(xboxLiveUser);
            }
            var playerNumber = GetPlayerNumber(xboxLiveUser.XboxUserId);
            this.CurrentPlayers[playerNumber].XboxLiveUser = null;
            this.CurrentPlayers[playerNumber].XboxLiveContext = null;
#if ENABLE_WINMD_SUPPORT
            this.CurrentPlayers[playerNumber].WindowsUser = null;
#endif
            this.NotifyAllCallbacks(playerNumber, null, XboxLiveAuthStatus.Succeeded, null, false);
        }

        /// <summary>
        /// Assists with the validating and setting up for adding callbacks
        /// </summary>
        /// <param name="playerNumber">The Player Number</param>
        /// <returns>True, if valid. False, otherwise</returns>
        private bool AddCallbackHelper(int playerNumber, string callbackTypeName)
        {
            if (ValidatePlayerNumber(playerNumber, "Adding " + callbackTypeName + " Callback", XboxLiveOperationType.AddCallback))
            {
                if (!CurrentPlayers.ContainsKey(playerNumber))
                {
                    var playerInfo = new XboxLivePlayerInfo()
                    {
                        SignInCallbacks = new List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>>(),
                        SignOutCallbacks = new List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>>()
                    };
                    this.CurrentPlayers.Add(playerNumber, playerInfo);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Signs in a player. Attempts sign in silently first then sign in.
        /// </summary>
        /// <param name="playerNumber">The player number</param>
        /// <param name="playerInfo">The player info of the the player being signed in</param>
        /// <returns>A Coroutine</returns>
        private IEnumerator SignInAsync(int playerNumber, XboxLivePlayerInfo playerInfo)
        {
            SignInStatus signInStatus = SignInStatus.Success;

            TaskYieldInstruction<SignInResult> signInSilentlyTask = playerInfo.XboxLiveUser.SignInSilentlyAsync().AsCoroutine();
            yield return signInSilentlyTask;

            try
            {
                signInStatus = signInSilentlyTask.Result.Status;
            }
            catch (Exception ex)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError("Exception occured: " + ex.Message);
                }

            }

            if (signInStatus != SignInStatus.Success)
            {
                TaskYieldInstruction<SignInResult> signInTask = playerInfo.XboxLiveUser.SignInAsync().AsCoroutine();
                yield return signInTask;

                try
                {
                    signInStatus = signInTask.Result.Status;
                }
                catch (Exception ex)
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogError("Exception occured: " + ex.Message);
                    }
                }
            }

            try
            {
                // Throw any exceptions if needed.
                if (signInStatus == SignInStatus.Success)
                {
                    CurrentNumberOfPlayers++;
                    XboxLive.Instance.StatsManager.AddLocalUser(playerInfo.XboxLiveUser);
                    XboxLive.Instance.SocialManager.AddLocalUser(playerInfo.XboxLiveUser, SocialManagerExtraDetailLevel.PreferredColorLevel);
                    this.NotifyAllCallbacks(playerNumber, playerInfo.XboxLiveUser, XboxLiveAuthStatus.Succeeded, null, true);
                }
                else
                {
                    NotifyAllCallbacks(playerNumber, null, XboxLiveAuthStatus.Failed, "Sign In Failed: Player " + playerNumber + " failed. Sign In Status: " + signInStatus);
                }
            }
            catch (Exception ex)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError("Exception occured: " + ex.Message);
                }
            }
        }

        private void Update()
        {
            if (this.PlayersPendingSignIn.Count > 0)
            {
                var playerNumber = this.PlayersPendingSignIn[0];
                this.StartCoroutine(this.SignInAsync(playerNumber, this.CurrentPlayers[playerNumber]));
                this.PlayersPendingSignIn.Remove(playerNumber);
            }
        }

        /// <summary>
        /// An object that's used to track the current status of Xbox Live Users 
        /// And update callback methods interested in sign in and sign out of the users
        /// </summary>
        private class XboxLivePlayerInfo
        {
            public XboxLiveUser XboxLiveUser { get; set; }

            public List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>> SignInCallbacks { get; set; }

            public List<UnityAction<XboxLiveUser, XboxLiveAuthStatus, string>> SignOutCallbacks { get; set; }

            public XboxLiveContext XboxLiveContext { get; set; }

#if ENABLE_WINMD_SUPPORT
        public Windows.System.User WindowsUser { get; set; }
#endif
        }

        /// <summary>
        /// An Enumeration used to signal the type of the operation being handled
        /// </summary>
        private enum XboxLiveOperationType
        {
            SignIn,
            SignOut,
            GetUser,
            AddCallback
        }
    }
}