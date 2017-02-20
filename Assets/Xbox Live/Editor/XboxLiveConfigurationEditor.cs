// -----------------------------------------------------------------------
//  <copyright file="XboxLiveConfigurationEditor.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Win32;
using Microsoft.Xbox.Services;

using UnityEditor;

using UnityEngine;

[InitializeOnLoad]
public class XboxLiveConfigurationEditor : EditorWindow
{
    private string configFileDirectory;
    private string configFilePath;
    private Vector2 scrollPosition;

    [MenuItem("Xbox Live/Configuration")]
    public static void ShowWindow()
    {
        GetWindow<XboxLiveConfigurationEditor>("Xbox Live");
    }

    internal XboxLiveAppConfiguration configuration;

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
        this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
        EditorGUILayout.Space();

        string associateButtonText = this.IsConfigured ? "Update Xbox Live Association" : "Enable Xbox Live";

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent(associateButtonText, "Run the Xbox Live Assocation Wizard to enable your game to communicate with Xbox Live."), GUILayout.MaxWidth(200)))
        {
            string wizardPath = Path.Combine(Application.dataPath, "Xbox Live/Tools/AssociationWizard/AssociationWizard.exe");
            // We need to make sure to quote the path that we pass to the association wizard.
            Process.Start(wizardPath, '"' + this.configFileDirectory + '"');
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
            if (string.IsNullOrEmpty(this.configuration.AppId))
            {
                EditorGUILayout.HelpBox("Your Xbox Live configuration does not appear to be completely valid.  You will need to re-associate your game before you can create a finished build.", MessageType.Warning, true);
            }

            GUILayout.Label("Title Configuration", EditorStyles.boldLabel);

            PropertyLabel("Name", this.configuration.DisplayName);
            PropertyLabel("Publisher", this.configuration.PublisherDisplayName);
            PropertyLabel("App ID", this.configuration.AppId);
            PropertyLabel("Product Family Name", this.configuration.ProductFamilyName);
            PropertyLabel("SCID", this.configuration.ServiceConfigurationId);
            PropertyLabel("Title ID", this.configuration.TitleId.ToString());
            PropertyLabel("Sandbox", this.configuration.Sandbox);
        }
        else
        {
            EditorGUILayout.HelpBox("In order to use Xbox Live functionality within your game, you must first associate it with a new or existing Xbox Live title.\nThis title information identifies your game with Xbox Live services and allows users to interact with Xbox Live.  You need a DevCenter account to associate your game, but you can create an empty configuration file to test functionality within Unity.", MessageType.Info, true);
        }

        // Always show a button to manually open the configuration
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (!File.Exists(this.configFilePath))
        {
            if (GUILayout.Button(new GUIContent("Create empty " + XboxLiveAppConfiguration.FileName, "Create an empty configuration file so that you can test Xbox Live in the editor before associating your game with DevCenter.")))
            {
                try
                {
                    const string emptyConfig = @"{
  ""AppId"": """",
  ""ProductFamilyName"": """",
  ""ServiceConfigurationId"": ""00000000-0000-0000-0000-0000694f5acb"",
  ""TitleId"": ""0000000000"",
  ""Sandbox"": ""XXXXXX.X"",
}";
                    File.WriteAllText(this.configFilePath, emptyConfig);
                }
                catch (ArgumentException)
                {
                }
            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Edit " + XboxLiveAppConfiguration.FileName, "Manually edit the configuration file if you already have your configuration values."), GUILayout.MaxWidth(200)))
            {
                Process.Start(this.configFilePath);
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (!File.Exists(this.configFilePath))
        {
            EditorGUILayout.HelpBox("If you already have a configuration file from elsewhere, or you know the values you need to fill out, you can create/edit the configuration file manually.", MessageType.Info, true);
        }
        else
        {
            EditorGUILayout.HelpBox("You can manually modify the existing configuration file if you need to update an individual value and you don't want to use the Association Wizard.", MessageType.Info, true);
        }

        GUILayout.Label("Developer Mode Configuration", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("In order to call Xbox Live services, your machine must be in Developer Mode and in the same sandbox that your title is configured in.  After you have have Enabled Xbox Live, you will be able to switch to Developer Mode.  Attempting to switch to Developer Mode may prompt you for administrative credentials.", MessageType.Info);

        string currentSandbox = "RETAIL";

        RegistryKey xboxLiveRegistryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\XboxLive");
        if (xboxLiveRegistryKey != null)
        {
            string registrySandbox = xboxLiveRegistryKey.GetValue("Sandbox") as string;
            if (registrySandbox != null)
            {
                currentSandbox = registrySandbox;
            }
        }

        bool developerModeEnabled = currentSandbox != "RETAIL";

        if (developerModeEnabled)
        {
            EditorGUILayout.HelpBox("Your machine is currently configured in Developer Mode.  You will need to switch back to retail in order to use Xbox Live functionality in any other apps or games.", MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal();
        PropertyLabel("Developer Mode", developerModeEnabled ? "Enabled (" + currentSandbox + ")" : "Disabled");

        if (this.configuration != null && currentSandbox != this.configuration.Sandbox)
        {
            if (GUILayout.Button("Switch to Developer Mode"))
            {
                SetSandbox(this.configuration.Sandbox);
            }
        }
        else
        {
            if (developerModeEnabled)
            {
                if (GUILayout.Button("Switch back to Retail Mode"))
                {
                    SetSandbox("RETAIL");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.EndScrollView();
    }

    private static void PropertyLabel(string name, string value)
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

    private static void SetSandbox(string sandboxId)
    {
        UnityEngine.Debug.Log("Setting sandbox to " + sandboxId);

        string command = string.Format("/c \"reg add HKLM\\Software\\Microsoft\\XboxLive /f /v Sandbox /d {0} && net stop XblAuthManager && net start XblAuthManager\"", sandboxId);
        ProcessStartInfo psi = new ProcessStartInfo("cmd", command)
        {
            UseShellExecute = true,
            Verb = "runas"
        };
        Process setSandboxProcess = Process.Start(psi);
        if (setSandboxProcess != null)
        {
            setSandboxProcess.WaitForExit();
        }
    }

    internal XboxLiveAppConfiguration TryLoad()
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