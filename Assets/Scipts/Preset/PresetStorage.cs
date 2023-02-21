using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PresetStorage
{
	public Dictionary<int, List<Preset>> presets = new Dictionary<int, List<Preset>>();

	public static PresetStorage current;

	public bool TryAdd(int n, Preset preset) {
		if ( !presets.ContainsKey(n) ) presets[n] = new List<Preset>();
		if ( presets[n].Contains(preset) ) return false;
		presets[n].Add(preset);
		return true;
	}

	public Preset GetRandom(int n) {
		return presets[n][Random.Range(0, presets[n].Count)];
	}
}
