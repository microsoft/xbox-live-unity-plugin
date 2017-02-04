// -----------------------------------------------------------------------
//  <copyright file="XboxLiveConfiguration.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;

using UnityEngine;

[Serializable]
public class XboxServicesConfiguration
{
    public const string ConfigurationFileName = "XboxServices.config";

    private static string configurationFilePath;

    public static string ConfigurationFilePath
    {
        get
        {
            return configurationFilePath ?? (configurationFilePath = Path.Combine(Application.dataPath, "..\\" + ConfigurationFileName));
        }
    }

    public string TitleId;

    public string ServiceConfigurationId;

    public string Environment;

    public string Sandbox;

    public string ProductFamilyName;

    public bool UseMockData;

    public void Save()
    {
        this.Save(ConfigurationFilePath);
    }

    private void Save(string path)
    {
        try
        {
            File.WriteAllText(path, JsonUtility.ToJson(this, true));
        }
        catch (ArgumentException)
        {
        }
    }

    public static XboxServicesConfiguration Load()
    {
        return Load(ConfigurationFilePath);
    }

    public static XboxServicesConfiguration Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return JsonUtility.FromJson<XboxServicesConfiguration>(File.ReadAllText(path));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}