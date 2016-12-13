// -----------------------------------------------------------------------
//  <copyright file="UserProfile.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Threading.Tasks;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.System;

using UnityEngine;
using UnityEngine.UI;

public class UserProfile : MonoBehaviour
{
    public GameObject signInPanel;
    public GameObject profileInfoPanel;
    public Image gamerpic;
    public Text gamertag;

    public XboxLiveUser CurrentUser;

    public void Start()
    {
        Debug.Log("Creating user");
        this.CurrentUser = new XboxLiveUser();
    }

    public void Awake()
    {
        Debug.Log("Awake");
        this.profileInfoPanel.SetActive(false);
    }

    public void SignIn()
    {
        this.StartCoroutine(this.SignInAsync());
    }

    public IEnumerator SignInAsync()
    {
        // Disable the sign-in button
        this.signInPanel.GetComponentInChildren<Button>().interactable = false;

        yield return this.CurrentUser.SignInAsync(IntPtr.Zero).AsCoroutine();

        this.CurrentUser.Gamertag = "Veleek";
        XboxLive.Instance.Context = new XboxLiveContext(this.CurrentUser);

        yield return this.LoadProfileInfo();
    }

    private IEnumerator LoadProfileInfo()
    {
        WWW www = new WWW("http://images-eds.xboxlive.com/image?url=z951ykn43p4FqWbbFvR2Ec.8vbDhj8G2Xe7JngaTToBrrCmIEEXHC9UNrdJ6P7KId46ktn4AUxk.ghIPeRshxRqsosbBN.5ygjwRBcUg7G_su3phiVsbouqmSe7ZRa8o&format=png");
        yield return www;

        Texture2D t = www.texture;
        Rect r = new Rect(0, 0, t.width, t.height);
        this.gamerpic.sprite = Sprite.Create(t, r, Vector2.zero);
        this.gamertag.text = this.CurrentUser.Gamertag;

        this.signInPanel.SetActive(false);
        this.profileInfoPanel.SetActive(true);
    }
}