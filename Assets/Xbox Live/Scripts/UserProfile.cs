// -----------------------------------------------------------------------
//  <copyright file="UserProfile.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

using Microsoft.Xbox.Services.Social.Manager;
using Microsoft.Xbox.Services.System;
using Microsoft.Xbox.Services;

using UnityEngine;
using UnityEngine.UI;

using Debug = System.Diagnostics.Debug;

public class UserProfile : MonoBehaviour
{
    public GameObject signInPanel;
    public GameObject profileInfoPanel;
    public Image gamerpic;
    public Image gamerpicMask;
    public Text gamertag;
    public Text gamerscore;

    public void Awake()
    {
        this.profileInfoPanel.SetActive(false);
        XboxLiveUser.SignOutCompleted += this.XboxLiveUserOnSignOutCompleted;

#if UNITY_EDITOR
        // TODO: This ignors all SSL certs because Mono uses it's own cert store.  This MUST be removed when the issues are resolved.
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
        {
            Debug.Write(errors);
            return true;
        };
#endif
    }

    public void Start()
    {
        // Disable the sign-in button if there's no configuration available.
        this.signInPanel.GetComponentInChildren<Button>().interactable = XboxLive.IsEnabled;
    }

    private void XboxLiveUserOnSignOutCompleted(object sender, SignOutCompletedEventArgs signOutCompletedEventArgs)
    {
        
        this.Refresh();
    }

    public void SignIn()
    {
        var coroutine = StartCoroutine(this.SignInAsync());
    }

    public IEnumerator SignInAsync()
    {
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

        yield return XboxLive.Instance.SignInAsync();
        yield return SocialManager.Instance.AddLocalUser(XboxLive.Instance.User, SocialManagerExtraDetailLevel.PreferredColor).AsCoroutine();
        yield return this.LoadProfileInfo();
    }

    private IEnumerator LoadProfileInfo()
    {
        ulong userId = ulong.Parse(XboxLive.Instance.User.XboxUserId);
        var group = SocialManager.Instance.CreateSocialUserGroupFromList(XboxLive.Instance.User, new List<ulong> { userId });
        var socialUser = group.GetUser(userId);

        WWW www = new WWW(socialUser.DisplayPicRaw + "&w=128");
        yield return www;

        Texture2D t = www.texture;
        Rect r = new Rect(0, 0, t.width, t.height);
        this.gamerpic.sprite = Sprite.Create(t, r, Vector2.zero);
        this.gamertag.text = XboxLive.Instance.User.Gamertag;
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
        bool isSignedIn = XboxLive.Instance.Context != null;
        this.signInPanel.SetActive(!isSignedIn);
        this.profileInfoPanel.SetActive(isSignedIn);
    }
}