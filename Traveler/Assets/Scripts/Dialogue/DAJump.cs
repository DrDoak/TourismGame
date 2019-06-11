using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAJump : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "@");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		string raw = originTextbox.MasterSequence.RawText;

		string targetTag = ExtractArgs(actionString,"@")[0];
		//Debug.Log ("Looking for tag: " + targetTag);
		int startInt = 0;
		int depth = 0;
		List<string> tags = new List<string>();
		for (int i = 0; i < raw.Length; i++) {
			char c = raw.ToCharArray () [i];
			if (c == '<') {
				depth++;
				tags.Insert (0, "");
			} else if (c == '>') {
				depth--;
				Debug.Log ("Comparing: " + tags [0] + " to " + targetTag);
				if (tags[0] == targetTag) { 
					startInt = i;
					break;
				}
				tags.RemoveAt(0);
			} else {
				List<string> newTags = new List<string> ();
				for (int j = 0; j < tags.Count; j++)
					newTags.Insert (j, tags [j] + c);
				tags = newTags;
			}
		}
		if (startInt != 0) {
			originTextbox.MasterSequence.closeSequence ();
			TextboxManager.StartSequence (raw.Substring (startInt + 1));
		}
	}
}