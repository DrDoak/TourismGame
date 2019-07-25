using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRAbandon : Transition {

	public float AbandonDistance = 10f;

	public override void OnUpdate() {
		if (OriginTask.GetTargetObj() == null || Vector3.Distance (transform.position, OriginTask.GetTargetObj().transform.position) > AbandonDistance) {
			//Debug.Log ("Abandoned to Neutral");
			TriggerTransition ();
		}
	}

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("AbandonDistance", this))
            AbandonDistance = float.Parse(g.GetVariable("AbandonDistance", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("AbandonDistance", AbandonDistance.ToString(), this);
    }

}
