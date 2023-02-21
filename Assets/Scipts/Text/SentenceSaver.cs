using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public static class SentenceSaver
{
	private static readonly string id = "sentenceprogress";

	private static SentenceProgress progress;

	public static void LoadProgress() {
		PlayerPrefs.DeleteAll();                //////  To Clear Progress \\\\\\
		if ( !PlayerPrefs.HasKey(id) ) {
			progress = new SentenceProgress();
			Save();
		}
		else {
			progress = JsonConvert.DeserializeObject<SentenceProgress>(PlayerPrefs.GetString(id));
		}
	}

    public static bool HasAnyProgress(Book book) {
		return progress.HasAnyProgress(book);
	}

	public static bool HasLevelProgress(Book book) {
		return progress.HasLevelProgress(book);
	}

	public static int GetIndex(Book book) {
		return progress.GetIndex(book);
	}

	public static Sentence GetSentence(Book book) {
		return progress.GetSentence(book);
	}

	public static void MoveIndex(Book book, int index) {
		progress.ChangeIndex(book, index);
	}

	public static void Save(Book book, int index, Sentence sentence) {
		progress.ChangeIndex(book, index);
		progress.ChangeSentence(book, sentence);
		Save();
	}

	public static void ClearSentense(Book book) {
		progress.ClearSentense(book);
	}

	private static void Save() {
		string json = JsonConvert.SerializeObject(progress);
		Debug.Log(json);
		PlayerPrefs.SetString(id, json);
	}
}
