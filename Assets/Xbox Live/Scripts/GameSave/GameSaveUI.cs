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

public class GameSaveUI : MonoBehaviour
{
    private GameSaveHelper gameSaveHelper;

    public string GameSaveContainerName = "TestContainer";

    public string GameSaveBlobName = "TestBlob";

    private GUIStyle guiStyle;
    private string logText;
    private System.Random random;
    private int gameData;
    private bool initializing;
    private List<string> logLines;
    private XboxLiveUser xboxLiveUser;

    // Use this for initialization
    void Start () {
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
        this.xboxLiveUser = XboxLiveComponent.Instance.User;
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
                }));
        }
        catch (Exception ex)
        {
            this.LogLine("InitializeSaveSystem failed: " + ex.Message);
        }
        this.LogLine("");
    }

    // Update is called once per frame
    void Update()
    {
        if (XboxLiveComponent.Instance.User != null && XboxLiveComponent.Instance.User.IsSignedIn && !this.gameSaveHelper.IsInitialized() && !this.initializing)
        {
            this.InitializeSaveSystem();
        }
    }

    private void DrawTextWithShadow(float x, float y, float width, float height, string text)
    {
        this.guiStyle.fontSize = 14;
        this.guiStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(x, y, height, height), text, this.guiStyle);
        this.guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(x - 1, y - 1, width, height), text, this.guiStyle);
    }

    void OnGUI()
    {
        lock (this.logText)
        {
            this.DrawTextWithShadow(10, 400, 800, 900, this.logText);
        }
        if (this.gameSaveHelper.IsInitialized())
        {
            if (GUI.Button(new Rect(10, 150, 150, 30), "Generate Data"))
            {
                this.gameData = this.random.Next();
                this.LogLine(string.Format("Game data: {0}", this.gameData));
                this.LogLine("");
            }

            if (GUI.Button(new Rect(10, 190, 150, 30), "Save Data"))
            {
                this.SaveData();
            }

            if (GUI.Button(new Rect(10, 230, 150, 30), "Load Data"))
            {
                this.LoadData();
            }

            if (GUI.Button(new Rect(10, 270, 150, 30), "Get Container Info"))
            {
                this.GetContainerInfo();
            }

            if (GUI.Button(new Rect(10, 310, 150, 30), "Delete Container"))
            {
                this.DeleteContainer();
            }
        }
    }

    private void SaveData()
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

        this.LogLine("");
    }

    private void LoadData()
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

        this.LogLine("");
    }

    private void GetContainerInfo()
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

        this.LogLine("");
    }

    private void LogSaveContainerInfoList(List<StorageContainerInfo> list)
    {
        if (list.Count == 0)
        {
            this.LogLine("[Empty ContainerStagingInfo list]");
            this.LogLine("");
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

    private void DeleteContainer()
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

        this.LogLine("");
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
                this.logText += "\n";
                this.logText += s;
            }
            this.logText += "\n";
        }
    }
}
