// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using global::System.Collections.Generic;
using System.Globalization;
using Microsoft.Xbox.Services.Social.Manager;

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    public class Social : MonoBehaviour
    {
        public Dropdown presenceFilterDropdown;
        public Transform contentPanel;
        public ScrollRect scrollRect;

        public bool EnableControllerInput = false;

        public int JoystickNumber = 1;

        public XboxControllerButtons ToggleFilterButton;

        public int PlayerNumber = 1;

        public string verticalScrollInputAxis;

        private string toggleFilterControllerButton;


        private Dictionary<int, XboxSocialUserGroup> socialUserGroups = new Dictionary<int, XboxSocialUserGroup>();
        private ObjectPool entryObjectPool;
        public float scrollSpeedMultiplier = 0.1f;
        private XboxLiveUser xboxLiveUser;
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


            if (this.EnableControllerInput)
            {
                if (this.ToggleFilterButton != XboxControllerButtons.None)
                {
                    this.toggleFilterControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.ToggleFilterButton);
                }
            }

            SignInManager.Instance.OnPlayerSignOut(this.PlayerNumber, this.OnPlayerSignOut);
        }

        private void Start()
        {
            this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);

            if (this.xboxLiveUser != null && this.xboxLiveUser.IsSignedIn)
            {
                this.CreateDefaultSocialGraphs();
                this.RefreshSocialGroups();
            }
        }


        private void Update()
        {
            if (this.EnableControllerInput)
            {
                if (!string.IsNullOrEmpty(this.verticalScrollInputAxis) && Input.GetAxis(this.verticalScrollInputAxis) != 0)
                {
                    var inputValue = Input.GetAxis(this.verticalScrollInputAxis);
                    this.scrollRect.verticalScrollbar.value = this.scrollRect.verticalNormalizedPosition + inputValue * scrollSpeedMultiplier;
                }

                if (!string.IsNullOrEmpty(this.toggleFilterControllerButton) && Input.GetKeyDown(this.toggleFilterControllerButton))
                {
                    switch (this.presenceFilterDropdown.value)
                    {
                        case 0: this.presenceFilterDropdown.value = 1; break;
                        case 1: this.presenceFilterDropdown.value = 0; break;
                    }
                }
            }
        }

        private void OnEventProcessed(object sender, SocialEvent socialEvent)
        {
            this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);

            if (this.xboxLiveUser != null && socialEvent.User.Gamertag == this.xboxLiveUser.Gamertag)
            {
                switch (socialEvent.EventType)
                {
                    case SocialEventType.LocalUserAdded:
                        if (socialEvent.ErrorCode == 0)
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
                this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            }
        }

        private void OnPlayerSignOut(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus xboxAuthStatus, string errorMessage)
        {
            if (xboxAuthStatus == XboxLiveAuthStatus.Succeeded)
            {
                this.xboxLiveUser = null;
                this.presenceFilterDropdown.value = 0;
                this.presenceFilterDropdown.RefreshShownValue();

                this.RefreshSocialGroups();
            }
            else
            {
                if (XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.LogError(errorMessage);
                }
            }
        }

        private void PresenceFilterValueChangedHandler(Dropdown target)
        {
            this.RefreshSocialGroups();
        }

        private void CreateDefaultSocialGraphs()
        {
            XboxSocialUserGroup allSocialUserGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromFilters(this.xboxLiveUser, PresenceFilter.All, RelationshipFilter.Friends);
            this.socialUserGroups.Add(0, allSocialUserGroup);

            XboxSocialUserGroup allOnlineSocialUserGroup = XboxLive.Instance.SocialManager.CreateSocialUserGroupFromFilters(this.xboxLiveUser, PresenceFilter.AllOnline, RelationshipFilter.Friends);
            this.socialUserGroups.Add(1, allOnlineSocialUserGroup);
        }

        private void RefreshSocialGroups()
        {
            if (this.xboxLiveUser != null)
            {
                XboxSocialUserGroup socialUserGroup;
                if (!this.socialUserGroups.TryGetValue(this.presenceFilterDropdown.value, out socialUserGroup) && XboxLiveServicesSettings.Instance.DebugLogsOn)
                {
                    Debug.Log("An Exception Occured: Invalid Presence Filter selected");
                    return;
                }
                
                Debug.LogWarning("Content Panel = " + this.contentPanel);

                try
                {
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
                }
                catch (Exception ex)
                {
                    Debug.LogError("An exception occured: " + ex.ToString());
                }
            }
            else
            {
                this.socialUserGroups = null;
                var children = new List<GameObject>();
                for (int i = 0; i < this.contentPanel.childCount; i++)
                {
                    GameObject child = this.contentPanel.transform.GetChild(i).gameObject;
                    children.Add(child);
                }

                this.contentPanel.DetachChildren();

                foreach (var child in children)
                {
                    Destroy(child);
                }
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

        private void OnDestroy()
        {
            if (SocialManagerComponent.Instance != null)
            {
                SocialManagerComponent.Instance.EventProcessed -= this.OnEventProcessed;
            }
            if (SignInManager.Instance != null)
            {
                SignInManager.Instance.RemoveCallbackFromPlayer(this.PlayerNumber, this.OnPlayerSignOut);
            }
        }
    }
}