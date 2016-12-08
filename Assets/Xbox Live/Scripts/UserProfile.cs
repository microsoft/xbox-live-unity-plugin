using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.System;
using System;

public class UserProfile : MonoBehaviour {

	public Texture2D gamerPic;

	public XboxLiveUser CurrentUser;

	// Use this for initialization
	void Start () {
		Debug.Log ("Creating user");
		this.CurrentUser = new XboxLiveUser ();
	}
	
	public void SignIn()
	{
		this.CurrentUser.SignInAsync (IntPtr.Zero).Wait();

		this.CurrentUser.Gamertag = "Veleek";
		this.CurrentUser.IsSignedIn = true;
		XboxLive.Instance.Context = new XboxLiveContext (this.CurrentUser);

		Debug.Log ("Signed In");
	}
}
