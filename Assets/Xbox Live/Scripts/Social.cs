// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using global::System.Collections;
using global::System.Collections.Generic;
using System.Globalization;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;
using UnityEngine.UI;

public class Social : MonoBehaviour
{
    public Dropdown presenceFilterDropdown;
    public Transform contentPanel;
    public ScrollRect scrollRect;
    public XboxLiveUserInfo XboxLiveUser;
    public string toggleFilterControllerButton;
    public string verticalScrollInputAxis;

    private Dictionary<int, XboxSocialUserGroup> socialUserGroups = new Dictionary<int, XboxSocialUserGroup>();
    private ObjectPool entryObjectPool;
    public float scrollSpeedMultiplier = 0.1f;

    private void Awake()
    {
        this.EnsureEventSystem();
        XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();
        this.entryObjectPool = this.GetComponent<ObjectPool>();
        SocialManagerComponent.Instance.EventProcessed += this.OnEventProcessed;

        this.presenceFilterDropdown.options.Clear();
        this.presenceFilterDropdown.options.Add(new Dropdown.OptionData() { text = PresenceFilter.All.ToString() });
        this.presenceFilterDropdown.options.Add(new Dropdown.OptionData() { text = "All Online" });
        this.presenceFilterDropdown.value = 0;
        this.presenceFilterDropdown.RefreshShownValue();

        this.presenceFilterDropdown.onValueChanged.AddListener(delegate
        {
            this.PresenceFilterValueChangedHandler(this.presenceFilterDropdown);
        });
    }

    private void Start()
    {
        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }

        if (this.XboxLiveUser != null && this.XboxLiveUser.User != null && this.XboxLiveUser.User.IsSignedIn)
        {
            this.CreateDefaultSocialGraphs();
            this.RefreshSocialGroups();
        }
    }


    private void Update()
    {
        if (!string.IsNullOrEmpty(this.verticalScrollInputAxis) && Input.GetAxis(this.verticalScrollInputAxis) != 0){
            var inputValue = Input.GetAxis(this.verticalScrollInputAxis);
            this.scrollRect.verticalScrollbar.value = this.scrollRect.verticalNormalizedPosition + inputValue * scrollSpeedMultiplier;
        }

        if (!string.IsNullOrEmpty(this.toggleFilterControllerButton) && Input.GetKeyDown(this.toggleFilterControllerButton)) {
            switch (this.presenceFilterDropdown.value) {
                case 0: this.presenceFilterDropdown.value = 1; break;
                case 1: this.presenceFilterDropdown.value = 0; break;
            }
        }
    }

    private void OnEventProcessed(object sender, SocialEvent socialEvent)
    {
        if (this.XboxLiveUser == null)
        {
            this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
        }

        if (this.XboxLiveUser != null && this.XboxLiveUser.User != null && socialEvent.User.Gamertag == this.XboxLiveUser.User.Gamertag)
        {
            switch (socialEvent.EventType)
            {
                case SocialEventType.LocalUserAdded:
                    if (socialEvent.Exception == null)
                    {
                        this.CreateDefaultSocialGraphs();
                    }
                    break;
                case SocialEventType.SocialUserGroupLoaded:
                case SocialEventType.SocialUserGroupUpdated:
                case SocialEventType.PresenceChanged:
                    this.RefreshSocialGroups();
                    break;
            }
        }
        else
        {
            if (this.XboxLiveUser == null)
            {
                this.XboxLiveUser = XboxLiveUserManager.Instance.GetSingleModeUser();
            }
        }
    }

    private void PresenceFilterValueChangedHandler(Dropdown target)
    {
        this.RefreshSocialGroups();
    }

    private void CreateDefaultSocialGraphs()
    {
        XboxSocialUserGroup allSocialUserGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromFilters(this.XboxLiveUser.User, PresenceFilter.All, RelationshipFilter.Friends);
        this.socialUserGroups.Add(0, allSocialUserGroup);

        XboxSocialUserGroup allOnlineSocialUserGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromFilters(this.XboxLiveUser.User, PresenceFilter.AllOnline, RelationshipFilter.Friends);
        this.socialUserGroups.Add(1, allOnlineSocialUserGroup);
    }

    private void RefreshSocialGroups()
    {
        XboxSocialUserGroup socialUserGroup;
        if (!this.socialUserGroups.TryGetValue(this.presenceFilterDropdown.value, out socialUserGroup) && XboxLiveServicesSettings.Instance.DebugLogsOn)
        {
            Debug.Log("An Exception Occured: Invalid Presence Filter selected");
            return;
        }

        while (this.contentPanel.childCount > 0)
        {
            var entry = this.contentPanel.GetChild(0).gameObject;
            this.entryObjectPool.ReturnObject(entry);
        }

        foreach (XboxSocialUser user in socialUserGroup.Users)
        {
            GameObject entryObject = this.entryObjectPool.GetObject();
            XboxSocialUserEntry entry = entryObject.GetComponent<XboxSocialUserEntry>();

            entry.Data = user;
            entryObject.transform.SetParent(this.contentPanel);
        }

        // Reset the scroll view to the top.
        this.scrollRect.verticalNormalizedPosition = 1;
    }

    public static Color ColorFromHexString(string color)
    {
        float r = (float)byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber) / 255;
        float g = (float)byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255;
        float b = (float)byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255;

        return new Color(r, g, b);
    }
}