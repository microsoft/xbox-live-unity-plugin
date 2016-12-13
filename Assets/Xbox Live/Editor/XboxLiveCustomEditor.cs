// -----------------------------------------------------------------------
//  <copyright file="XboxLiveCustomEditor.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.IO;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(XboxLive))]
public class XboxLiveCustomEditor : Editor
{
    private SerializedProperty configuration;

    private void OnEnable()
    {
        this.configuration = this.serializedObject.FindProperty("Configuration");

        LoadConfiguration();

        FileSystemWatcher configFileWatcher = new FileSystemWatcher(Application.dataPath)
        {
            Filter = XboxServicesConfiguration.ConfigurationFileName
        };
        configFileWatcher.Created += ConfigFileChanged;
        configFileWatcher.Deleted += ConfigFileChanged;
        configFileWatcher.Renamed += ConfigFileChanged;
        configFileWatcher.Changed += ConfigFileChanged;
        configFileWatcher.EnableRaisingEvents = true;
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        string associateButtonText = XboxLive.Instance.IsConfigured ? "Update Xbox Live Association" : "Enable Xbox Live";

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(associateButtonText, GUILayout.MaxWidth(200)))
        {
            string wizardPath = Path.Combine(Application.dataPath, "Xbox Live/Tools/AssociationWizard/AssociationWizard.exe");
            Process.Start(wizardPath, Application.dataPath);
        }

        if (XboxLive.Instance.IsConfigured)
        {
            if (GUILayout.Button("Reset Xbox Live Association", GUILayout.MaxWidth(200)))
            {
                XboxServicesConfiguration.Clear();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (XboxLive.Instance.IsConfigured)
        {
            EditorGUILayout.PropertyField(this.configuration, true);
        }
        else
        {
            EditorGUILayout.HelpBox("In order to use Xbox Live functionality within your game, you must first associate it with a new or existing Xbox Live title.\nThis title information identifies your game with Xbox Live services and allows users to interact with Xbox Live and also allows you to interact with", MessageType.Info, true);
        }

        this.serializedObject.ApplyModifiedProperties();
    }

    private static void LoadConfiguration()
    {
        XboxLive.Instance.Configuration = File.Exists(XboxServicesConfiguration.ConfigurationFilePath) ? XboxServicesConfiguration.Load() : null;
    }

    private static void ConfigFileChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
        LoadConfiguration();
    }
}