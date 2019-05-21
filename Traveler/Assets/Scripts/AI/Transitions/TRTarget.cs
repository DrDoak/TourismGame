using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRTarget : Transition {

	// Use this for initialization
	void Start () {
		
	}
	
	public override void OnSight(Observable o) {
		if (o.GetComponent<Attackable> () &&
		    MasterAI.GetComponent<Attackable> ().CanAttack (
			    o.GetComponent<Attackable> ().Faction)) {
			TargetTask.Target = o.gameObject;
			//Debug.Log ("Triggering Transition");
			TriggerTransition ();
		}
	}
}
