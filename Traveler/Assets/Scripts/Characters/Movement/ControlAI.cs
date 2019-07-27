using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlAI : CharCustomControl
{
    public Vector3 Destination;

    private NavMeshAgent m_agent;
    private CharacterController m_charControl;
    
    private GameObject m_followTarget;
    private bool m_isFollow = false;
    private float m_tolerance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_charControl = gameObject.GetComponent<CharacterController>();
        m_targetPoint = transform.position;
    }
    void Update()
    {
        m_agent.updateRotation = false;
        if (m_isFollow)
        {
            if (m_followTarget != null)
            {
                m_agent.SetDestination(m_followTarget.transform.position);
            } else
            {
                m_isFollow = false;
            }
        }
    }

    public override InputPacket BaseMovement()
    {
        InputPacket newPacket = new InputPacket();

        m_agent.updatePosition = false;
        m_agent.updateRotation = false;

        if (Vector3.Distance(transform.position, m_targetPoint) > m_tolerance)
        {
            newPacket.InputMove = m_agent.desiredVelocity.normalized;
            m_agent.velocity = m_charControl.velocity;
            Destination = m_agent.destination;
        }

        return newPacket;
    }

    public override void SetTarget(Vector3 target, float tolerance = 4f)
    {
        //Debug.Log("Setting destination to : " + t);
        m_agent.SetDestination(target);
        m_tolerance = tolerance;
        m_targetPoint = target;
    }
    public void SetFollowGameObject(GameObject follow, bool keepFollowing = true)
    {
        m_agent.SetDestination(follow.transform.position);
        if (keepFollowing)
        {
            m_followTarget = follow;
            m_isFollow = true;
        }
    }
}
