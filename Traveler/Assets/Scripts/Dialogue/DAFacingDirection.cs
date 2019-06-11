using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAFacingDirection : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return (MatchStart (actionString, "]") || MatchStart(actionString, "["));
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		List<string> chars = ExtractArgs(actionString.Substring(1,actionString.Length - 1),"");
		if (chars.Count < 2)
			return;
		GameObject character = GameObject.Find (chars[0]);
		GameObject target = GameObject.Find (chars[1]);
		if (character != null && character.GetComponent<Orientation>()) {
			if (actionString.ToCharArray () [0] == ']') {
				character.GetComponent<Orientation> ().SetDirection (target.transform.position.x < character.transform.position.x);
			} else {
				character.GetComponent<Orientation> ().SetDirection (target.transform.position.x > character.transform.position.x);
			}
		}
	}
}
