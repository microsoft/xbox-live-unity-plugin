// -----------------------------------------------------------------------
//  <copyright file="SocialManagerComponent.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Internal use only.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;

public class SocialManagerComponent : MonoBehaviour
{
    private XboxSocialUserGroup group;

    public void DoWork()
    {
        try
        {
            if (this.group == null)
            {
                ulong userId = UInt64.Parse(XboxLive.Instance.User.XboxUserId);
                this.group = SocialManager.Instance.CreateSocialUserGroupFromList(XboxLive.Instance.User, new List<ulong> { userId });
            }

            var result = SocialManager.Instance.DoWork();
            Debug.Log(string.Format("SocialManager processed {0} events", result.Count));

            if (this.group != null)
            {
                foreach (XboxSocialUser user in this.group)
                {
                    Debug.Log(user.DisplayName);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}