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

public class UserProfile : MonoBehaviour
{
    public bool EnableControllerInput = false;

    public int JoystickNumber = 1;

    public XboxControllerButtons SignInButton;

    public XboxControllerButtons SignOutButton;

    public int PlayerNumber = 1;

    private string signInInputButton;

    private string signOutInputButton;

    [HideInInspector]
    public GameObject signInPanel;

    [HideInInspector]
    public GameObject profileInfoPanel;

    [HideInInspector]
    public Image gamerpic;

    [HideInInspector]
    public Image gamerpicMask;

    [HideInInspector]
    public Text gamertag;

    [HideInInspector]
    public Text gamerscore;

    private bool AllowGuestAccounts = false;

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
        if (XboxLive.Instance.AppConfig == null || XboxLive.Instance.AppConfig.ServiceConfigurationId == null)
        {
            this.ConfigAvailable = false;

            Text signInButtonText = this.signInPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>(true);
            if (signInButtonText != null)
            {
                signInButtonText.fontSize = 16;
                signInButtonText.text = "Xbox Live is not enabled.\nSee errors for detail.";
            }
        }
        this.Refresh();

        try
        {
            SocialManagerComponent.Instance.EventProcessed += SocialManagerEventProcessed;
            SignInManager.Instance.OnPlayerSignOut(this.PlayerNumber, this.XboxLiveUserOnSignOutCompleted);
            SignInManager.Instance.OnPlayerSignIn(this.PlayerNumber, this.SetupForRefresh);
            this.xboxLiveUser = SignInManager.Instance.GetUser(this.PlayerNumber);
            if (this.xboxLiveUser != null)
            {
                this.LoadProfileInfo();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }

    private void XboxLiveUserOnSignOutCompleted(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus authStatus, string errorMessage)
    {
        this.signInPanel.GetComponentInChildren<Button>().interactable = true;
        this.AllowSignInAttempt = true;
        this.Refresh();
    }

    public void SignIn()
    {
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

        // Don't allow subsequent sign in attempts until the current attempt completes
        this.AllowSignInAttempt = false;
        this.StartCoroutine(SignInManager.Instance.AddUser(this.PlayerNumber));
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
            this.xboxLiveUser = SignInManager.Instance.GetUser(this.PlayerNumber);
        }

        if (socialEvent.User.XboxUserId != this.xboxLiveUser.XboxUserId)
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

            this.gamerscore.text = socialUser.Gamerscore;

            if (socialUser.PreferredColor != null)
            {
                this.profileInfoPanel.GetComponent<Image>().color =
                    ColorFromHexString(socialUser.PreferredColor.PrimaryColor);
                this.gamerpicMask.color = ColorFromHexString(socialUser.PreferredColor.PrimaryColor);
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

    private void SetupForRefresh(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus authStatus, string errorMessage)
    {
        if (authStatus == XboxLiveAuthStatus.Succeeded && xboxLiveUser != null)
        {
            this.xboxLiveUser = xboxLiveUser;
        }
        else
        {
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

    private void SignOut() {
        this.StartCoroutine(SignInManager.Instance.RemoveUser(this.PlayerNumber));
    }

    private void OnDestroy()
    {
        SignInManager.Instance.RemoveCallback(this.PlayerNumber, this.XboxLiveUserOnSignOutCompleted);
        SignInManager.Instance.RemoveCallback(this.PlayerNumber, this.SetupForRefresh);
        SocialManagerComponent.Instance.EventProcessed -= SocialManagerEventProcessed;
    }
}