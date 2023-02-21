using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterMaster : MonoBehaviour
{
	private static TMP_Text text;
	private static RectTransform rectTransform;

	private static int size = 0;

	private static float cellSize = 108;

	private const string pattern = "<color=#{0}>{1}</color>";

	private static List<char> letters = new List<char>();

	private static Color textColor = Color.black;

	private static int PatternLength {
		get => pattern.Length + 5 - 2;
	}

	private static int StringLength {
		get => PatternLength * size + 1;
	}

	private static int LetterStart {
		get => pattern.IndexOf('1') - 1 + 5;
	}

	private static string CreateBlock(char letter, Color color ) {
		return string.Format(pattern, ColorUtility.ToHtmlStringRGBA(color), letter);
	}

	private void Awake() {
		text = GetComponent<TMP_Text>();
		rectTransform = text.rectTransform;
	}

	public static void BuildLetters(int size, Preset preset, string sentence) {
		LetterMaster.size = size;

		rectTransform.sizeDelta = Vector2.one * cellSize * size;

		letters.Clear();
		for (int i = 0; i < size; i++ ) {
			for (int j = 0; j < size; j++ ) {
				Color color = Color.clear;
				letters.AddRange(CreateBlock('_', color).ToCharArray());
			}
			if ( i < size - 1 ) letters.Add('\n');
		}

		List<Vector2Int> path = preset.GetPath();
		for ( int i = 0; i < sentence.Length; i++ ) {
			SetLetter(path[i].x, path[i].y, sentence[i]);
			SetColor(path[i].x, path[i].y, Color.black);
		}
	}

	private static int ToIndex(int i, int j) {
		return (size - 1 - j) * StringLength + i * PatternLength + LetterStart;
	}

	private static void UpdateText() {
		text.SetCharArray(letters.ToArray());
	}

	private static void SetLetter(int i, int j, char c) {
		letters[ToIndex(i, j)] = char.ToUpper(c);
		UpdateText();
	}

	public static void SetColor(int i, int j, Color color) {
		int index = ToIndex(i, j) - 9;
		string htmlColor = ColorUtility.ToHtmlStringRGBA(color);
		for (int k = 0; k < htmlColor.Length; k++ ) {
			letters[index + k] = htmlColor[k];
		}
		UpdateText();
	}
}
