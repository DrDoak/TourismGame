using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlDebugAI : CharCustomControl
{
    private NavMeshAgent m_agent;
    private CharacterController m_charControl;
    private Vector3 m_destination;
    private Camera m_cam;

    void Start()
    {

        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_charControl = gameObject.GetComponent<CharacterController>();
        m_agent.destination = transform.position;

        m_destination = new Vector3();
        m_cam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                m_agent.SetDestination(hit.point);
            }
        }
        return;
    }


    public override InputPacket BaseMovement()
    {
        InputPacket newPacket = new InputPacket();

        m_agent.updatePosition = false;
        m_agent.updateRotation = false;

        newPacket.InputMove = m_agent.desiredVelocity.normalized;
        m_agent.velocity = m_charControl.velocity;

        return newPacket;
    }
}
