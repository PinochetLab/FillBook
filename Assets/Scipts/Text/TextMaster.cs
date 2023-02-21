using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TextMaster
{

	private static readonly int minLength = 6;
	private static readonly int maxLength = 64;

	private static readonly int bufferSize = 10;

	private static readonly char[] separators = new char[] { '.', '?', '!'};

	private static readonly string path = "Assets/Resources/TextAssets/";
	
	public static Sentence ReadSentence(string name, ref int startIndex) {
		using ( FileStream fs = new FileStream(path + name, FileMode.Open) ) {
			fs.Seek(startIndex, SeekOrigin.Begin);

			string startFill = "";

			List<string> words = new List<string>();
			List<string> fills = new List<string>();

			string word = "";
			string fill = "";
			bool wasWord = true;
			bool needToEnd = false;
			bool hasWord = false;

			while ( !needToEnd ) {
				byte[] bytes = new byte[bufferSize];
				fs.Read(bytes, 0, bufferSize);
				var str = System.Text.Encoding.Default.GetString(bytes);
				foreach ( char c in str ) {
					startIndex++;
					if ( needToEnd && c == ' ' ) break; 
					if ( !Char.IsLetter(c) || separators.Contains(c) ) {
						if ( separators.Contains(c) ) needToEnd = true;
						else {
							if ( !hasWord ) {
								startFill += c;
								continue;
							}
						}
						fill += c;
						if ( wasWord ) {
							words.Add(word);
							word = "";
						}
						wasWord = false;
					}
					else {
						hasWord = true;
						word += c;
						if ( !wasWord ) {
							fills.Add(fill);
							fill = "";
						}
						wasWord = true;
					}
				}
			}

			if ( word != "" ) words.Add(word);
			if ( fill != "" ) fills.Add(fill);

			return new Sentence(startFill, words, fills);
		}
	}
}
