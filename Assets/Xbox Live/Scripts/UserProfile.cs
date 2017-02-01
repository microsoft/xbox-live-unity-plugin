// -----------------------------------------------------------------------
//  <copyright file="UserProfile.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social;
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
        var coroutine = this.StartCoroutine(this.SignInAsync());
    }

    public IEnumerator SignInAsync()
    {
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
        {
            return true;
        };

        yield return XboxLive.Instance.SignInAsync();

        //yield return this.LoadProfileInfo();
        Task addLocalUser = SocialManager.Instance.AddLocalUser(XboxLive.Instance.User, SocialManagerExtraDetailLevel.PreferredColorLevel);
        yield return addLocalUser.AsCoroutine();
        ulong userId = Convert.ToUInt64(XboxLive.Instance.User.XboxUserId);

        XboxSocialUserGroup group = SocialManager.Instance.CreateSocialUserGroupFromList(XboxLive.Instance.User, new List<ulong> { userId });
        XboxSocialUser user;
        do
        {
            yield return null;
            user = group.GetUser(userId);

            Debug.Log("User? " + (user == null));
        }
        while (user == null);

        var u = group.Users.FirstOrDefault();
    }

    private IEnumerator LoadProfileInfo()
    {
        string userId = XboxLive.Instance.Context.User.XboxUserId;
        var getUserProfile = XboxLive.Instance.Context.ProfileService.GetUserProfileAsync(userId).AsCoroutine();
        yield return getUserProfile;

        XboxUserProfile profile;
        try
        {
            profile = getUserProfile.Result;
        }
        catch (AggregateException ae)
        {
            Debug.Log(ae.ToString());
            Debug.Log(ae.InnerException.ToString());
            yield break;
        }
        string gamerpicUrl = profile.GameDisplayPictureResizeUri.ToString();
        gamerpicUrl += "&width=128&height=128";
        WWW www = new WWW(gamerpicUrl);
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