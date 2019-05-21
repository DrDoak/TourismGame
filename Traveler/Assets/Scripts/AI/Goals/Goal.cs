using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    protected AICharacter m_masterAI;

    private float m_goalPriority;
    public virtual float GoalPriority { get { return m_goalPriority; } private set { m_goalPriority = value; } }

    public virtual string GoalName { get { return "DefaultGoal"; } }

    public virtual void OnHit(HitInfo hb) { }

    public virtual void OnSight(Observable o) { }

    public virtual void OnStart() { }

    public virtual void OnTime() { }

    public virtual void OnEnterZone(Zone z) { }

    public virtual void OnExitZone(Zone z) { }

    public void SetGoalPriority(float newP)
    {
        m_goalPriority = newP;
    }
    public void SetMasterAI(AICharacter master)
    {
        m_masterAI = master;
    }
}
