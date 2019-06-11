using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Orientation))]
public class CharacterBase : MonoBehaviour
{
    public bool CanControl = true;

    public bool AutoOrientSprite = true;
    public string IdleAnimation = "idle";
    public string WalkAnimation = "walk";
    public string HurtAnimation = "hit";
    public string AirAnimation = "air";
    public string JumpAnimation = "air";
    public string LandAnimation = "land";

    CharacterController m_controller;

    private Dictionary<Attackable, HitInfo> m_hitTargets;
    public Dictionary<Attackable, HitInfo> AttackHistory { get { return m_hitTargets; } private set { m_hitTargets = value; } }

    private AnimatorSprite m_anim;
    private Attackable m_attackable;
    private Orientation m_orient;
    private ActionInfo m_currentAction = null;
  

    private float m_animationSpeed = 2f;

    private Dictionary<HitboxInfo, float> m_queuedHitboxes = new Dictionary<HitboxInfo, float>();
    private bool m_pauseAnim = false;

    private bool m_haveMovedOnGround = false;
    private bool m_hitStateIsGuard = false;

    private string m_actionAnim = "";

    //private Dictionary<ProjectileInfo, float> m_queuedProjectiles = new Dictionary<ProjectileInfo, float>();

    [HideInInspector]
    public AudioClip AttackSound;

    [HideInInspector]
    public float StunTime = 0.0f;

    public Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_attackable = GetComponent<Attackable>();
        m_anim = GetComponent<AnimatorSprite>();
    }

    // Update is called once per frame
    internal void Update()
    {
        activateStunIfDead();
        if (progressStun())
            return;
        updateQueueActions();
        progressAnimation();
        if (progressAction())
            return;
        if (canAct()) {
            
        }
    }
    public void progressAnimation()
    {
        if (m_actionAnim != "")
            m_anim.Play(m_actionAnim);
        else if (!m_pauseAnim)
            standardAnimation();

    }
    public void standardAnimation()
    {
        if (false) //!m_controller.isGrounded)
        {
            m_haveMovedOnGround = false;
            if (GetComponent<MovementBase>().TrueAverageVelocity.y > 0f)
            {
                m_anim.Play(new string[] { JumpAnimation, AirAnimation });
            }
            else
            {
                m_anim.Play(AirAnimation);
            }
        }
        else
        {
            if (GetComponent<MovementBase>().IsAttemptingMovement() && GetComponent<MovementBase>().TrueAverageVelocity.magnitude > 0.025f)
            {
                if (GetComponent<MovementBase>().TrueAverageVelocity.z > 0f)
                {
                    //m_anim.Play(WalkAnimation + "_back");
                    m_anim.Play(new string[] { WalkAnimation + "_back", WalkAnimation },false);
                } else
                {
                    m_anim.Play(WalkAnimation);
                }
                m_haveMovedOnGround = true;
            }
            else
            {
                if (false) //(!m_haveMovedOnGround && m_physics.TimeCollided[Direction.DOWN] < 0.45f)
                {
                    m_anim.Play(new string[] { LandAnimation, IdleAnimation });
                }
                else
                {
                    m_anim.Play(IdleAnimation);
                    //m_anim.Play(new string[] { WalkAnimation + "_back", WalkAnimation }, false);
                }
            }
        }
    }

    // -- STUN --


    public void RegisterStun(float st, bool defaultStun, HitInfo hi, bool guard = false)
    {

        if (m_currentAction != null)
        {
            m_currentAction.OnInterrupt(StunTime, defaultStun, hi);
        }
        if (defaultStun)
        {
            startStunState(st, guard);
        }
    }

    private bool progressStun()
    {
        if (StunTime <= 0.0f)
            return false;
        if (m_hitStateIsGuard)
        {
            m_anim.Play(new string[] { HurtAnimation }, AutoOrientSprite);
        }
        else
        {
            m_anim.Play(HurtAnimation, AutoOrientSprite);
        }
        StunTime -= Time.deltaTime;
        if (m_currentAction != null)
            m_currentAction = null;
        if (StunTime <= 0.0f && m_attackable.Alive)
            EndStun();
        return true;
    }

    private void activateStunIfDead()
    {
        if (m_attackable.Alive)
            return;
        startStunState(3.0f);
    }
    public void EndStun()
    {
        if (m_attackable.Alive)
        {
            CanControl = true;
            StunTime = 0.0f;
        }
    }
    public void SetPause(bool p)
    {
        m_pauseAnim = p;
    }
    private void startStunState(float st, bool guard = false)
    {
        //Debug.Log ("Starting Hit State with Stun: "+ st);
        EndAction();
        StunTime = st;
        m_hitStateIsGuard = guard;
        CanControl = false;
    }

    // --- ACTIONS

    
    private bool progressAction()
    {
        if (m_currentAction == default(ActionInfo))
            return false;
        m_currentAction.Progress();
        return true;
    }

    public void OnActionProgressed(AttackState state)
    {
        switch (state)
        {
            case AttackState.STARTUP:
                OnActionStart();
                break;
            case AttackState.RECOVERY:
                OnActionRecover();
                break;
            case AttackState.INACTIVE:
                EndAction();
                break;
        }
    }

    private void OnActionStart()
    {
        /*if (m_currentAction.m_SoundInfo.AttackFX)
			AddEffect(m_currentAction.m_SoundInfo.AttackFX, m_currentAction.m_AttackAnimInfo.RecoveryTime + 0.2f);*/
        //m_anim.Play(m_currentAction.m_AttackAnimInfo.StartUpAnimation, true);
        m_actionAnim = m_currentAction.m_AttackAnimInfo.StartUpAnimation;
        m_anim.SetSpeed(m_currentAction.m_AttackAnimInfo.AnimSpeed * m_animationSpeed);
        //Debug.Log("On action start");
    }

    private void OnActionRecover()
    {
        //m_anim.Play(m_currentAction.m_AttackAnimInfo.RecoveryAnimation, true);
        m_actionAnim = m_currentAction.m_AttackAnimInfo.RecoveryAnimation;
        //Debug.Log("On action recover");
    }

    public void SkipActionToEnd()
    {
        if (m_currentAction != null)
        {
            m_currentAction.OnInterrupt(0, true, new HitInfo());
        }
        EndAction();
    }

    public void EndAction()
    {
        CanControl = true;
        GetComponent<BasicPhysics>().CanMove = true;
        m_currentAction = null;
        m_anim.SetSpeed(1.0f);
        m_actionAnim = "";
    }

    public void QueueHitbox(HitboxInfo hi, float delay)
    {
        m_queuedHitboxes.Add(hi, Time.timeSinceLevelLoad + delay);
    }
    private void updateQueueActions()
    {
        Dictionary<HitboxInfo, float> newQueue = new Dictionary<HitboxInfo, float>();
        foreach (HitboxInfo hi in m_queuedHitboxes.Keys)
        {
            /*if (Time.timeSinceLevelLoad > m_queuedHitboxes[hi])
                 GetComponent<HitboxMaker>().CreateHitbox(hi);
             else
                 newQueue.Add(hi, m_queuedHitboxes[hi]); */
        }
        m_queuedHitboxes = newQueue;
        /* Dictionary<ProjectileInfo, float> newQueue2 = new Dictionary<ProjectileInfo, float>();
         foreach (ProjectileInfo pi in m_queuedProjectiles.Keys)
         {
             /*if (Time.timeSinceLevelLoad > m_queuedProjectiles[pi])
                 CreateProjectile(pi);
             else
                 newQueue2.Add(pi, m_queuedProjectiles[pi]);
         }
         m_queuedProjectiles = newQueue2; */
    }

    /* public void QueueProjectile(ProjectileInfo pi, float delay)
     {
         m_queuedProjectiles.Add(pi, Time.timeSinceLevelLoad + delay);
     }*/
    bool canAct()
    {
        return (StunTime <= 0 && CanControl);
    }


    // MAY NEED TO REFACTOR

    public void RegisterHit(GameObject otherObj, HitInfo hi, HitResult hr)
    {
        //Debug.Log ("Collision: " + this + " " + otherObj);
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnHitConfirm(hi, otherObj, hr));
        //Debug.Log ("Registering hit with: " + otherObj);
        if (otherObj.GetComponent<Attackable>() != null)
        {
            m_hitTargets[otherObj.GetComponent<Attackable>()] = hi;
        }
        if (m_currentAction != null)
            m_currentAction.OnHitConfirm(otherObj, hi, hr);
    }

    public void TryAction(ActionInfo ai)
    {
        if (!canAct())
            return;
        m_currentAction = ai;
        GetComponent<BasicPhysics>().CanMove = false;
        CanControl = false;
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnAttack(m_currentAttack));
        if (m_currentAction != null)
        {
            m_currentAction.ResetAndProgress();
        }
    }
}
