using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelBuilder : MonoBehaviour
{
	[SerializeField] private Book bookField;

	private static Book book;

	private static List<List<Vector2Int>> paths = new List<List<Vector2Int>>();

	private static Sentence sentence;
	private static int startIndex;

	private void Awake() {
		book = bookField;
		SentenceSaver.LoadProgress();
	}

	private void Start() {
		OpenLevel();
	}

	private static void OpenLevel() {
		startIndex = 0;
		if ( SentenceSaver.HasAnyProgress(book) ) {
			startIndex = SentenceSaver.GetIndex(book);
		}
		Sentence sentence;
		if ( SentenceSaver.HasLevelProgress(book) ) {
			sentence = SentenceSaver.GetSentence(book);
		}
		else {
			sentence = TextMaster.ReadSentence(book.fileName, ref startIndex);
		}
		LevelBuilder.sentence = sentence;
		Debug.Log(sentence.guessed.Count(x => x));
		Save();
		BuildLevel(sentence);
	}

	private static void Save() {
		SentenceSaver.Save(book, startIndex, sentence);
	}

	private static void FinishLevel() {

	}

	private static void NextLevel() {
		
	}

	public static bool TryFinish(List<Vector2Int> path, out bool finished) {
		finished = false;
		int index = 0;
		foreach (var p in paths ) {
			bool eq = p.Count == path.Count;
			if ( eq ) {
				for (int i = 0; i < p.Count; i++ ) {
					Debug.Log(p[i]);
					if (p[i] != path[i] ) {
						Debug.Log(p[i]);
						eq = false;
						break;
					}
				}
				if ( eq ) {

					sentence.Guess(index);
					Save();
					SentenceTextController.UpdateText();
					
					if ( sentence.IsGuessed() ) {
						finished = true;
						Save();
						SentenceSaver.ClearSentense(book);
						OpenLevel();
					}
					return true;
				}
			}
			index++;
		}
		return false;
	}

	private static void BuildLevel(Sentence sentence) {
		var words = sentence.words;

		int count = words.Sum(x => x.Length);
		int n = Mathf.CeilToInt(Mathf.Sqrt(count));

		Preset preset = PresetStorage.current.GetRandom(n);

		paths.Clear();

		List<Vector2Int> fullPath = preset.GetPath();

		int index = 0;

		foreach ( string word in words ) {
			List<Vector2Int> path = new List<Vector2Int>();
			for ( int i = 0; i < word.Length; i++ ) {
				path.Add(fullPath[index]);
				index++;
			}
			Debug.Log(path.Count);
			paths.Add(path);
		}

		string sentenceString = "";
		foreach ( string word in words ) sentenceString += word;

		Tiler.BuildLevel(n, preset, sentenceString.Length);
		LetterMaster.BuildLetters(n, preset, sentenceString);
		SentenceTextController.SetSentence(sentence);

		foreach ( var p in paths ) {
			Debug.Log(p.Count);
		}

		for ( int i = 0; i < sentence.words.Count; i++ ) {
			if ( sentence.guessed[i] ) {
				Tiler.FillWord(paths[i]);
			}
		}
	}
}