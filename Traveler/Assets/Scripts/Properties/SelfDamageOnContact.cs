using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BasicPhysics))]
[RequireComponent (typeof (Attackable))]
public class SelfDamageOnContact : MonoBehaviour
{
	public float MinimumVelocityChangeForDamage = 4.0f;
	public float SelfDamageAmount = 20.0f;
	public float DamageOnContactEnemy = 20.0f;


	private float m_lastSpeed = 0.0f;
	private BasicPhysics m_physics;
	private Attackable m_attackable;
    // Start is called before the first frame update
    void Start()
    {
		m_physics = GetComponent<BasicPhysics> ();
		m_attackable = GetComponent<Attackable> ();
    }

    // Update is called once per frame
    void Update()
    {
		float newSpeed = m_physics.m_trueVelocity.magnitude;
		float delta = newSpeed - m_lastSpeed;
		if (Mathf.Abs(delta) > MinimumVelocityChangeForDamage) { 
			m_attackable.DamageObj (SelfDamageAmount);
		}
		m_lastSpeed = m_physics.m_trueVelocity.magnitude;
    }

	internal void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("Hit Something: " + other.gameObject);
		if (other.GetComponent<Attackable> () != null &&
		    m_attackable.CanAttack (other.GetComponent<Attackable> ().Faction))
			m_attackable.DamageObj (DamageOnContactEnemy);
	}
}
