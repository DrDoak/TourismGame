using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKAggressiveAttack : Task {

	public Vector2 TargetPositionOffset = new Vector2 ();
	public float TargetPositionTolerance = 0f;
	public float TargetToleranaceVariance = 0f;

	public bool OnlyUseSpecificAttacks = false;
	public List<string> AcceptableAttacks;

	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	public override void OnTransition () {
		if (Target != null) {
			MasterAI.GetComponent<OffenseAI> ().setTarget (Target.GetComponent<MovementBase> (),
				TargetPositionOffset,TargetPositionTolerance + Random.Range(-TargetToleranaceVariance/2f,TargetToleranaceVariance/2f));
		}
	}
}
