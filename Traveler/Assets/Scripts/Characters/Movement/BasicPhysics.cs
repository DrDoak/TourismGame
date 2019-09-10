using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicPhysics : MonoBehaviour
{
    private const float VELOCITY_MINIMUM_THRESHOLD = 0.3f;

    // References
    private CharacterController m_controller;
    private NavMeshAgent m_agent;

    //Persistent Stats
    public float DecelerationRatio = 1.0f;
    public float TerminalVelocity = -1f;
    public float GravityForce = -1.0f;
    public bool Floating = false;

    // Particles
    public bool DrawParticles = true;
    public GameObject DefaultParticle;
    public int DefaultNumParticles = 5;
    private int NumParticles = 0;
    private float MAX_PARTICLE_INTERVAL = 5f;
    private Vector3 last_part_position;
    private float MIN_PARTICLE_DISTANCE = 0.5f;
    private float m_lastMovement = 0f;
    private Dictionary<GameObject, GameObject> m_particleSystems;
    private bool m_shouldDrawParticle = false;

    // Tracking movement
    private Vector3 m_accumulatedVelocity = Vector3.zero;
    private Vector3 m_artificialVelocity = new Vector3();
    private List<Force> m_forces;
    private Vector3 m_lastPosition;
    public Vector3 m_trueVelocity;
    private Vector3 m_velocity;
    public Dictionary<Direction, float> TimeCollided { get { return m_timeCollided; } private set { m_timeCollided = value; } }
    public Dictionary<Direction, float> m_timeCollided;
    public bool IsGrounded;

    // Tracking inputed movement
    private Force m_inputedForce;
    private Vector3 m_inputedMove = Vector3.zero;
    public Vector3 InputedMove { get { return m_inputedMove; } }

    //Special Case Variables:
    private float m_gravityCancelTime = 0f;
    private float m_oldFloatingTime = 0f;
    private bool m_oldFloating = false;


    internal void Awake()
    {
        m_oldFloating = Floating;
        m_controller = GetComponent<CharacterController>();
        m_forces = new List<Force>();
        m_inputedForce = new Force();
        m_lastPosition = transform.position;
        m_trueVelocity = new Vector3();
        m_timeCollided = new Dictionary<Direction, float>();
        m_agent = GetComponent<NavMeshAgent>();
        for (int i = 0; i < 4; i++)
            m_timeCollided[(Direction)i] = 0f;
        m_particleSystems = new Dictionary<GameObject, GameObject>();
        if (DrawParticles && DefaultParticle != null)
        {
            m_particleSystems[gameObject] = Instantiate(DefaultParticle, transform);
        }
        last_part_position = transform.position;
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    // Update is called once per frame
    internal void FixedUpdate()
    {
        DecelerateAutomatically(VELOCITY_MINIMUM_THRESHOLD);
        ProcessMovement();
        UpdateFloating();
    }
    internal void Update()
    {
        m_trueVelocity = (transform.position - m_lastPosition) / Time.deltaTime;
        m_lastPosition = transform.position;
        IsGrounded = m_controller.isGrounded;
        if (m_agent != null)
        {
            m_agent.velocity = m_controller.velocity;
        }
        particleProcess();
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
        applyForcesToVelocity();
        processArtificialVelocity();
        m_controller.Move(m_velocity);
    }

    private void applyForcesToVelocity()
    {
        m_inputedForce.MyForce *= Time.fixedDeltaTime;
        
        m_velocity.x = m_inputedForce.MyForce.x;
        m_velocity.z = m_inputedForce.MyForce.z;
        if (Floating)
            m_velocity.y = m_inputedForce.MyForce.y;

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

        if (m_velocity.y > TerminalVelocity && !Floating) 
        {
            if (!m_controller.isGrounded && m_gravityCancelTime <= 0f)
            {
                m_velocity.y += GravityForce * Time.fixedDeltaTime;
            }
            else if (m_controller.isGrounded)
            {
                m_velocity.y += GravityForce * Time.fixedDeltaTime;// * Time.fixedDeltaTime * 6f;
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
    public void AddToVelocity(Vector3 veloc)
    {
        m_accumulatedVelocity.x += veloc.x;
        m_accumulatedVelocity.z += veloc.z;
        AddSelfForce(new Vector3(0f, veloc.y,0f), 0f);
    }
    public void AddSelfForce(Vector3 speed, float duration)
    {
        m_forces.Add(new Force(speed, duration));
    }
    private void storeData(CharData d)
    {
        d.SetFloat("TerminalVelocity", TerminalVelocity);
    }

    private void loadData(CharData d)
    {
        TerminalVelocity = d.GetFloat("TerminalVelocity",TerminalVelocity);
    }

    public bool OnGround()
    {
        return m_controller.isGrounded;
    }
    public Vector3 Velocity()
    {
        return m_trueVelocity;
    }
    public void CancelVerticalMomentum()
    {
        foreach (Force force in m_forces)
        {
            force.MyForce.y = 0f;
        }
        m_velocity.y = 0f;
        m_artificialVelocity.y = 0f;
    }
    public void FreezeInAir(float time)
    {
        if (time > 0f)
        {
            m_oldFloating = Floating;
            m_oldFloatingTime = time;
            Floating = true;
            CancelVerticalMomentum();
            m_accumulatedVelocity = Vector2.zero;
            m_artificialVelocity = Vector2.zero;
        }
        else
        {
            ContinueFromFreeze();
        }
    }
    public void ContinueFromFreeze()
    {
        Floating = m_oldFloating;
    }
    private void UpdateFloating()
    {
        if (m_oldFloatingTime > 0f)
        {
            m_oldFloatingTime -= Time.fixedDeltaTime;
            if (m_oldFloatingTime <= 0f)
                ContinueFromFreeze();
        }
    }

    private void particleProcess()
    {
        if (DrawParticles && m_trueVelocity.magnitude > 0.1f && IsGrounded)
        {
            if (Vector3.Distance(last_part_position, transform.position) > MIN_PARTICLE_DISTANCE ||
                Time.timeSinceLevelLoad - m_lastMovement > MAX_PARTICLE_INTERVAL)
            {
                m_lastMovement = Time.timeSinceLevelLoad;
                last_part_position = transform.position;
                if (NumParticles <= 0)
                {
                    EmitParticle(gameObject, DefaultNumParticles);
                }
                m_shouldDrawParticle = true;
            } else
            {
                m_shouldDrawParticle = false;
            }
        }
    }
    public bool ShouldDrawParticle()
    {
        return m_shouldDrawParticle;
    }
    public void EmitParticle(GameObject origin, int numParticles)
    {
        if (m_particleSystems.ContainsKey(origin))
        {
            m_particleSystems[origin].GetComponent<ParticleSystem>().Emit(numParticles);
        }
    }
    public void AddParticleSystem(GameObject origin, GameObject prefab)
    {
        if (!DrawParticles)
            return;
        if (NumParticles == 0)
        {
            GameObject defaultPart = m_particleSystems[gameObject];
            m_particleSystems.Remove(gameObject);
            Destroy(defaultPart);
        }
        m_particleSystems[origin] = Instantiate(prefab, transform);
        NumParticles++;
    }
    public void RemoveParticleSystem(GameObject origin)
    {
        if (!DrawParticles)
            return;
        GameObject part = m_particleSystems[origin];
        m_particleSystems.Remove(origin);
        Destroy(part);
        NumParticles--;
        if (NumParticles <= 0 && DefaultParticle != null)
        {
            m_particleSystems[gameObject] = Instantiate(DefaultParticle, transform);
        }
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
