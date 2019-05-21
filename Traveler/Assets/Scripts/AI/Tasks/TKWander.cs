using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKWander : Task {

	public Vector2 WanderRange = new Vector2 ();
	public Vector3 WanderCenterOffset = new Vector3 ();
	public GameObject WanderAroundObject = null;
	public float TargetPositionTolerance = 0f;
	public float TargetToleranaceVariance = 0f;

	public float TimeStop = 0f;
	public float TimeStopVariance = 0f;

	public Vector3 NextTarget;
	public bool DrawWanderArea = false;

	public float m_nextTimeStop = 0f;
	public float m_currentTimeStopped = 0f;
	private Vector3 startingPoint;

	// Use this for initialization
	void Start () {
		Init ();
		startingPoint = transform.parent.position;
	}

	// Update is called once per frame
	public override void OnActiveUpdate () {
		if (NextTarget != null) {
			float d = Vector2.Distance (new Vector2 (transform.position.x, transform.position.y),
				          new Vector2 (NextTarget.x, NextTarget.y));
			if (d < TargetPositionTolerance) {
				m_currentTimeStopped += Time.deltaTime;
				if (m_currentTimeStopped > m_nextTimeStop)
					SetNextTarget ();
			}
		}
	}

	private void SetNextTarget() {
		Vector3 c = startingPoint + WanderCenterOffset;
		if (WanderAroundObject != null)
			c = WanderAroundObject.transform.position;
		NextTarget = new Vector3 (c.x + Random.Range (-WanderRange.x / 2f, WanderRange.x / 2f),
			c.y + Random.Range (-WanderRange.y / 2f, WanderRange.y / 2f), c.z);
		float tol = TargetPositionTolerance + Random.Range (-TargetToleranaceVariance / 2f, TargetToleranaceVariance / 2f);
		MasterAI.GetComponent<MovementBase> ().SetTargetPoint (NextTarget);

		m_currentTimeStopped = 0f;
		m_nextTimeStop = TimeStop + Random.Range (-TimeStopVariance / 2f, TimeStopVariance / 2f);
	}

	public override void OnTransition () {
		if (startingPoint == Vector3.zero)
			startingPoint = MasterAI.transform.position;
		SetNextTarget ();
	}

	void OnDrawGizmos() {
		if (DrawWanderArea) {
			Gizmos.color = new Color (0f, 0.5f, 0.9f, .25f);
			Vector3 centerPoint = new Vector3 ();
			if (startingPoint == null) {
				centerPoint = transform.parent.transform.position + WanderCenterOffset;
			} else {
				centerPoint = startingPoint + WanderCenterOffset;
			}
			if (WanderAroundObject != null)
				centerPoint = WanderAroundObject.transform.position;

			Gizmos.DrawCube (centerPoint, new Vector3 (WanderRange.x, WanderRange.y, 0.1f));
		}
	}
}
