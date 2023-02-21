using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Preset
{
	public Vector2Int start;
	public string path;

	public Preset(Vector2Int start, string path) {
		this.start = start;
		this.path = path;
	}

	public static char ToChar(Vector2Int v) {
		if ( v == Vector2Int.left ) return 'l';
		else if ( v == Vector2Int.right ) return 'r';
		else if ( v == Vector2Int.up ) return 'u';
		else return 'd';
	}

	public static Vector2Int ToVector(char c) {
		switch ( c ) {
			case 'r':
				return Vector2Int.right;
			case 'l':
				return Vector2Int.left;
			case 'u':
				return Vector2Int.up;
			case 'd':
				return Vector2Int.down;
			default:
				return Vector2Int.zero;
		}
	}

	public List<Vector2Int> GetPath() {
		List<Vector2Int> res = new List<Vector2Int>() { start };
		Vector2Int current = start;
		foreach (char c in path ) {
			current += ToVector(c);
			res.Add(current);
		}
		return res;
	}
}
