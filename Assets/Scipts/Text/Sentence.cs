using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Sentence
{
	public string startFill = "";
	public List<string> words = new List<string>();
	public List<string> fills = new List<string>();
	public List<bool> guessed = new List<bool>();

	private static readonly char filler = '_';

	private string GetString(List<bool> guessed) {
		string result = startFill;
		for (int i = 0; i < words.Count; i++ ) {
			if ( i < guessed.Count && !guessed[i] ) {
				result += new string(filler, words[i].Length);
			}
			else {
				result += words[i];
			}
			if ( i < fills.Count ) {
				result += fills[i];
			}
		}
		return result;
	}

	public void Guess(int index) {
		guessed[index] = true;
	}

	public string GetStringInProgress() {
		return GetString(guessed);
	}

	public string GetSentenceString() {
		return GetString(new List<bool>());
	}

	public bool IsGuessed() {
		return guessed.Count(x => !x) == 0;
	}

	public List<string> GetLastWords() {
		List<string> result = new List<string>();
		for ( int i = 0; i < words.Count; i++ ) {
			if ( guessed[i] ) result.Add(words[i]);
		}
		return result;
	}

	public string GetWordString() {
		string result = "";
		words.ForEach(w => result += w.ToLower());
		return result;
	}

	public Sentence(string startFill, List<string> words, List<string> fills) {
		this.startFill = startFill;
		this.words = words;
		this.fills = fills;
		words.ForEach(x => guessed.Add(false));
	}
}
