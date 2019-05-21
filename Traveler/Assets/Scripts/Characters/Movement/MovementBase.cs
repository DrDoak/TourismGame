using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class FootstepInfo
{
    public bool PlayFootsteps = false;
    public AudioClip FootStepClip = null;
    public float FootStepVolume = 0.3f;
    public float FootStepPitchVariation = 0.25f;
    public float FootStepVolumeVariation = 0.0f;
    public float FootstepInterval = 0.75f;
}

public class InputPacket
{
    public Vector3 InputMove;
    public bool JumpDown;
    public bool JumpHold;
}

public class MovementBase : MonoBehaviour
{

    private BasicPhysics m_physics;
    private const float MIN_JUMP_INTERVAL = 0.2f;
    private const float SMOOTH_TIME = .1f;
    ControlPlayer m_playerCustomControl;
    ControlAI m_aiCustomControl;
    CharCustomControl m_currentControl;

    public bool IsPlayerControl = false;
    public bool CanJump = true;
    public int MidAirJumps = 0;
    [SerializeField]
    private float m_moveSpeed = 8.0f;
    public float MoveSpeed { get { return m_moveSpeed; } private set { m_moveSpeed = value; } }
    [SerializeField]
    private float m_jumpHeight = 4.0f;
    [SerializeField]
    private bool VariableJumpHeight = false;
    public float JumpHeight { get { return m_jumpHeight; } private set { m_jumpHeight = value; } }

    public int CurrentAirJumps = 0;

    // Movement tracking
    protected bool m_jumpHold;
    protected bool m_jumpDown;
    public Vector3 m_inputMove;
    public Vector3 m_jumpVector;
    private float m_jumpVelocity;
    private Vector3 m_velocity;
    private float m_lastJump = 0.0f;
    public FootstepInfo m_FootStepInfo;
    private bool m_variableJumpApplied = false;
    float m_sinceStep = 0f;
    private float m_accelerationTimeX = .1f;
    private float m_accelerationTimeZ = .1f;
    private Orientation m_orient;

    internal void Awake()
    {
        m_physics = GetComponent<BasicPhysics>();

        m_aiCustomControl = GetComponent<ControlAI>();
        m_playerCustomControl = GetComponent<ControlPlayer>();
        m_orient = GetComponent<Orientation>();
        if (CanJump)
            SetJumpHeight(JumpHeight);
        //m_savedCurrentPlayer = IsCurrentPlayer;
        updateCustomControl();
    }

    // Start is called before the first frame update
    void Start()
    {
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnControllableChange(IsCurrentPlayer));
    }

    internal void Update()
    {
        updateCustomControl();
        if (!m_physics.CanMove)
        {
            m_inputMove = new Vector2(0f, 0f);
        }
        if (m_FootStepInfo.PlayFootsteps)
            playStepSounds();
        moveSmoothly();
        currentPlayerControl();
        resetJumps();
    }

    public void SetTargetPoint(Vector3 target)
    {
        m_currentControl.SetTarget(target);
    }
    public void SetJumpHeight(float jumpHeight)
    {
        m_jumpVector.y = (-m_physics.GravityForce * (20f * Mathf.Sqrt(jumpHeight))) + 25f;
        m_jumpHeight = jumpHeight;
    }

    private void updateCustomControl()
    {
        if (IsPlayerControl)
            m_currentControl = m_playerCustomControl;
        else
            m_currentControl = m_aiCustomControl;
    }
    private void playStepSounds()
    {
        if (m_inputMove.x != 0f && m_physics.OnGround())
        {
            m_sinceStep += Time.deltaTime;
            if (m_sinceStep > m_FootStepInfo.FootstepInterval)
            {
                m_sinceStep = 0f;
                /*FindObjectOfType<AudioManager>().PlayClipAtPos(
                    (m_FootStepInfo.FootStepClip == null) ? FXBody.Instance.SFXFootstep : m_FootStepInfo.FootStepClip, transform.position,
                    m_FootStepInfo.FootStepVolume, m_FootStepInfo.FootStepVolumeVariation, m_FootStepInfo.FootStepPitchVariation);*/
            }
        }
        else
        {
            m_sinceStep = m_FootStepInfo.FootstepInterval;
        }
    }

    private void resetJumps()
    {
        if (m_physics.OnGround())
            CurrentAirJumps = MidAirJumps;
    }

    private void moveSmoothly()
    {
        Vector3 targetVel = new Vector3(m_inputMove.x * MoveSpeed,0, m_inputMove.z * MoveSpeed);
        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVel.x, ref m_accelerationTimeX, SMOOTH_TIME);
        m_velocity.z = Mathf.SmoothDamp(m_velocity.z, targetVel.z, ref m_accelerationTimeZ, SMOOTH_TIME);
        m_physics.InputMove(m_velocity, m_inputMove);
    }
    
    private void currentPlayerControl()
    {
        /*if (m_savedCurrentPlayer != IsCurrentPlayer)
        {
            ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnControllableChange(IsCurrentPlayer));
            m_savedCurrentPlayer = IsCurrentPlayer;
        }*/
        InputPacket ip = m_currentControl.BaseMovement();
        if (ip.InputMove.magnitude > 0.01f)
        {
            Direction d = m_orient.VectorToDirection(ip.InputMove);
            m_orient.SetDirection(d);
        }
        m_jumpDown = ip.JumpDown;
        m_jumpHold = ip.JumpHold;
        if (CanJump)
            JumpMovement();
        m_inputMove = ip.InputMove;
    }

    protected void JumpMovement()
    {
        if (m_jumpDown)
        {
            AttemptJump();
        }
        float dt = (Time.timeSinceLevelLoad - m_lastJump);
        if (VariableJumpHeight && dt > 0.1f && dt < 0.2f && m_jumpHold && !m_variableJumpApplied)
        {
            m_variableJumpApplied = true;
            ApplyJumpVector(new Vector2(1f, 0.35f));
        }
    }
    private void AttemptJump()
    {
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnJump());
        if (!CanBasicJump())
            return;
        
        if (VariableJumpHeight)
        {
            ApplyJumpVector(new Vector2(1f, 0.8f));
        }
        else
        {
            ApplyJumpVector(new Vector2(1f, 1f));
        }

        //FindObjectOfType<AudioManager>().PlayClipAtPos(FXBody.Instance.SFXJump, transform.position, 0.3f, 0f, 0.25f);
        m_lastJump = Time.timeSinceLevelLoad;
        m_variableJumpApplied = false;

        //m_physics.AddArtificialVelocity(new Vector3(0f, -0.75f * m_physics.Velocity().y,0f));
        if (!m_physics.OnGround())
            CurrentAirJumps -= 1;
    }
    public bool CanBasicJump()
    {
        if ((Time.timeSinceLevelLoad - m_lastJump) < MIN_JUMP_INTERVAL)
            return false;

        if (!m_physics.OnGround() && CurrentAirJumps <= 0)
            return false;

        return true;
    }
    public bool IsAttemptingMovement()
    {
        return m_inputMove.x != 0f || m_inputMove.z != 0f;
    }
    public void ApplyJumpVector(Vector2 scale)
    {
        float y = m_jumpVector.y;
        Vector2 jv = new Vector2(m_jumpVector.x, y); //- Mathf.Max (0, m_physics.TrueVelocity.y / Time.deltaTime));
        jv.x *= scale.x;
        jv.y *= scale.y;
        m_physics.AddSelfForce(jv, 0f);
    }
}
