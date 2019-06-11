using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAQuestion : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		Debug.Log ("Recognized?");
		return MatchStart (actionString, "?");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		List<string> rawOptions = new List<string> ();
		bool inQuestion = true;
		string prompt = "";
		string lastLine = "";
		for (int i = 1; i < actionString.Length; i++) {
			char c = actionString.ToCharArray () [i];
			if (c == '-') {
				if (inQuestion) {
					prompt = lastLine;
					inQuestion = false;
				} else {
					Debug.Log ("Added option: " + lastLine);
					rawOptions.Add (lastLine.Substring(1));
				}
				lastLine = "";
			}
			lastLine += c;
		}
		Debug.Log ("Added option end: " + lastLine.Substring (1));
		rawOptions.Add (lastLine.Substring(1));
		GameObject go = GameObject.Instantiate (GameObject.FindObjectOfType<TextboxManager> ().DialogueBoxPrefab);
		go.GetComponent<DialogueOptionBox> ().Prompt = prompt;
		go.GetComponent<DialogueOptionBox> ().MasterSequence = originTextbox.MasterSequence;
		Debug.Log ("length of rawoptions; " + rawOptions.Count);
		foreach (string s in rawOptions) {
			DialogueOption dop = new DialogueOption ();
			dop.SelectionText = s;
			dop.OnSelect = SelectionFunction;
			dop.remainderText = s + originTextbox.MasterSequence.RawText;
			go.GetComponent<DialogueOptionBox> ().AddDialogueOption (dop);
		}
		originTextbox.MasterSequence.closeSequence ();
	}

	private void SelectionFunction(DialogueOption dop) {
		Debug.Log ("Starting seqeunce: " + dop.remainderText);
		TextboxManager.StartSequence (dop.remainderText);
		GameObject.Destroy (dop.MasterBox.gameObject);
	}
}