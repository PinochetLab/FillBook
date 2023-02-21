using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class SentenceProgress
{
	public Dictionary<string, int> indices = new Dictionary<string, int>();
	public Dictionary<string, Sentence> sentences = new Dictionary<string, Sentence>();

	public SentenceProgress() { }

	public int GetIndex(Book book) {
		return indices[book.name];
	}

	public bool HasAnyProgress(Book book) {
		return indices.ContainsKey(book.name);
	}

	public bool HasLevelProgress(Book book) {
		return sentences.ContainsKey(book.name);
	}

	public void ClearProgress(Book book) {
		indices.Remove(book.name);
		sentences.Remove(book.name);
	}

	public void ClearLevelProgress(Book book) {
		ChangeSentence(book, null);
	}

	public Sentence GetSentence(Book book) {
		return sentences[book.name];
	}

	public void ChangeIndex(Book book, int index) {
		indices[book.name] = index;
	}

	public void ChangeSentence(Book book, Sentence sentence) {
		sentences[book.name] = sentence;
	}

	public void ClearSentense(Book book) {
		sentences.Remove(book.name);
	}
}
