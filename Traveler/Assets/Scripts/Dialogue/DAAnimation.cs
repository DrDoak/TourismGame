using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAAnimation : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "#");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		List<string> chars = ExtractArgs(actionString.Substring(1,actionString.Length - 1),"");
		if (chars.Count < 2)
			return;
		GameObject character = GameObject.Find (chars [0]);
		string anim = chars [1];
		if (character != null && character.GetComponent<AnimatorSprite>()) {
			Debug.Log ("Playing anim: " + anim + " for character: " + character.name);
			originTextbox.FreezeCharacter(character.GetComponent<MovementBase> ());

			if (character.GetComponent<CharacterBase> ())
				character.GetComponent<CharacterBase> ().SetPause (true);
			bool res = character.GetComponent<AnimatorSprite> ().Play (anim);
			Debug.Log ("res: " + res);
		}
	}
}