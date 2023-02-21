using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiler : MonoBehaviour
{
	private static Tilemap tilemap;
	private static TileBase fieldTile;

	private static LineRenderer lineRenderer;
	private static Transform lineParent;
	private static LineRenderer currentLine;

	private static List<Vector2Int> filledCells = new List<Vector2Int>();

	[SerializeField] private TileBase fieldTileBase;

	private static int n = 0;

	private static bool isRightStarted;

	private static Preset preset;
	private static List<Vector2Int> cells = new List<Vector2Int>();
	private static List<Vector2Int> enteredWord = new List<Vector2Int>();

	private static readonly List<Color> colors = new List<Color>() {
		Color.green,
		Color.blue,
		Color.yellow,
		Color.red,
		Color.magenta,
		Color.cyan
	};

	private static Color currentColor = Color.white;

	private static Vector2Int overCell = Vector2Int.zero;
	private static Vector2Int lastCell = Vector2Int.zero;

	private void Awake() {
		tilemap = GetComponent<Tilemap>();
		lineRenderer = GetComponentInChildren<LineRenderer>();
		currentLine = lineRenderer;
		lineParent = lineRenderer.transform.parent;
		lineRenderer.positionCount = 0;
		fieldTile = fieldTileBase;
	}

	private static Vector3 CellCenter(Vector2Int v) {
		return (Vector3)(Vector3Int)v + (Vector3)Vector2.one / 2;
	}

	private Vector2Int CellUnderMouse() {
		return (Vector2Int)tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	private static void SetTile(Vector2Int v, TileBase t) {
		tilemap.SetTile((Vector3Int)v, t);
	}

	private static TileBase GetTile(Vector2Int v) {
		return tilemap.GetTile((Vector3Int)v);
	}

	private static bool IsFilled(Vector2Int v) {
		return GetTile(v);
	}

	private static void ClearLines() {
		int n = lineParent.childCount;
		for (int i = 0; i < n - 1; i++ ) {
			Destroy(lineParent.GetChild(n - 1 - i).gameObject);
		}
		currentLine = lineRenderer;
	}

	private static LineRenderer AddLine() {
		return Instantiate(lineRenderer.gameObject, lineParent).GetComponent<LineRenderer>();
	}

	private static void Fill(TileBase t) {
		foreach (Vector2Int cell in cells ) {
			SetTile(cell, t);
			if ( !t ) {
				SetColor(cell, Color.white);
			}
		}
	}

	private static void SetColor(Vector2Int v, Color color) {
		tilemap.SetTileFlags((Vector3Int)v, TileFlags.None);
		tilemap.SetColor((Vector3Int)v, color);
	}

	public static void BuildLevel(int n, Preset preset, int sentenceLength) {
		ClearLines();
		Fill(null);
		filledCells.Clear();
		Tiler.preset = preset;
		cells = preset.GetPath();
		cells.RemoveRange(sentenceLength, cells.Count - sentenceLength);
		Tiler.n = n;
		Fill(fieldTile);
		Vector2 v = tilemap.cellSize * n / 2;
		Camera.main.transform.position = new Vector3(v.x, v.y + n * 0.075f, Camera.main.transform.position.z);
		Camera.main.orthographicSize = n * (1.3f / 2);
	}

	private static void UpdateColors() {

		lineRenderer.positionCount = enteredWord.Count;
		currentLine.startColor = currentColor;
		currentLine.endColor = currentColor;

		bool dark = 0.299 * currentColor.r + 0.587 * currentColor.g + 0.114 * currentColor.b < 0.5f;

		for ( int i = 0; i < enteredWord.Count; i++ ) {
			SetColor(enteredWord[i], currentColor);
			currentLine.SetPosition(i, CellCenter(enteredWord[i]));
			if ( dark ) LetterMaster.SetColor(enteredWord[i].x, enteredWord[i].y, Color.white);
		}
	}

	private void ClearCell(Vector2Int v) {
		SetColor(v, Color.white);
		LetterMaster.SetColor(v.x, v.y, Color.black);
	}

	public static void FillWord(List<Vector2Int> vs) {
		enteredWord = new List<Vector2Int>(vs);
		currentColor = colors[Random.Range(0, colors.Count)];
		UpdateColors();
		AddLine();
		filledCells.AddRange(enteredWord);
		enteredWord.Clear();
		UpdateColors();
		currentColor = Color.white;
	}

	private void Update() {
		Vector2Int mouseCell = CellUnderMouse();

		if ( Input.GetMouseButtonDown(0) ) {
			if ( !filledCells.Contains(mouseCell) ) {
				enteredWord.Clear();
				isRightStarted = IsFilled(mouseCell);
				currentColor = colors[Random.Range(0, colors.Count)];
			}
		}
		else if ( Input.GetMouseButton(0) ) {
			if ( IsFilled(mouseCell) && !enteredWord.Contains(mouseCell) && !filledCells.Contains(mouseCell) ) {
				if (enteredWord.Count == 0 || Vector2Int.Distance(enteredWord[enteredWord.Count - 1], mouseCell) == 1 ) {
					enteredWord.Add(mouseCell);
					SetColor(mouseCell, Color.green);
					lastCell = mouseCell;
					UpdateColors();
				}
			}else if ( IsFilled(mouseCell) && mouseCell != lastCell && !filledCells.Contains(mouseCell) ) {
				int index = enteredWord.IndexOf(mouseCell);
				for (int i = index + 1; i < enteredWord.Count; i++ ) {
					ClearCell(enteredWord[i]);
				}
				enteredWord.RemoveRange(index + 1, enteredWord.Count - index - 1);
				UpdateColors();
			}
		}
		else if ( Input.GetMouseButtonUp(0) ) {
			if ( !LevelBuilder.TryFinish(enteredWord, out bool finished) ) {
				foreach ( Vector2Int cell in enteredWord ) {
					ClearCell(cell);
				}
				enteredWord.Clear();
				UpdateColors();
			}
			else {
				if ( !finished ) {
					AddLine();
					filledCells.AddRange(enteredWord);
				}
				enteredWord.Clear();
				UpdateColors();
			}
		}
		else {
			if ( mouseCell != overCell ) {
				if ( !filledCells.Contains(overCell) ) SetColor(overCell, Color.white);
				overCell = mouseCell;
				if ( IsFilled(mouseCell) && !filledCells.Contains(mouseCell) ) {
					SetColor(mouseCell, Color.gray);
				}
			}
		}
	}
}
