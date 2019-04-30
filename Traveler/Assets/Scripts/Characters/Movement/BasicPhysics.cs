﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPhysics : MonoBehaviour
{
    private const float VELOCITY_MINIMUM_THRESHOLD = 0.3f;

    // References
    private CharacterController m_controller;

    //Persistent Stats
    public float DecelerationRatio = 1.0f;
    public float TerminalVelocity = -1f;
    public float GravityForce = -1.0f;
    public bool CanMove = true;

    // Tracking movement
    private Vector3 m_accumulatedVelocity = Vector3.zero;
    private Vector3 m_artificialVelocity = new Vector3();
    private List<Force> m_forces;
    private Vector3 m_lastPosition;
    public Vector3 m_trueVelocity;
    private Vector3 m_velocity;
    public Dictionary<Direction, float> TimeCollided { get { return m_timeCollided; } private set { m_timeCollided = value; } }
    public Dictionary<Direction, float> m_timeCollided;

    // Tracking inputed movement
    private Force m_inputedForce;
    private Vector3 m_inputedMove = Vector3.zero;
    public Vector3 InputedMove { get { return m_inputedMove; } }

    //Special Case Variables:
    private float m_gravityCancelTime = 0f;
    

    internal void Awake()
    {
        m_controller = GetComponent<CharacterController>();
        m_forces = new List<Force>();
        m_inputedForce = new Force();
        m_lastPosition = transform.position;
        m_trueVelocity = new Vector3();
        m_timeCollided = new Dictionary<Direction, float>();
        for (int i = 0; i < 4; i++)
            m_timeCollided[(Direction)i] = 0f;

        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    // Update is called once per frame
    internal void FixedUpdate()
    {
        DecelerateAutomatically(VELOCITY_MINIMUM_THRESHOLD);
        ProcessMovement();
        
    }
    internal void Update()
    {
        m_trueVelocity = (transform.position - m_lastPosition) / Time.deltaTime;
        m_lastPosition = transform.position;
    }

    private void DecelerateAutomatically(float threshold)
    {
        if (m_accumulatedVelocity.sqrMagnitude > threshold)
            m_accumulatedVelocity *= (1.0f - Time.fixedDeltaTime * DecelerationRatio * 3.0f);
        else
            m_accumulatedVelocity = Vector2.zero;
    }
    public void InputMove(Vector3 veloc, Vector3 input)
    {
        m_inputedMove = input;
        m_inputedForce.MyForce = veloc;
    }
    public bool IsAttemptingMovement()
    {
        return m_inputedMove.x != 0.0f && m_inputedMove.z != 0.0f;
    }

    private void ProcessMovement()
    {
        checkCanMove();
        applyForcesToVelocity();
        processArtificialVelocity();
        m_controller.Move(m_velocity);
    }

    private void checkCanMove()
    {
        if (CanMove)
            return;
        m_inputedForce.MyForce = new Vector3(0, 0, 0);
    }

    private void applyForcesToVelocity()
    {
        m_inputedForce.MyForce *= Time.fixedDeltaTime;
        
        m_velocity.x = m_inputedForce.MyForce.x;
        m_velocity.z = m_inputedForce.MyForce.z;

        List<Force> forcesToRemove = new List<Force>();
        bool noYForces = true;
        foreach (Force force in m_forces)
        {
            m_velocity += force.MyForce * Time.fixedDeltaTime;
            force.Duration -= Time.fixedDeltaTime;
            if (force.MyForce.y != 0f)
                noYForces = false;
            if (force.Duration < 0f)
                forcesToRemove.Add(force);
        }
        if (m_controller.isGrounded && noYForces)
            m_velocity.y = 0f;
        m_gravityCancelTime -= Time.deltaTime;
        foreach (Force force in forcesToRemove)
            m_forces.Remove(force);

        if (m_velocity.y > TerminalVelocity)
        {
            if (!m_controller.isGrounded && m_gravityCancelTime <= 0f)
            {
                m_velocity.y += GravityForce * Time.fixedDeltaTime;
            }
            else if (m_controller.isGrounded)
            {
                m_velocity.y += GravityForce;// * Time.fixedDeltaTime * 6f;
            }
        }
        m_velocity += m_accumulatedVelocity * Time.fixedDeltaTime;
    }

    public void AddArtificialVelocity(Vector3 vel)
    {
        m_artificialVelocity += vel;
    }
    private void processArtificialVelocity()
    {
        if (m_artificialVelocity.magnitude > 0f)
        {
            m_velocity += m_artificialVelocity;
            m_artificialVelocity = new Vector3();
        }
    }
    public void AddSelfForce(Vector3 force, float duration)
    {
        m_forces.Add(new Force(force, duration));
    }
    private void storeData(CharData d)
    {
        d.PersistentFloats["TerminalVelocity"] = TerminalVelocity;
    }

    private void loadData(CharData d)
    {
        TerminalVelocity = d.PersistentFloats["TerminalVelocity"];
    }

    public bool OnGround()
    {
        return m_controller.isGrounded;
    }
    public Vector3 Velocity()
    {
        return m_trueVelocity;
    }
    /*
     private void ProcessWater() {
     if (ReactToWater && Submerged && m_gravityCancelTime <= 0f)
       {
           m_velocity.y += BuoyancyScale * Time.fixedDeltaTime;
       }
       else if (m_velocity.y > TerminalVelocity && !Floating)
       {
           if (!m_collisions.below && m_gravityCancelTime <= 0f)
           {
               m_velocity.y += GravityForce * Time.fixedDeltaTime;
           }
           else if (m_collisions.below)
           {
               m_velocity.y += GravityForce * Time.fixedDeltaTime * 6f;
           }
       }*/
}