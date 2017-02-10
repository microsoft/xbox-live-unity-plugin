using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Xbox.Services;

using SyntaxTree.VisualStudio.Unity.Bridge;

using UnityEditor;

using UnityEngine;

[InitializeOnLoad]
public class XboxLiveConfigurationEditor : EditorWindow
{
    private string configFileDirectory;
    private string configFilePath;

    [MenuItem("Xbox Live/Configuration")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<XboxLiveConfigurationEditor>("Xbox Live");
    }

    private XboxLiveAppConfiguration configuration;

    private bool IsConfigured
    {
        get
        {
            return this.configuration != null;
        }
    }

    private void OnEnable()
    {
        this.configFileDirectory = Path.Combine(Application.dataPath, "..");
        this.configFilePath = Path.Combine(this.configFileDirectory, XboxLiveAppConfiguration.FileName);
        this.configuration = this.TryLoad();

        // Start a file system watcher to notify us if we need to reload the configuration file.
        FileSystemWatcher configFileWatcher = new FileSystemWatcher(this.configFileDirectory)
        {
            Filter = XboxLiveAppConfiguration.FileName
        };
        configFileWatcher.Created += this.ConfigFileChanged;
        configFileWatcher.Deleted += this.ConfigFileChanged;
        configFileWatcher.Renamed += this.ConfigFileChanged;
        configFileWatcher.Changed += this.ConfigFileChanged;
        configFileWatcher.EnableRaisingEvents = true;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        string associateButtonText = this.IsConfigured ? "Update Xbox Live Association" : "Enable Xbox Live";

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent(associateButtonText, "Run the Xbox Live Assocation Wizard to enable your game to communicate with Xbox Live."), GUILayout.MaxWidth(200)))
        {
            string wizardPath = Path.Combine(Application.dataPath, "Xbox Live/Tools/AssociationWizard/AssociationWizard.exe");
            Process.Start(wizardPath, this.configFileDirectory);
        }

        if (File.Exists(this.configFilePath))
        {
            if (GUILayout.Button(new GUIContent("Delete Xbox Live Configuration", "Delete the configuration file containing the Xbox Live identity for your game."), GUILayout.MaxWidth(200)))
            {
                File.Delete(this.configFilePath);
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (this.IsConfigured)
        {
            GUILayout.Label("Title Identity", EditorStyles.boldLabel);

            this.PropertyLabel("App ID", this.configuration.AppId);
            this.PropertyLabel("Product Family Name", this.configuration.ProductFamilyName);
            this.PropertyLabel("SCID", this.configuration.ServiceConfigurationId);
            this.PropertyLabel("Title ID", this.configuration.TitleId.ToString());
            this.PropertyLabel("Sandbox", this.configuration.Sandbox);
            this.PropertyLabel("Environment ", this.configuration.Environment);
        }
        else
        {
            EditorGUILayout.HelpBox("In order to use Xbox Live functionality within your game, you must first associate it with a new or existing Xbox Live title.\nThis title information identifies your game with Xbox Live services and allows users to interact with Xbox Live and also allows you to interact with", MessageType.Info, true);
        }

        // Always show a button to manually open the configuration
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Edit " + XboxLiveAppConfiguration.FileName, "Manually create/edit the configuration file if you already have your configuration values"), GUILayout.MaxWidth(200)))
        {
            if (!File.Exists(this.configFilePath))
            {
                try
                {
                    const string emptyConfig = @"{
  ""AppId"": """",
  ""ProductFamilyName"": """",
  ""ServiceConfigurationId"": ""00000000-0000-0000-0000-0000694f5acb"",
  ""TitleId"": ""0000000000"",
  ""Sandbox"": ""XXXXXX.X"",
  ""Environment"": """",
}";
                    File.WriteAllText(this.configFilePath, emptyConfig);
                }
                catch (ArgumentException)
                {
                }
            }

            Process.Start(this.configFilePath);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("If you already have an existing configuration file, or you know the values you need to fill out, you can create/edit the configuration file manually.", MessageType.Info, true);

        if(this.IsConfigured)
        { 
            /*
            bool useMockData = GUILayout.Toggle(
                this.configuration.UseMockData,
                new GUIContent("Use Mock Xbox Live Data", "If checked, calls to Xbox Live will be faked to provide a small hard-coded set of data to evaluate Xbox Live functionality before completing the full configuration"));

            if (this.configuration.UseMockData != useMockData)
            {
                this.configuration.UseMockData = useMockData;
                this.configuration.Save();
            }
            */
        }
    }

    private void PropertyLabel(string name, string value)
    {
        const int labelHeight = 18;
        const string missingValue = "<empty>";

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(name);
        EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(value) ? missingValue : value, GUILayout.Height(labelHeight));
        EditorGUILayout.EndHorizontal();
    }

    private void ConfigFileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
        // When the config file changes, reload the configuration.
        this.configuration = this.TryLoad();
    }

    private XboxLiveAppConfiguration TryLoad()
    {
        try
        {
            return XboxLiveAppConfiguration.Load(this.configFilePath);
        }
        catch (Exception)
        {
            return null;
        }
    }
}