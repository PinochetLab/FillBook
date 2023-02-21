using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SentenceTextController : MonoBehaviour
{
	private static TMP_Text text;

	private static Sentence sentence;

	private void Awake() {
		text = GetComponent<TMP_Text>();
	}

	public static void UpdateText() {
		text.text = sentence.GetStringInProgress();	
	}

	public static void SetSentence(Sentence sentence) {
		SentenceTextController.sentence = sentence;
		UpdateText();
	}
}
