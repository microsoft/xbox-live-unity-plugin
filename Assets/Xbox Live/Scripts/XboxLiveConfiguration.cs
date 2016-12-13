using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
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
			return configurationFilePath ?? (configurationFilePath = Path.Combine(Application.dataPath, ConfigurationFileName));
		}
	}

    public string TitleId;

    public string PrimaryServiceConfigId;

	public string ProductFamilyName;

	public static XboxServicesConfiguration Load()
	{
		if (!File.Exists (ConfigurationFilePath)) {
			throw new InvalidOperationException ("You must associate your game with an Xbox Live Title in order to use Xbox Live functionality.");
		}

		try {
			return JsonUtility.FromJson<XboxServicesConfiguration> (File.ReadAllText (ConfigurationFilePath));
		} catch (ArgumentException) {
			return null;
		}
	}

	/// <summary>
	/// Delete the Xbox Services configuration file from disc to disassociate a game from a store product.  Also
	/// deletes the Unity .meta file so that it won't complain about a missing file.
	/// </summary>
	public static void Clear ()
	{
		AssetDatabase.DeleteAsset ("Assets/XboxServices.config");
		return;
	}
}
