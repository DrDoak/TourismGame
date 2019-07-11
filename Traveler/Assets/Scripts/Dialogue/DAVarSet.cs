using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVarSet : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "SET");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		List<string> args = ExtractArgs(actionString,"SET");
		if (args.Count != 2) {
			Debug.Log ("INVALID SET VARIABLE COMMAND, Need 2 Args got: " + args.Count);
			return;
		}
        SaveObjManager.PublicVars().ClearString(args[0]);
		SaveObjManager.PublicVars ().SetString (args [0], args [1]);
	}
}