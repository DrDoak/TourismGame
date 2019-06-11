using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DATextSpeed : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "$");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		originTextbox.TimeBetweenType = float.Parse (ExtractArgs(actionString,"$")[0]);
	}
}