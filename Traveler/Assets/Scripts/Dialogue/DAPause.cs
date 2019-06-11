using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAPause : DialogueAction {
	
	public override bool IsExecutionString(string actionString) {
		float res;
		return actionString.ToCharArray()[0] != '$' && float.TryParse (actionString, out res);
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		originTextbox.SetPause (float.Parse (actionString));		
	}
}