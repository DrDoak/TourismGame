using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CharacterBase))]
[RequireComponent (typeof (MovementBase))]
public class OffenseAI : MonoBehaviour {

	public List<AttackInfo> allAttacks;
	AttackInfo currentAttack;
	public MovementBase CurrentTarget;
	private Vector3 m_targetOffset;
	public Vector3 TargetPoint;

	public float baseSpacing = 1.0f;
	//public float baseReactionSpeed = 1.0f;
	//public float baseDecisionMaking = 1.0f;
	public float baseAggression = 0.5f;

	float spacing;
	//float reactionSpeed;
	//float decisionMaking;
	float aggression;
    CharacterBase m_charBase;
	MovementBase m_movement;

	public string currentAction = "wait";

	private const float DETERMINATION_INTERVAL = 0.05f;
	private float m_nextDetermination;

	private float m_minDistance;

	void Start () {
		spacing = baseSpacing;
		//reactionSpeed = baseReactionSpeed;
		//decisionMaking = baseDecisionMaking;
		aggression = baseAggression;
		allAttacks = new List<AttackInfo>();
		foreach (AttackInfo ai in GetComponents<AttackInfo> ()) {
			if (ai.name != "sheath" && ai.name != "unsheath") {
				allAttacks.Add (ai);
			}
		}
		m_charBase = GetComponent<CharacterBase> ();
		m_movement = GetComponent<MovementBase> ();
		m_nextDetermination = Random.Range (0f, DETERMINATION_INTERVAL);
	}

	void Update () {
		/*if (CurrentTarget != null && !m_movement.IsCurrentPlayer && !m_charBase.IsAttacking()) {
			if (currentAction == "wait") {
				decideNextAction ();
			} else if (currentAction == "moveToTarget") {
				if (CurrentTarget.transform.position.x > transform.position.x) {
					TargetPoint = CurrentTarget.transform.position + m_targetOffset;
				} else {
					TargetPoint = CurrentTarget.transform.position + new Vector3(-m_targetOffset.x,m_targetOffset.y,0f);
				}

				if (Vector3.Distance (transform.position, TargetPoint) < m_movement.m_minDistance) {
					m_movement.FacePoint (CurrentTarget.transform.position);
				} else {
					m_movement.SetTargetPoint (TargetPoint, m_minDistance);
				}
				decideNextAction ();
			} else if (currentAction == "attack") {
				if (!m_charBase.IsAttacking()) {
					decideNextAction ();
				}
			}
		}*/
	}

	public void decideNextAction() {
		/*Vector3 otherPos = CurrentTarget.transform.position;
		float dir = (GetComponent<PhysicsSS> ().FacingLeft) ? -1f : 1f;

		if (Time.timeSinceLevelLoad > m_nextDetermination) {
			if (Random.value < (aggression * 0.1f)) {
				foreach (AttackInfo ainfo in allAttacks) {
					float xDiff = Mathf.Abs (transform.position.x + (dir * ainfo.m_AIInfo.AIPredictionOffset.x) - otherPos.x);
					float yDiff = Mathf.Abs (transform.position.y + ainfo.m_AIInfo.AIPredictionOffset.y - otherPos.y);
					float p = Random.value;
					if ((ainfo.m_AIInfo.AIPredictionHitbox.x) +
					    (ainfo.m_AIInfo.AIPredictionHitbox.x) * Random.Range (0f, 1f - spacing) > xDiff &&
					    (ainfo.m_AIInfo.AIPredictionHitbox.y) +
					    (ainfo.m_AIInfo.AIPredictionHitbox.y) * Random.Range (0f, 1f - spacing) > yDiff &&
					    p < ainfo.m_AIInfo.Frequency) {
						m_charBase.TryAttack (ainfo.AttackName);
						currentAction = "attack";
						allAttacks.Reverse ();
						break;
					}
				}
			}
			m_nextDetermination = Time.timeSinceLevelLoad + DETERMINATION_INTERVAL;
		}
		currentAction = "moveToTarget";*/
	}

	public void commitToAction() {}

	public void setTarget(MovementBase c) {
		setTarget (c, new Vector3 (), 1f);
	}

	public void setTarget(MovementBase c, Vector3 offset, float tolerance = 1f) {
		// Debug.Log ("Setting target to: offset: " + offset + " t: " + tolerance);
		m_minDistance = tolerance;
		m_targetOffset = new Vector3 (offset.x, offset.y, 0f);
		CurrentTarget = c;
	}
}