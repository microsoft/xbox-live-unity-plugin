// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    public class PlayerAuthentication : MonoBehaviour
    {

        [Header("Xbox Live Settings")]
        public int PlayerNumber = 1;

        [Header("Display Settings")]
        public Theme Theme = Theme.Light;

        [Header("Controller Settings")]
        public bool EnableControllerInput = false;

        public int JoystickNumber = 1;

        public XboxControllerButtons SignInButton;

        public XboxControllerButtons SignOutButton;


        [Header("UI Component References")]
        public GameObject signInPanel;

        public GameObject profileInfoPanel;

        public Image gamerpic;

        public Image gamerpicMask;

        public Image signInPanelImage;

        public Image profileInfoPanelImage;

        public Text gamertag;

        public Text playerNumberText;


        private string signInInputButton;

        private string signOutInputButton;
        

        public readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();

        private XboxLiveUser xboxLiveUser;

        private XboxSocialUserGroup userGroup;

        private bool AllowSignInAttempt = true;

        private bool ConfigAvailable = true;
        public void Awake()
        {
            this.EnsureEventSystem();
            XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();

            if (this.EnableControllerInput)
            {
                if (this.SignInButton != XboxControllerButtons.None)
                {
                    this.signInInputButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.SignInButton);
                }

                if (this.SignOutButton != XboxControllerButtons.None)
                {
                    this.signOutInputButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.SignOutButton);
                }
            }
        }

        public void Start()
        {
            // Disable the sign-in button if there's no configuration available.
            if (!XboxLiveServicesSettings.EnsureXboxLiveServiceConfiguration())
            {
                this.ConfigAvailable = false;

                Text signInButtonText = this.signInPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>(true);
                if (signInButtonText != null)
                {
                    signInButtonText.fontSize = 16;
                    signInButtonText.text = "Xbox Live is not enabled.\nSee errors for detail.";
                }
            }
            this.playerNumberText.text = "P" + this.PlayerNumber;
            this.Refresh();

            try
            {
                SocialManagerComponent.Instance.EventProcessed += SocialManagerEventProcessed;
                SignInManager.Instance.OnPlayerSignOut(this.PlayerNumber, this.OnPlayerSignOut);
                SignInManager.Instance.OnPlayerSignIn(this.PlayerNumber, this.OnPlayerSignIn);
                this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
                if (this.xboxLiveUser != null)
                {
                    this.LoadProfileInfo();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }

            this.StartCoroutine(this.LoadTheme());
        }

        private void OnPlayerSignOut(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus authStatus, string errorMessage)
        {
            if (authStatus == XboxLiveAuthStatus.Succeeded)
            {
                this.signInPanel.GetComponentInChildren<Button>().interactable = true;
                this.AllowSignInAttempt = true;
                this.xboxLiveUser = null;
                this.Refresh();
            }
            else
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(errorMessage);
                }
            }
        }

        public void SignIn()
        {
            // Disable the sign-in button
            this.signInPanel.GetComponentInChildren<Button>().interactable = false;

            // Don't allow subsequent sign in attempts until the current attempt completes
            this.AllowSignInAttempt = false;
            this.StartCoroutine(SignInManager.Instance.SignInPlayer(this.PlayerNumber));
        }

        public void Update()
        {
            while (ExecuteOnMainThread.Count > 0)
            {
                ExecuteOnMainThread.Dequeue().Invoke();
            }

            if (this.AllowSignInAttempt && this.EnableControllerInput && !string.IsNullOrEmpty(this.signInInputButton) && Input.GetKeyDown(this.signInInputButton))
            {
                this.SignIn();
            }

            if (!this.AllowSignInAttempt && this.EnableControllerInput && !string.IsNullOrEmpty(this.signOutInputButton) && Input.GetKeyDown(this.signOutInputButton))
            {
                this.SignOut();
            }
        }

        private void LoadProfileInfo(bool userAdded = true)
        {
            this.gamertag.text = this.xboxLiveUser.Gamertag;

            if (userAdded)
            {
                try
                {
                    userGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromList(this.xboxLiveUser, new List<string> { this.xboxLiveUser.XboxUserId });
                }
                catch (Exception ex)
                {
                    if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                    {
                        Debug.LogError("Exception occured: " + ex.Message);
                    }
                }
            }
        }

        private void SocialManagerEventProcessed(object sender, SocialEvent socialEvent)
        {
            if (this.xboxLiveUser == null)
            {
                this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            }

            if (this.xboxLiveUser == null || (socialEvent.User.XboxUserId != this.xboxLiveUser.XboxUserId))
            {
                // Ignore the social event
                return;
            }

            if (socialEvent.EventType == SocialEventType.LocalUserAdded)
            {
                if (socialEvent.ErrorCode != 0 && XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogFormat("Failed to add local user to SocialManager: {0}", socialEvent.ErrorMessge);
                    LoadProfileInfo(false);
                }
                else
                {
                    LoadProfileInfo();
                }
            }
            else if (socialEvent.EventType == SocialEventType.SocialUserGroupLoaded &&
                     ((SocialUserGroupLoadedEventArgs)socialEvent.EventArgs).SocialUserGroup == userGroup)
            {
                if (socialEvent.ErrorCode != 0 && XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogFormat("Failed to load the SocialUserGroup: {0}", socialEvent.ErrorMessge);
                }
                else
                {
                    StartCoroutine(FinishLoadingProfileInfo());
                }
            }
        }

        private IEnumerator FinishLoadingProfileInfo()
        {
            this.playerNumberText.color = ThemeHelper.GetThemeBaseFontColor(this.Theme);
            var socialUser = userGroup.GetUsersFromXboxUserIds(new List<string> { this.xboxLiveUser.XboxUserId })[0];

            var www = new WWW(socialUser.DisplayPicRaw + "&w=128");
            yield return www;

            try
            {
                if (www.isDone && string.IsNullOrEmpty(www.error))
                {
                    var t = www.texture;
                    var r = new Rect(0, 0, t.width, t.height);
                    this.gamerpic.sprite = Sprite.Create(t, r, Vector2.zero);
                }

            }
            catch (Exception ex)
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.Log("There was an error while loading Profile Info. Exception: " + ex.Message);
                }
            }
            this.Refresh();
        }

        public static Color ColorFromHexString(string color)
        {
            var r = (float)byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber) / 255;
            var g = (float)byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255;
            var b = (float)byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255;

            return new Color(r, g, b);
        }

        private void OnPlayerSignIn(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus authStatus, string errorMessage)
        {
            if (authStatus == XboxLiveAuthStatus.Succeeded && xboxLiveUser != null)
            {
                this.xboxLiveUser = xboxLiveUser;
            }
            else
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(errorMessage);
                }
                this.signInPanel.GetComponentInChildren<Button>().interactable = true;
                this.AllowSignInAttempt = true;
            }
        }

        private void Refresh()
        {
            var isSignedIn = this.xboxLiveUser != null && this.xboxLiveUser.IsSignedIn;
            this.AllowSignInAttempt = !isSignedIn && this.ConfigAvailable;
            this.signInPanel.GetComponentInChildren<Button>().interactable = this.AllowSignInAttempt;
            this.signInPanel.SetActive(!isSignedIn);
            this.profileInfoPanel.SetActive(isSignedIn);
        }

        private IEnumerator LoadTheme() {
            yield return null;

            var backgroundColor = ThemeHelper.GetThemeBackgroundColor(this.Theme);
            this.profileInfoPanelImage.color = backgroundColor;
            this.gamerpicMask.color = backgroundColor;
            this.signInPanelImage.sprite = ThemeHelper.LoadSprite(this.Theme, "RowBackground-Highlighted");
            this.gamertag.color = ThemeHelper.GetThemeBaseFontColor(this.Theme);
        }

        private void SignOut()
        {
            this.playerNumberText.color = Color.white;
            this.StartCoroutine(SignInManager.Instance.SignOutPlayer(this.PlayerNumber));
        }

        private void OnDestroy()
        {
            if (SignInManager.Instance != null)
            {
                SignInManager.Instance.RemoveCallbackFromPlayer(this.PlayerNumber, this.OnPlayerSignOut);
                SignInManager.Instance.RemoveCallbackFromPlayer(this.PlayerNumber, this.OnPlayerSignIn);
            }
            if (SocialManagerComponent.Instance != null)
            {
                SocialManagerComponent.Instance.EventProcessed -= SocialManagerEventProcessed;
            }
        }
    }
}