// -----------------------------------------------------------------------
//  <copyright file="UserProfile.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;
using Microsoft.Xbox.Services.System;

using UnityEngine;
using UnityEngine.UI;

public class UserProfile : MonoBehaviour
{
    public GameObject signInPanel;
    public GameObject profileInfoPanel;
    public Image gamerpic;
    public Text gamertag;

    public void Awake()
    {
        this.profileInfoPanel.SetActive(false);
        XboxLiveUser.SignOutCompleted += this.XboxLiveUserOnSignOutCompleted;

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
        {
            Debug.Log(errors);
            return true;
        };
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
        this.StartCoroutine(this.SignInAsync());
    }

    public IEnumerator SignInAsync()
    {
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

        yield return XboxLive.Instance.SignInAsync();
        yield return SocialManager.Instance.AddLocalUser(XboxLive.Instance.User, SocialManagerExtraDetailLevel.PreferredColorLevel).AsCoroutine();
        yield return this.LoadProfileInfo();
    }

    private IEnumerator LoadProfileInfo()
    {
        ulong userId = ulong.Parse(XboxLive.Instance.User.XboxUserId);
        var group = SocialManager.Instance.CreateSocialUserGroupFromList(XboxLive.Instance.User, new List<ulong> { userId });
        var socialUser = group.GetUser(userId);

        // How do we get the user profile pic from the Xbox Live SDK?  
        WWW www = new WWW(socialUser.DisplayPicRaw + "&w=128");
        yield return www;

        Texture2D t = www.texture;
        Rect r = new Rect(0, 0, t.width, t.height);
        this.gamerpic.sprite = Sprite.Create(t, r, Vector2.zero);
        this.gamertag.text = XboxLive.Instance.User.Gamertag;

        this.Refresh();
    }

    private void Refresh()
    {
        bool isSignedIn = XboxLive.Instance.Context != null;
        this.signInPanel.SetActive(!isSignedIn);
        this.profileInfoPanel.SetActive(isSignedIn);
    }
}