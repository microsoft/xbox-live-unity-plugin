using System.Diagnostics;
using System.IO;

using UnityEditor;

using UnityEngine;

[InitializeOnLoad]
public class XboxLiveConfigurationEditor : EditorWindow
{
    private const string MissingValue = "<missing>";

    [MenuItem("Xbox Live/Configuration")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<XboxLiveConfigurationEditor>("Xbox Live");
    }

    private XboxServicesConfiguration configuration;

    private bool IsConfigured
    {
        get
        {
            return this.configuration != null;
        }
    }

    private void OnEnable()
    {
        this.configuration = XboxServicesConfiguration.Load();

        FileSystemWatcher configFileWatcher = new FileSystemWatcher(Application.dataPath)
        {
            Filter = XboxServicesConfiguration.ConfigurationFileName
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
            Process.Start(wizardPath, Application.dataPath);
        }

        if (File.Exists(XboxServicesConfiguration.ConfigurationFilePath))
        {
            if (GUILayout.Button(new GUIContent("Delete Xbox Live Configuration", "Delete the configuration file containing the Xbox Live identity for your game."), GUILayout.MaxWidth(200)))
            {
                // Delete the Xbox Services configuration file from disc to disassociate a game from a store product.  
                // Also deletes the Unity .meta file so that it won't complain about a missing file.
                AssetDatabase.DeleteAsset(string.Format("Assets/{0}", XboxServicesConfiguration.ConfigurationFileName));
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (this.IsConfigured)
        {
            const int labelHeight = 18;
            GUILayout.Label("Title Identity", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Title Id");
            EditorGUILayout.SelectableLabel(this.configuration.TitleId ?? MissingValue, GUILayout.Height(labelHeight));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Scid");
            EditorGUILayout.SelectableLabel(this.configuration.ServiceConfigurationId ?? MissingValue, GUILayout.Height(labelHeight));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Product Family Name");
            EditorGUILayout.SelectableLabel(this.configuration.ProductFamilyName ?? MissingValue, GUILayout.Height(labelHeight));
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("In order to use Xbox Live functionality within your game, you must first associate it with a new or existing Xbox Live title.\nThis title information identifies your game with Xbox Live services and allows users to interact with Xbox Live and also allows you to interact with", MessageType.Info, true);
        }

        // Always show a button to manually open the configuration
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Edit " + XboxServicesConfiguration.ConfigurationFileName, "Manually create/edit the configuration file if you already have your configuration values"), GUILayout.MaxWidth(200)))
        {
            if (!File.Exists(XboxServicesConfiguration.ConfigurationFilePath))
            {
                this.configuration = new XboxServicesConfiguration
                {
                    TitleId = "<Decimal>",
                    ServiceConfigurationId = "<Guid>",
                    ProductFamilyName = "<String>",
                    UseMockData = true,
                };
                this.configuration.Save();
            }

            Process.Start(XboxServicesConfiguration.ConfigurationFilePath);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("If you already have an existing configuration file, or you know the values you need to fill out, you can create/edit the configuration file manually.", MessageType.Info, true);

        if(this.IsConfigured)
        { 
            bool useMockData = GUILayout.Toggle(
                this.configuration.UseMockData,
                new GUIContent("Use Mock Xbox Live Data", "If checked, calls to Xbox Live will be faked to provide a small hard-coded set of data to evaluate Xbox Live functionality before completing the full configuration"));

            if (this.configuration.UseMockData != useMockData)
            {
                this.configuration.UseMockData = useMockData;
                this.configuration.Save();
            }
        }
    }

    private void ConfigFileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
        // When the config file changes, reload the configuration.
        this.configuration = XboxServicesConfiguration.Load();
    }
}