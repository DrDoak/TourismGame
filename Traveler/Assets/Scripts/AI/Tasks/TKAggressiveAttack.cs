using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKAggressiveAttack : Task {

	public Vector3 TargetPositionOffset = new Vector3();
	public float TargetPositionTolerance = 5f;
	public float TargetToleranaceVariance = 0f;

	public bool OnlyUseSpecificAttacks = false;
    [SerializeField]
    private float DistanceToTarget = 0f;
	public List<string> AcceptableAttacks;

    private MovementBase m_movement;
    private CharacterBase m_charBase;
    private Vector3 m_currentTargetPoint;

    [SerializeField]
    private string currentAction = "wait";

    private const float DETERMINATION_INTERVAL = 0.05f;
    private float m_nextDetermination;

    private float m_minDistance;
    public float baseSpacing = 1.0f;

    //public float baseReactionSpeed = 1.0f;
    //public float baseDecisionMaking = 1.0f;
    public float baseAggression = 100f;

    float spacing;
    //float reactionSpeed;
    //float decisionMaking;
    float aggression;

    // Use this for initialization
    void Start () {
		Init ();
        aggression = baseAggression;
        m_movement = MasterAI.GetComponent<MovementBase>();
        m_charBase = MasterAI.GetComponent<CharacterBase>();
    }

    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null) {

            if (currentAction == "wait") {
                m_movement.FacePoint2D(GetTargetObj().transform.position);
                decideNextAction ();
            } else if (currentAction == "moveToTarget") {   
                if (GetTargetObj().transform.position.x > MasterAI.transform.position.x) {
                    m_currentTargetPoint = GetTargetObj().transform.position + TargetPositionOffset;
                } else {
                    m_currentTargetPoint = GetTargetObj().transform.position + new Vector3(-TargetPositionOffset.x, TargetPositionOffset.y,TargetPositionOffset.z);
                }
                DistanceToTarget = Vector3.Distance(MasterAI.transform.position, m_currentTargetPoint);
                //Debug.Log("from " + MasterAI.transform.position + " to " + m_currentTargetPoint + " Actual distance: " + DistanceToTarget + " tolerance " + TargetPositionTolerance);
                if (DistanceToTarget > TargetPositionTolerance)
                    m_movement.SetTargetPoint(m_currentTargetPoint, TargetPositionTolerance);
                else
                {
                    m_movement.FacePoint2D(GetTargetObj().transform.position);
                }
                    
                decideNextAction ();
            } else if (currentAction == "attack") {
                m_movement.FacePoint2D(GetTargetObj().transform.position);
                if (m_charBase.IsAutonomous) {
                    decideNextAction ();
                }
            }
        }
    }
    void decideNextAction()
    {
        Vector3 otherPos = GetTargetObj().transform.position;
		float dir = (MasterAI.GetComponent<Orientation> ().FacingLeft) ? -1f : 1f;
		if (Time.timeSinceLevelLoad > m_nextDetermination) {
			if (Random.value < (aggression * 0.1f)) {
                Debug.Log("Aggression triggerred");
				foreach (ActionInfo ainfo in m_charBase.GetValidActions(otherPos)) {
					float p = Random.value;
					if (p < ainfo.m_AIInfo.Frequency) {
						m_charBase.TryAction (ainfo);
						currentAction = "attack";
						break;
					}
				}
			}
			m_nextDetermination = Time.timeSinceLevelLoad + DETERMINATION_INTERVAL;
		}
		currentAction = "moveToTarget";
    }

    // Update is called once per frame
    public override void OnTransition () {
		/*if (GetTargetObj() != null) {
			MasterAI.GetComponent<OffenseAI> ().setTarget (GetTargetObj().GetComponent<MovementBase> (),
				TargetPositionOffset,TargetPositionTolerance + Random.Range(-TargetToleranaceVariance/2f,TargetToleranaceVariance/2f));
		}*/
	}
}
