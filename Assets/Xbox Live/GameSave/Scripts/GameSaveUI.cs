#if NETFX_CORE
using System.Linq;
using Windows.System;
#endif

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.ConnectedStorage;

using UnityEngine;
using UnityEngine.UI;

public class GameSaveUI : MonoBehaviour
{
    private GameSaveHelper gameSaveHelper;

    public string GameSaveContainerName = "TestContainer";
    public string GameSaveBlobName = "TestBlob";
    public Text Console;
    public Scrollbar ScrollBar;
    public RectTransform ScrollRect;
    public XboxLiveUserInfo XboxLiveUser;

    private GUIStyle guiStyle;
    private string logText;
    private System.Random random;
    private int gameData;
    private bool initializing;
    private List<string> logLines;

    // Use this for initialization
    void Start()
    {
        this.logText = string.Empty;
        this.guiStyle = new GUIStyle();
        this.random = new System.Random();
        this.gameSaveHelper = new GameSaveHelper();
        this.logLines = new List<string>();
    }

    public void InitializeSaveSystem()
    {
        this.initializing = true;

        // Game Saves require a Windows System User
        try
        {
            if (this.gameSaveHelper.IsInitialized())
            {
                this.LogLine("Save System is already initialized.");
                return;
            }

            this.LogLine("Initializing save system...");
            this.StartCoroutine(this.gameSaveHelper.Initialize(this.XboxLiveUser.User,
                r =>
                    {
                        var status = r;
                        this.LogLine(
                            status == GameSaveStatus.Ok
                                ? "Successfully initialized save system."
                                : string.Format("InitializeSaveSystem failed: {0}", status));

                        if (status != GameSaveStatus.Ok)
                        {
                            this.initializing = false;
                        }
                    }));
        }
        catch (Exception ex)
        {
            this.LogLine("InitializeSaveSystem failed: " + ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (XboxLiveUserHelper.Instance.SingleUserModeEnabled
            && XboxLiveUserHelper.Instance.SingleXboxLiveUser.User != null)
        {
            this.XboxLiveUser = XboxLiveUserHelper.Instance.SingleXboxLiveUser;
        }

        if (this.XboxLiveUser != null && this.XboxLiveUser.User != null && this.XboxLiveUser.User.IsSignedIn && !this.gameSaveHelper.IsInitialized() && !this.initializing)
        {
            this.InitializeSaveSystem();
            this.initializing = false;
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
            this.gameData = this.random.Next();
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
}
