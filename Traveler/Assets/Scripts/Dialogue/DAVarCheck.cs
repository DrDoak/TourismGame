using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVarCheck : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "IF");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		List<string> args = ExtractArgs(actionString,"IF");

		if (args.Count != 3) {
			Debug.Log ("INVALID SET VARIABLE COMMAND, Need 3 Args got: " + args.Count);
			return;
		}
		string s = SaveObjManager.PublicVars ().PersistentStrings [args [0]];
		if (s == null || s != args[1])
			return;
		originTextbox.PerformSpecialAction (args [2]);
	}
}
