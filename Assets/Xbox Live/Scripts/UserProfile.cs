// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Xbox.Services.CSharp;
using Microsoft.Xbox.Services.CSharp.Social.Manager;

using UnityEngine;
using UnityEngine.UI;

public class UserProfile : MonoBehaviour
{
    private XboxLiveUser User;

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

    public void Awake()
    {
        this.EnsureEventSystem();

        this.profileInfoPanel.SetActive(false);
        XboxLiveUser.SignOutCompleted += this.XboxLiveUserOnSignOutCompleted;
        this.Refresh();
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
    }

    private void XboxLiveUserOnSignOutCompleted(object sender, SignOutCompletedEventArgs signOutCompletedEventArgs)
    {
        this.Refresh();
    }

    public void SignIn()
    {
        this.StartCoroutine(this.SignInAsync());
    }

    public IEnumerator SignInAsync()
    {
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

        this.User = XboxLiveComponent.Instance.User;
        TaskYieldInstruction<SignInResult> signInTask = this.User.SignInAsync().AsCoroutine();
        yield return signInTask;

        // Throw any exceptions if needed.
        if (signInTask.Result.Status != SignInStatus.Success)
        {
            throw new Exception("Sign in Failed");
        }

        XboxLive.Instance.SocialManager.AddLocalUser(this.User);
        yield return XboxLive.Instance.SocialManager.AddLocalUser(this.User, SocialManagerExtraDetailLevel.PreferredColor).AsCoroutine();
        yield return this.LoadProfileInfo();
    }

    private IEnumerator LoadProfileInfo()
    {
        ulong userId = ulong.Parse(this.User.XboxUserId);
        var group = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromList(this.User, new List<ulong> { userId });
        var socialUser = group.GetUser(userId);

        WWW www = new WWW(socialUser.DisplayPicRaw + "&w=128");
        yield return www;

        Texture2D t = www.texture;
        Rect r = new Rect(0, 0, t.width, t.height);
        this.gamerpic.sprite = Sprite.Create(t, r, Vector2.zero);
        this.gamertag.text = this.User.Gamertag;
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
        bool isSignedIn = this.User != null && this.User.IsSignedIn;
        this.signInPanel.SetActive(!isSignedIn);
        this.profileInfoPanel.SetActive(isSignedIn);
    }
}