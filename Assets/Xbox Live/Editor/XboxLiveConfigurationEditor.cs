// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Win32;
using Microsoft.Xbox.Services;

using UnityEditor;

using UnityEngine;

using Debug = UnityEngine.Debug;

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

        if (this.configuration != null)
        {
            if (string.IsNullOrEmpty(this.configuration.ServiceConfigurationId) || this.configuration.TitleId == 0 || string.IsNullOrEmpty(this.configuration.AppId))
            {
                EditorGUILayout.HelpBox("Your Xbox Live configuration appears invalid.  You will need to re-associate your game before you can create a finished build.", MessageType.Warning, true);
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
            EditorGUILayout.HelpBox("This plugin only supports the Xbox Live Creators Program. You can learn more at https://aka.ms/xblcp \n\nFor developers in the ID@Xbox program, instead follow the docs at http://aka.ms/xbldocs", MessageType.Warning);
            EditorGUILayout.HelpBox("In order to use Xbox Live functionality within your game, you can use the Xbox Live Assocation Wizard to link your game to a new or existing Xbox Live title.\n\nThis title information identifies your game with Xbox Live services and allows users to interact with Xbox Live.", MessageType.Info, true);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.Space();
        if (GUILayout.Button(new GUIContent("Run Xbox Live Association Wizard", "Run the Xbox Live Assocation Wizard to generate the configuration files needed to communicate with Xbox Live."), GUILayout.MaxWidth(250)))
        {
            string wizardPath = Path.Combine(Application.dataPath, "Xbox Live/Tools/AssociationWizard/AssociationWizard.exe");
            // We need to make sure to quote the path that we pass to the association wizard.
            Process.Start(wizardPath, '"' + this.configFileDirectory + '"');
        }

        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (!File.Exists(this.configFilePath))
        {
            EditorGUILayout.HelpBox("If you already have a configuration file from elsewhere, or you know the values you need to fill out, you can create/edit the configuration file manually.", MessageType.Info, true);
        }
        else
        {
            EditorGUILayout.HelpBox("You can manually modify the existing configuration file if you need to update an individual value and you don't want to use the Association Wizard.", MessageType.Info, true);
        }

        // Always show a button to manually open the configuration
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Open Configuration", "Open the configuration file to manually edit it if you already have your configuration values."), GUILayout.MaxWidth(150)))
        {
            // Check if a configuration file exists.
            if (!File.Exists(this.configFilePath))
            {
                // Create an empty one so that nothing breaks.
                const string emptyConfig = @"{
  ""PublisherId"": ""CN=00000000-0000-0000-0000-000000000000"",
  ""PublisherDisplayName"": """",
  ""PackageIdentityName"": """",
  ""DisplayName"": """",
  ""AppId"": """",
  ""ProductFamilyName"": """",
  ""PrimaryServiceConfigId"": ""00000000-0000-0000-0000-000000000000"",
  ""TitleId"": 0,
  ""Sandbox"": """",
}";
                File.WriteAllText(this.configFilePath, emptyConfig);
            }

            Process.Start(this.configFilePath);
        }

        if (File.Exists(this.configFilePath))
        {
            if (GUILayout.Button(new GUIContent("Delete Configuration", "Delete the configuration file containing the Xbox Live identity for your game."), GUILayout.MaxWidth(150)))
            {
                File.Delete(this.configFilePath);
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

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

        PropertyLabel("Developer Mode", developerModeEnabled ? "Enabled (" + currentSandbox + ")" : "Disabled");

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
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
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.EndScrollView();

        // Set a few additional settings based on values in the configuration file.
        // This is handled in the OnGUI thread because we need to be on the main thread to update these values.
        if (this.configuration != null)
        {
            // Update the certificate to the Association Wizard generated certificate if it exists.
            string certificatePath = string.Format(this.configuration.PackageIdentityName + "_StoreKey.pfx");
            if (File.Exists(certificatePath) && PlayerSettings.WSA.certificatePath != certificatePath)
            {
                string certificateDescriptor;
                if (PlayerSettings.WSA.certificatePath == "Assets/WSATestCertificate.pfx")
                {
                    certificateDescriptor = "Unity generated test publisher";
                }
                else
                {
                    certificateDescriptor = string.Format("exististing '{0}' publisher", PlayerSettings.WSA.certificateSubject);
                }

                Debug.LogFormat(
                    "Replacing {0} certificate with '{1}' ({2}) publisher certificate.\r\n" +
                    "To use a different certificate, delete or rename '{3}' and update Windows Store Publishing settings in 'Edit > Project Settings > Player' to the proper certificate.", 
                    certificateDescriptor, 
                    this.configuration.PublisherDisplayName,
                    this.configuration.PublisherId,
                    certificatePath);

                PlayerSettings.WSA.SetCertificate(certificatePath, string.Empty);
            }

            if (!PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.InternetClient))
            {
                Debug.Log("Enabling InternetClient capability which is required for Xbox Live.");
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
            }

            PlayerSettings.WSA.packageName = this.configuration.PackageIdentityName;
        }
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

    /// <summary>
    /// Update the Xbox Live sandbox the current machine is in.
    /// </summary>
    /// <param name="sandboxId">The sandbox to switch to.</param>
    /// <remarks>
    /// Modifying the sandbox requires making a change to the registry which requires administrative credentials.
    /// Calling this function may open up a UAC prompt.  Additiontally, the XblAuthManager service is restarted
    /// following the sandbox change to make it pick up the new value.
    /// </remarks>
    private static void SetSandbox(string sandboxId)
    {
        Debug.Log("Setting sandbox to " + sandboxId);
        RunCommand("reg add HKLM\\Software\\Microsoft\\XboxLive /f /v Sandbox /d " + sandboxId);
        RunCommand("net stop XblAuthManager && net start XblAuthManager");
    }

    private static void RunCommand(string command, bool asAdmin = true)
    {
        ProcessStartInfo psi = new ProcessStartInfo("cmd", String.Format("/c \"{0}\"", command))
        {
            UseShellExecute = true,
            Verb = asAdmin ? "runas" : null,
            CreateNoWindow = true,
        };
        Process commandProcess = Process.Start(psi);

        if (commandProcess == null)
        {
            throw new UnityException("Unable to start process to run command '{0}'." + command);
        }

        if (!commandProcess.WaitForExit(30000))
        {
            throw new UnityException("Command '{0}' has not completed after 30 seconds.  Unable to determine if it has succeeded.");
        }

        if (commandProcess.ExitCode != 0)
        {
            Debug.LogWarningFormat("Command '{0}' failed.  Exit code: {1}", command, commandProcess.ExitCode);
        }
    }

    /// <summary>
    /// Attempts to loads the current Xbox Live configuration if present.
    /// </summary>
    /// <returns>The configuration declared in the configuration file, or null if it does not exist or is invalid.</returns>
    internal XboxLiveAppConfiguration TryLoad()
    {
        if (!File.Exists(this.configFilePath))
        {
            return null;
        }

        try
        {
            return XboxLiveAppConfiguration.Load(this.configFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(string.Format("Xbox Live config file exists but failed to load: {0}", ex.Message));
            return null;
        }
    }
}