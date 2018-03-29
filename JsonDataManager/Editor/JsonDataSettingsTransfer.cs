﻿using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEditor.Build;
using UnityEditor;

public class JsonDataSettingsTransfer : IPostprocessBuild
{
	public int callbackOrder => 0;

#if UNITY_2018_1_OR_NEWER
	public void OnPostprocessBuild(BuildReport report)
	{
		OnBuild(report.summary.outputPath);
	}
#else
	public void OnPostprocessBuild(BuildTarget target, string path)
	{
		OnBuild(path);
	}
#endif

	private void OnBuild(string path)
	{
		var buildPath = BuildSettingsPath(path);
		var editorPath = AssetSettingsPath();

		var settings = SettingsFilenames(editorPath);
		Directory.CreateDirectory(buildPath);
		foreach (var setting in settings)
		{
			var filename = Path.GetFileName(setting);
			var newFilepath = Path.Combine(buildPath, filename);
			File.Copy(setting, newFilepath);
		}

		Debug.Log($"JsonDataSettingsTransfer moved {settings.Length} settings files to {buildPath}");
	}

	private string BuildSettingsPath(string exePath)
	{
		var basePath = Path.GetDirectoryName(exePath);
		var gameFolder = Path.GetFileNameWithoutExtension(exePath) + "_Data";
		var fullPath = Path.Combine(basePath, gameFolder);
		var settingsPath = Path.Combine(fullPath, JsonDataManager.BuildSettingsFolder);

		return settingsPath;
	}

	private string AssetSettingsPath() => Path.Combine(Application.dataPath, JsonDataManager.EditorSettingsFolder);

	private string[] SettingsFilenames(string path) => Directory.GetFiles(path).Where(p => p.EndsWith(".json")).ToArray();
}