using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlDebugAI : CharCustomControl
{
    public Transform _playerTrans;
    public float _speed = 2;
    public float _turnSpeed = 3;

    private NavMeshAgent m_agent;
    private Vector3 _desVelocity;
    private CharacterController m_charControl;
    private Vector3 m_destination;
    private Camera m_cam;

    void Start()
    {

        m_agent = this.gameObject.GetComponent<NavMeshAgent>();
        m_charControl = this.gameObject.GetComponent<CharacterController>();
        m_agent.destination = this._playerTrans.position;

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

        Vector3 lookPos;
        Quaternion targetRot;

        //m_agent.destination = this._playerTrans.position;
        this._desVelocity = m_agent.desiredVelocity;

        m_agent.updatePosition = false;
        m_agent.updateRotation = false;

        lookPos = this._playerTrans.position - this.transform.position;
        lookPos.y = 0;
        targetRot = Quaternion.LookRotation(lookPos);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * this._turnSpeed);

        m_charControl.Move(this._desVelocity.normalized * this._speed * Time.deltaTime);

        m_agent.velocity = m_charControl.velocity;

        return newPacket;
    }
}
