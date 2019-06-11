using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DACameraFocus : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "CAM");
	}

	public override void PerformAction(string actionString, Textbox originTextbox) {
		string targetChar = ExtractArgs (actionString, "CAM")[0];
		GameObject target = GameObject.Find (targetChar);
		//Debug.Log ("Focusing on: " + targetChar);
		if (target != null) {
			GameObject.FindObjectOfType<CameraController> ().Target = target;
		}
	}

}

