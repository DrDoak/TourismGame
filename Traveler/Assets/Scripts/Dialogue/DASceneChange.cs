using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DASceneChange : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "/s");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		Initiate.Fade (ExtractArgs(actionString,"/s")[0], Color.white, 2.0f);
	}
}