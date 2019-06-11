using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAKeyname : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "&");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		originTextbox.CurrentText += TextboxManager.GetKeyString (ExtractArgs(actionString,"&")[0]);
	}
}