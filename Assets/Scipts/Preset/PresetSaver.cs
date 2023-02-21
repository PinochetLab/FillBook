using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class PresetSaver
{
	private static string jsonPath = "/Data/presets.JSON";

	private static string FullPath {
		get => Application.streamingAssetsPath + jsonPath;
	}

	public static PresetStorage Load() {
		if ( File.Exists(FullPath) && !string.IsNullOrWhiteSpace(File.ReadAllText(FullPath)) ) {
			return JsonConvert.DeserializeObject<PresetStorage>(File.ReadAllText(FullPath));
		}
		else {
			return new PresetStorage();
		}
	}

	public static void Save(PresetStorage preset) {
		string json = JsonConvert.SerializeObject(preset, Formatting.Indented, new JsonSerializerSettings { });
		File.WriteAllText(FullPath, json);
	}
}
