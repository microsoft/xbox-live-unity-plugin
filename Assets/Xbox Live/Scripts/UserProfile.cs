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
    public XboxLiveUserInfo XboxLiveUser;

    public string SignInKeyName;

    private bool SignInCalledOnce;

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

    [HideInInspector]
    public XboxLiveUserInfo XboxLiveUserPrefab;

    public bool AllowGuestAccounts = false;

    public void Awake()
    {
        this.EnsureEventSystem();

        if (!XboxLiveUserHelper.Instance.IsInitialized)
        {
            XboxLiveUserHelper.Instance.Initialize();
        }
    }

    public void Start()
    {
        // Disable the sign-in button if there's no configuration available.
        if (XboxLive.Instance.AppConfig == null)
        {
            Button signInButton = this.signInPanel.GetComponentInChildren<Button>();
            signInButton.interactable = false;
            Text signInButtonText = signInButton.GetComponentInChildren<Text>(true);
            if (signInButtonText != null)
            {
                signInButtonText.fontSize = 16;
                signInButtonText.text = "Xbox Live is not enabled.\nSee errors for detail.";
            }
        }

        if (XboxLiveUserHelper.Instance.SingleUserModeEnabled && XboxLiveUserHelper.Instance.SingleXboxLiveUser == null)
        {
            Debug.Log("Instantiating Xbox Live User");
            this.XboxLiveUser = Instantiate(this.XboxLiveUserPrefab);
            XboxLiveUserHelper.Instance.SingleXboxLiveUser = this.XboxLiveUser;
        }

        this.Refresh();
    }

    private void XboxLiveUserOnSignOutCompleted(object sender, SignOutCompletedEventArgs signOutCompletedEventArgs)
    {
        this.Refresh();
    }

    public void SignIn()
    {
        this.StartCoroutine(this.InitializeXboxLiveUser());
    }

    public void Update()
    {
        if (XboxLiveUserHelper.Instance.SingleUserModeEnabled && XboxLiveUserHelper.Instance.SingleXboxLiveUser != null
            && XboxLiveUserHelper.Instance.SingleXboxLiveUser.User != null
            && !XboxLiveUserHelper.Instance.SingleXboxLiveUser.User.IsSignedIn && !this.SignInCalledOnce)
        {
            this.StartCoroutine(this.SignInAsync());
        }

        if (this.XboxLiveUser != null && this.XboxLiveUser.User != null && !this.XboxLiveUser.User.IsSignedIn && !this.SignInCalledOnce)
        {
            this.StartCoroutine(this.SignInAsync());
        }

        if (!this.SignInCalledOnce && !string.IsNullOrEmpty(this.SignInKeyName) && Input.GetKeyDown(this.SignInKeyName))
        {
            this.StartCoroutine(this.InitializeXboxLiveUser());
        }

    }

    public IEnumerator InitializeXboxLiveUser()
    {
        yield return null;
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

#if NETFX_CORE
        if (!XboxLiveUserHelper.Instance.SingleUserModeEnabled) {
            var autoPicker = new Windows.System.UserPicker { AllowGuestAccounts = this.AllowGuestAccounts};
            autoPicker.PickSingleUserAsync().AsTask().ContinueWith(
                    task =>
                        {
                            if (task.Status == TaskStatus.RanToCompletion)
                            {
                                this.XboxLiveUser.WindowsSystemUser = task.Result;
                                this.XboxLiveUser.Initialize();
                            }
                            else
                            {
                                Debug.Log("Exception occured: "+ task.Exception.Message);
                            }
                        });
        } 
        else {
           this.XboxLiveUser = XboxLiveUserHelper.Instance.SingleXboxLiveUser;
           this.XboxLiveUser.Initialize();
        }
#else
        if (XboxLiveUserHelper.Instance.SingleUserModeEnabled)
        {
            this.XboxLiveUser = XboxLiveUserHelper.Instance.SingleXboxLiveUser;
        }

        this.XboxLiveUser.Initialize();
#endif

    }

    public IEnumerator SignInAsync()
    {
        this.SignInCalledOnce = true;

        TaskYieldInstruction<SignInResult> signInTask = this.XboxLiveUser.User.SignInAsync().AsCoroutine();
        yield return signInTask;

        // Throw any exceptions if needed.
        if (signInTask.Result.Status != SignInStatus.Success)
        {
            throw new Exception("Sign in Failed");
        }


        XboxLive.Instance.StatsManager.AddLocalUser(this.XboxLiveUser.User);
        var addLocalUserTask = XboxLive.Instance.SocialManager.AddLocalUser(this.XboxLiveUser.User, SocialManagerExtraDetailLevel.PreferredColor).AsCoroutine();
        yield return addLocalUserTask;

        if (addLocalUserTask.Task.IsFaulted)
        {
            throw addLocalUserTask.Task.Exception;
        }

        yield return this.LoadProfileInfo();
    }

    private IEnumerator LoadProfileInfo()
    {
        var userId = ulong.Parse(this.XboxLiveUser.User.XboxUserId);
        var group = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromList(this.XboxLiveUser.User, new List<ulong> { userId });
        var socialUser = group.GetUser(userId);

        var www = new WWW(socialUser.DisplayPicRaw + "&w=128");
        yield return www;

        var t = www.texture;
        var r = new Rect(0, 0, t.width, t.height);
        this.gamerpic.sprite = Sprite.Create(t, r, Vector2.zero);
        this.gamertag.text = this.XboxLiveUser.User.Gamertag;
        this.gamerscore.text = socialUser.Gamerscore;

        this.profileInfoPanel.GetComponent<Image>().color = ColorFromHexString(socialUser.PreferredColor.PrimaryColor);
        this.gamerpicMask.color = ColorFromHexString(socialUser.PreferredColor.PrimaryColor);

        this.Refresh();
    }

    public static Color ColorFromHexString(string color)
    {
        float r = (float)byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber) / 255;
        float g = (float)byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255;
        float b = (float)byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255;

        return new Color(r, g, b);
    }

    private void Refresh()
    {
        bool isSignedIn = this.XboxLiveUser.User != null && this.XboxLiveUser.User.IsSignedIn;
        this.signInPanel.SetActive(!isSignedIn);
        this.profileInfoPanel.SetActive(isSignedIn);
    }
}