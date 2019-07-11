﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxTrigger : Interactable {
	
	public bool typeText = true;

	public bool ClearAllSequence = true;
	public bool Skippable = false;
	public DialogueSound soundType = DialogueSound.SPOKEN;

	[TextArea(3,8)]
	public string TextboxString;

	void Start() {
		base.init ();
	}

	void Update () { destroyAfterUse (); }

	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 1, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}

	protected override void onTrigger(GameObject interactor) {
        Debug.Log("On Trigger");
		if (ClearAllSequence)
			TextboxManager.ClearAllSequences ();
		TextboxManager.SetSoundType (soundType);
		if (ClearAllSequence)
			TextboxManager.ClearAllSequences ();
		TextboxManager.StartSequence (TextboxString,null,Skippable);
	}

	private void storeData(CharData d) {
		d.SetBool("TriggerUsed", TriggerUsed);
		d.SetString( "TextboxString", TextboxString);
	}

	private void loadData(CharData d) {
		TriggerUsed = d.GetBool("TriggerUsed");
		TextboxString = d.GetString("TextboxString");
	}
}