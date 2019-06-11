using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAEnd : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "END");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		Debug.Log ("End Called");
		originTextbox.MasterSequence.closeSequence ();
	}
}
