using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRAbandon : Transition {

	public float abandonDistance = 10f;

	public override void OnUpdate() {
		if (OriginTask.Target == null || Vector3.Distance (transform.position, OriginTask.Target.transform.position) > abandonDistance) {
			//Debug.Log ("Abandoned to Neutral");
			TriggerTransition ();
		}
	}
}
