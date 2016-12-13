using System.Diagnostics;
using System.IO;

using UnityEditor;

using UnityEngine;

public class XboxLiveConfigurationEditor : EditorWindow
{
    private const string MissingValue = "<missing>";

    public bool IsXboxLiveConfigured { get; private set; }

    public XboxServicesConfiguration ServiceIdentity { get; set; }

    [MenuItem("Xbox Live/Configuration")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<XboxLiveConfigurationEditor>("Xbox Live");
    }

    private void OnEnable()
    {
        this.LoadConfiguration();

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

    private void LoadConfiguration()
    {
        this.ServiceIdentity = File.Exists(XboxServicesConfiguration.ConfigurationFilePath) ? XboxServicesConfiguration.Load () : null;
        this.IsXboxLiveConfigured = this.ServiceIdentity != null;

        //Debug.Log ("Configuration Loaded? " + this.IsXboxLiveConfigured);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space ();

        string associateButtonText = this.IsXboxLiveConfigured ? "Update Xbox Live Association" : "Enable Xbox Live";

        EditorGUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();

        if (GUILayout.Button(associateButtonText, GUILayout.MaxWidth(200)))
        {
            string wizardPath = Path.Combine(Application.dataPath, "Xbox Live/Tools/AssociationWizard/AssociationWizard.exe");
            Process.Start(wizardPath, Application.dataPath);
        }

        if (this.IsXboxLiveConfigured) {
            if (GUILayout.Button ("Reset Xbox Live Association", GUILayout.MaxWidth(200))) {
                XboxServicesConfiguration.Clear ();
            }
        }

        GUILayout.FlexibleSpace ();
        EditorGUILayout.EndHorizontal ();
        
        if (this.IsXboxLiveConfigured) {
            const int labelHeight = 18;
            GUILayout.Label ("Title Identity", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.PrefixLabel ("Title Id");
            EditorGUILayout.SelectableLabel (this.ServiceIdentity.TitleId ?? MissingValue, GUILayout.Height (labelHeight));
            EditorGUILayout.EndHorizontal ();


            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.PrefixLabel ("Scid");
            EditorGUILayout.SelectableLabel (this.ServiceIdentity.PrimaryServiceConfigId ?? MissingValue, GUILayout.Height (labelHeight));
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.PrefixLabel ("Product Family Name");
            EditorGUILayout.SelectableLabel (this.ServiceIdentity.ProductFamilyName ?? MissingValue, GUILayout.Height (labelHeight));
            EditorGUILayout.EndHorizontal ();
        } else {
            EditorGUILayout.HelpBox ("In order to use Xbox Live functionality within your game, you must first associate it with a new or existing Xbox Live title.\nThis title information identifies your game with Xbox Live services and allows users to interact with Xbox Live and also allows you to interact with", MessageType.Info, true);
        }
    }

    private void ConfigFileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
        //Debug.Log("XboxServices.config file " + fileSystemEventArgs.ChangeType);
        this.LoadConfiguration ();
    }
}