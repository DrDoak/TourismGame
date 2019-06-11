using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSequence  {
	public int numChars = 0;
	public string rawText = "";
	public List<DialogueUnit> allDUnits;
	public DialogueSequence parentSequence = null;
	public GameObject Speaker;

	public void AdvanceSequence() {
		DialogueUnit currDialogueUnit = allDUnits [0];
		currDialogueUnit.Speaker = Speaker;
		currDialogueUnit.startSequence ();
		allDUnits.Remove (currDialogueUnit);
	}
	public void CloseSequence() {
		DialogueUnit first = null;
		if (allDUnits.Count > 0) {
			first = allDUnits [0];
		}
		allDUnits.Clear ();
		if (first != null)
			first.closeSequence ();
	}
}
