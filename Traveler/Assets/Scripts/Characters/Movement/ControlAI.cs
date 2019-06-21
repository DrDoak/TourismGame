using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlAI : CharCustomControl
{
    private NavMeshAgent m_agent;
    private CharacterController m_charControl;
    
    private GameObject m_followTarget;
    private bool m_isFollow = false;

    public Vector3 Destination;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_charControl = gameObject.GetComponent<CharacterController>();
       
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

        newPacket.InputMove = m_agent.desiredVelocity.normalized;
        m_agent.velocity = m_charControl.velocity;
        Destination = m_agent.destination;

        return newPacket;
    }

    public override void SetTarget(Vector3 t)
    {
        Debug.Log("Setting destination to : " + t);
        m_agent.SetDestination(t);
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
