// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if ENABLE_WINMD_SUPPORT
using System.Linq;
using Windows.System;
#endif

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xbox.Services.ConnectedStorage;

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    public class GameSaveUI : MonoBehaviour
    {
        private GameSaveHelper gameSaveHelper;
        public int PlayerNumber = 1;
        public string GameSaveContainerName = "TestContainer";
        public string GameSaveBlobName = "TestBlob";
        public Text Console;
        public Scrollbar ScrollBar;
        public RectTransform ScrollRect;
        public bool EnableControllerInput = false;
        public int JoystickNumber = 1;
        public XboxControllerButtons GenerateDataButton;
        public XboxControllerButtons SaveDataButton;
        public XboxControllerButtons LoadDataButton;
        public XboxControllerButtons GetInfoButton;
        public XboxControllerButtons DeleteContainerButton;

        private string generateNewControllerButton;
        private string saveDataControllerButton;
        private string loadDataControllerButton;
        private string getInfoControllerButton;
        private string deleteContainerControllerButton;

        private string logText;
        private int gameData;
        private bool initializing;
        private List<string> logLines;
        private XboxLiveUser xboxLiveUser;
        // Use this for initialization
        void Start()
        {
            XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();
            this.logText = string.Empty;
            this.gameSaveHelper = new GameSaveHelper();
            this.logLines = new List<string>();

            this.xboxLiveUser = SignInManager.Instance.GetPlayer(this.PlayerNumber);
            if (this.xboxLiveUser != null && this.xboxLiveUser.IsSignedIn)
            {
                this.InitializeSaveSystem();
            }
            else
            {
                SignInManager.Instance.OnPlayerSignIn(this.PlayerNumber, this.OnPlayerSignIn);
                SignInManager.Instance.OnPlayerSignOut(this.PlayerNumber, this.OnPlayerSignOut);
            }

            if (this.EnableControllerInput)
            {
                if (this.GenerateDataButton != XboxControllerButtons.None)
                {
                    this.generateNewControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.GenerateDataButton);
                }

                if (this.SaveDataButton != XboxControllerButtons.None)
                {
                    this.saveDataControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.SaveDataButton);
                }

                if (this.LoadDataButton != XboxControllerButtons.None)
                {
                    this.loadDataControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.LoadDataButton);
                }

                if (this.GetInfoButton != XboxControllerButtons.None)
                {
                    this.getInfoControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.GetInfoButton);
                }

                if (this.DeleteContainerButton != XboxControllerButtons.None)
                {
                    this.deleteContainerControllerButton = "joystick " + this.JoystickNumber + " button " + XboxControllerConverter.GetUnityButtonNumber(this.DeleteContainerButton);
                }
            }
        }

        public void OnPlayerSignIn(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus signinStatus, string errorMessage)
        {
            if (signinStatus == XboxLiveAuthStatus.Succeeded)
            {
                this.xboxLiveUser = xboxLiveUser;
                this.InitializeSaveSystem();
            }
        }

        public void InitializeSaveSystem()
        {
            // Game Saves require a Windows System User
            try
            {
                if (this.gameSaveHelper.IsInitialized())
                {
                    this.LogLine("Save System is already initialized.");
                    return;
                }

                this.LogLine("Initializing save system...");
                this.StartCoroutine(this.gameSaveHelper.Initialize(this.xboxLiveUser,
                    r =>
                    {
                        var status = r;
                        this.LogLine(
                            status == GameSaveStatus.Ok
                                ? "Successfully initialized save system."
                                : string.Format("InitializeSaveSystem failed: {0}", status));

                        this.initializing = false;

                    }));
            }
            catch (Exception ex)
            {
                this.LogLine("Initializing Save System failed: " + ex.Message);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this.EnableControllerInput)
            {
                if (!string.IsNullOrEmpty(this.generateNewControllerButton) && Input.GetKeyDown(this.generateNewControllerButton))
                {
                    this.GenerateData();
                }

                if (!string.IsNullOrEmpty(this.saveDataControllerButton) && Input.GetKeyDown(this.saveDataControllerButton))
                {
                    this.SaveData();
                }

                if (!string.IsNullOrEmpty(this.loadDataControllerButton) && Input.GetKeyDown(this.loadDataControllerButton))
                {
                    this.LoadData();
                }

                if (!string.IsNullOrEmpty(this.getInfoControllerButton) && Input.GetKeyDown(this.getInfoControllerButton))
                {
                    this.GetContainerInfo();
                }

                if (!string.IsNullOrEmpty(this.deleteContainerControllerButton) && Input.GetKeyDown(this.deleteContainerControllerButton))
                {
                    this.DeleteContainer();
                }
            }
        }

        void OnGUI()
        {
            lock (this.logText)
            {
                this.Console.text = this.logText;
            }
        }

        public void GenerateData()
        {
            if (this.gameSaveHelper.IsInitialized())
            {
                this.gameData = UnityEngine.Random.Range(111111,999999);
                this.LogLine(string.Format("Game data: {0}", this.gameData));
            }
            else
            {
                this.LogLine("Game Save Helper hasn't been initialized yet. Please Sign In first.");
            }
        }

        public void SaveData()
        {
            if (this.gameSaveHelper.IsInitialized())
            {
                try
                {
                    var contentToSave = new Dictionary<string, byte[]>
                                    {
                                        {
                                            this.GameSaveBlobName,
                                            Encoding.UTF8.GetBytes(this.gameData.ToString())
                                        }
                                    };

                    this.StartCoroutine(this.gameSaveHelper.SubmitUpdates(this.GameSaveContainerName, contentToSave, null,
                        saveResultStatus =>
                        {
                            this.LogLine(
                                    saveResultStatus == GameSaveStatus.Ok
                                        ? string.Format("Saved data : {0}", this.gameData)
                                        : string.Format("SaveDataForUser failed: {0}", saveResultStatus));
                        }));

                }
                catch (Exception ex)
                {
                    this.LogLine("SaveDataForUser failed: " + ex.Message);
                }
            }
            else
            {
                this.LogLine("Game Save Helper hasn't been initialized yet. Please Sign In first.");
            }
        }

        public void LoadData()
        {
            if (this.gameSaveHelper.IsInitialized())
            {
                try
                {
                    this.StartCoroutine(this.gameSaveHelper.GetAsStrings(this.GameSaveContainerName, new[] { this.GameSaveBlobName },
                        loadResultDictionary =>
                        {
                            try
                            {
                                var blobLoadResult = loadResultDictionary[this.GameSaveBlobName];
#if !UNITY_EDITOR
                            this.gameData = int.Parse(blobLoadResult);
#endif
                            this.LogLine(string.Format("Loaded data : {0}", this.gameData));
                            }
                            catch (Exception ex)
                            {
                                this.LogLine(ex.Message);
                            }
                        }));
                }
                catch (Exception ex)
                {
                    this.LogLine("LoadData failed: " + ex.Message);
                }
            }
            else
            {
                this.LogLine("Game Save Helper hasn't been initialized yet. Please Sign In first.");
            }

        }

        public void GetContainerInfo()
        {
            if (this.gameSaveHelper.IsInitialized())
            {
                try
                {
                    this.StartCoroutine(this.gameSaveHelper.GetContainerInfo(string.Empty,
                        result =>
                        {
                            if (result.Status == GameSaveStatus.Ok)
                            {
                                this.LogLine("Got container info:");
                                this.LogLine("");
                                this.LogSaveContainerInfoList(result.Value);
                            }
                            else
                            {
                                this.LogLine(string.Format("GetContainerInfo failed: {0}", result.Status));
                            }
                        }));
                }
                catch (Exception ex)
                {
                    this.LogLine("GetContainerInfo failed: " + ex.Message);
                }
            }
            else
            {
                this.LogLine("Game Save Helper hasn't been initialized yet. Please Sign In first.");
            }

        }

        private void LogSaveContainerInfoList(List<StorageContainerInfo> list)
        {
            if (list.Count == 0)
            {
                this.LogLine("[Empty ContainerStagingInfo list]");
            }

            for (var i = 0; i < list.Count; i++)
            {
                this.LogLine(string.Format("Container #{0}", i));
                this.LogLine("Name: " + list[i].Name);
                this.LogLine("DisplayName: " + list[i].DisplayName);
                this.LogLine(string.Format("LastModifiedTime: {0}", list[i].LastModifiedTime));
                this.LogLine(string.Format("TotalSize: {0}", list[i].TotalSize));
                this.LogLine(string.Format("NeedsSync: {0}", list[i].NeedsSync));
                this.LogLine("");
            }
        }

        public void DeleteContainer()
        {
            if (this.gameSaveHelper.IsInitialized())
            {
                try
                {
                    this.StartCoroutine(this.gameSaveHelper.DeleteContainer(this.GameSaveContainerName,
                        deleteStatus =>
                        {
                            this.LogLine(
                                    deleteStatus == GameSaveStatus.Ok
                                        ? "Deleted save container."
                                        : string.Format("DeleteContainer failed: {0}", deleteStatus));
                        }));


                }
                catch (Exception ex)
                {
                    this.LogLine("DeleteContainer failed: " + ex.Message);
                }
            }
            else
            {
                this.LogLine("Game Save Helper hasn't been initialized yet. Please Sign In first.");
            }
        }



        public void LogLine(string line)
        {
            lock (this.logText)
            {
                if (this.logLines.Count > 30)
                {
                    this.logLines.RemoveAt(0);
                }
                this.logLines.Add(line);

                this.logText = string.Empty;
                foreach (var s in this.logLines)
                {
                    this.logText += s;
                    this.logText += "\n";
                }
            }
        }

        private void OnPlayerSignOut(XboxLiveUser xboxLiveUser, XboxLiveAuthStatus authStatus, string errorMessage)
        {
            if (authStatus == XboxLiveAuthStatus.Succeeded)
            {
                this.xboxLiveUser = null;
                this.initializing = false;
                lock (this.logText)
                {
                    this.logLines.Clear();
                    this.logText = string.Empty;
                }
            }
        }

        private void OnDestroy()
        {
            SignInManager.Instance.RemoveCallbackFromPlayer(this.PlayerNumber, this.OnPlayerSignIn);
            SignInManager.Instance.RemoveCallbackFromPlayer(this.PlayerNumber, this.OnPlayerSignOut);
        }
    }
}