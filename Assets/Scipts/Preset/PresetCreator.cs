using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(PresetCreator))]
public class PresetCreator_CE : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if ( GUILayout.Button("Generate") ) {
			(target as PresetCreator).Generate();
		}

		if ( GUILayout.Button("Generate++") ) {
			(target as PresetCreator).CleverGenerate();
		}
	}
}
#endif

public class PresetCreator : MonoBehaviour {
	private static int n = 1;

	[Header("Generation")]

	[SerializeField] private int count = 100;
	[SerializeField] private int size = 3;

	private void Awake() {
		PresetStorage.current = PresetSaver.Load();
	}

	public void CleverGenerate() {
		n = size;
		PresetStorage ps = PresetSaver.Load();

		for (int index = 0; index < count; index++ ) {
			Vector2Int start = new Vector2Int(UnityEngine.Random.Range(0, n), UnityEngine.Random.Range(0, n));
			var paths = new Queue<List<Vector2Int>>();
			paths.Enqueue(new List<Vector2Int>() { start });
			while ( paths.Count > 0 ) {
				var path = paths.Dequeue();
				Vector2Int cell = path[path.Count - 1];

				bool valid(Vector2Int v) {
					return v.x >= 0 && v.x < n && v.y >= 0 && v.y < n && !path.Contains(v);
				}

				Func<Vector2Int, Vector2Int> left = v => v + Vector2Int.left;
				Func<Vector2Int, Vector2Int> right = v => v + Vector2Int.right;
				Func<Vector2Int, Vector2Int> up = v => v + Vector2Int.up;
				Func<Vector2Int, Vector2Int> down = v => v + Vector2Int.down;

				Vector2Int dir(Vector2Int v) {
					return v - cell;
				}

				Vector2Int perp1(Vector2Int v) {
					return new Vector2Int(v.y, -v.x);
				}

				Vector2Int perp2(Vector2Int v) {
					return -perp1(v);
				}

				List<Vector2Int> neighbours(Vector2Int v) {
					return new List<Vector2Int>() {left(v), right(v), up(v), down(v)};
				}

				int ncount(Vector2Int v) {
					return neighbours(v).Count(valid);
				}

				var variants = neighbours(cell).Where(valid).ToList();

				bool endFound = false;

				foreach (var variant in variants ) {
					if ( ncount(variant) <= 1 ) {
						path.Add(variant);
						paths.Enqueue(path);
						endFound = true;
						break;
					}
				}

				if ( endFound ) continue;

				var dallowed = new List<Vector2Int>();
				var allowed = new List<Vector2Int>();

				foreach ( var point in variants ) {
					if ( !valid(point + dir(point)) ) {
						dallowed.Add(point);
						if ( valid(perp1(point)) ) allowed.Add(perp1(point));
						if ( valid(perp2(point)) ) allowed.Add(perp2(point));
					}
				}

				if ( dallowed.Count > 0 && dallowed.Count < variants.Count ) {
					foreach ( var v in allowed ) {
						if ( !dallowed.Contains(v) ) {
							var p = new List<Vector2Int>(path) { v };
							paths.Enqueue(p);
						}
					}
					continue;
				}

				var ns = variants.Where(valid).ToList();

				if ( ns.Count == 0 && path.Count == n * n ) {
					string s = "";
					for (int i = 1; i < path.Count; i++ ) {
						s += Preset.ToChar(path[i] - path[i - 1]);
					}
					var preset = new Preset(path[0], s);
					ps.TryAdd(n, preset);
				}
				else if (ns.Count > 0) {
					path.Add(ns[UnityEngine.Random.Range(0, ns.Count)]);
					paths.Enqueue(path);
				}
			}
		}
		PresetSaver.Save(ps);
	}

	public void Generate() {
		n = size;

		List<List<bool>> used = new List<List<bool>>();
		for ( int i = 0; i < n; i++ ) {
			var str = new List<bool>();
			for ( int j = 0; j < n; j++ ) {
				str.Add(false);
			}
			used.Add(str);
		}

		PresetStorage ps = PresetSaver.Load();

		for ( int k = 0; k < count; k++ ) {
			Vector2Int c = new Vector2Int(UnityEngine.Random.Range(0, size), UnityEngine.Random.Range(0, size));
			Vector2Int start = c;
			string path = "";

			for ( int j = 0; j < n * n - 1; j++ ) {
				used[c.x][c.y] = true;
				string s = "";
				if ( c.x > 0 && !used[c.x - 1][c.y] ) s += 'l';
				if ( c.x < size - 1 && !used[c.x + 1][c.y] ) s += 'r';
				if ( c.y < size - 1 && !used[c.x][c.y + 1] ) s += 'u';
				if ( c.y > 0 && !used[c.x][c.y - 1] ) s += 'd';

				if ( s.Length == 0 ) {
					path = "";
					break;
				}

				char d = s[UnityEngine.Random.Range(0, s.Length)];
				path += d;
				c += Preset.ToVector(d);
			}

			if ( path != "" ) {
				ps.TryAdd(size, new Preset(start, path));
			}

			for ( int i = 0; i < n; i++ ) {
				for ( int j = 0; j < n; j++ ) {
					used[i][j] = false;
				}
			}
		}

		PresetSaver.Save(ps);
	}
}