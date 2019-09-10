using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemporaryProperty
{
    public bool Timed = true;
    public float Time = 5.0f;
    public string PropertyName;
}
[System.Serializable]
public class BuffInfo
{
    public float HealthDifference;
    public List<TemporaryProperty> ApplyProperty;
    public List<Resistence> Resistances;
    public float duration = 5.0f;
}

public class ActionSelf : ActionInfo
{
    [SerializeField]
    private BuffInfo m_buffData;

    protected override void OnAttack()
    {
        base.OnAttack();
        applyBuff();
    }

    protected void applyBuff()
    {
        //m_hitboxMaker.AddHitType(HitType);
        foreach(TemporaryProperty tp in m_buffData.ApplyProperty)
        {

        }
        if (m_buffData.HealthDifference != 0f)
        {
            m_hitboxMaker.GetComponent<Attackable>().DamageObj(-m_buffData.HealthDifference);
        }
        foreach(Resistence r in m_buffData.Resistances)
        {
            m_hitboxMaker.GetComponent<Attackable>().AddResistence(r);
        }
        //		Vector2 offset = m_physics.OrientVectorToDirection(m_HitboxInfo.HitboxOffset);
        //		m_hitboxMaker.CreateHitbox(m_HitboxInfo.HitboxScale, offset, m_HitboxInfo.Damage,
        //			m_HitboxInfo.Stun, m_HitboxInfo.HitboxDuration, m_HitboxInfo.Knockback, true, true,m_HitboxInfo.Element,m_HitboxInfo.ApplyProps);
    }
}
