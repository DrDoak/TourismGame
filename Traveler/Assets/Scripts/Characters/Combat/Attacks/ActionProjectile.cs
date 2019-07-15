using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileInfo {
	public float Delay = 0f;
	public GameObject Projectile = null;
	public Vector2 ProjectileCreatePos = new Vector2 (1.0f, 0f);
	public bool AimTowardsTarget = false;
	public float MaxAngle = 360f;
	public Vector2 ProjectileAimDirection = new Vector2 (1.0f, 0f);
	public float ProjectileSpeed = 10.0f;
	public int PenetrativePower = 1;
	public float Damage = 10.0f;
	public float Stun = 0.3f;
	public float HitboxDuration = 0.5f;
	public Vector2 Knockback = new Vector2(10.0f,10.0f);
	public ElementType Element = ElementType.PHYSICAL;
}


public class ActionProjectile : ActionInfo {
	
	[SerializeField]
	private List<ProjectileInfo> m_ProjectileData;

	protected override void OnAttack()
	{
		base.OnAttack();
		if (m_ProjectileData.Count != 0) {
			createProjectiles ();
		}
	}

	protected void createProjectiles()
	{
		//m_hitboxMaker.AddHitType(HitType);
		foreach (ProjectileInfo pi in m_ProjectileData) {
			if (pi.Delay <= 0f)
				GetComponent<CharacterBase> ().CreateProjectile(pi);
			else
				GetComponent<CharacterBase> ().QueueProjectile (pi, pi.Delay);
		}
		//		Vector2 offset = m_physics.OrientVectorToDirection(m_HitboxInfo.HitboxOffset);
		//		m_hitboxMaker.CreateHitbox(m_HitboxInfo.HitboxScale, offset, m_HitboxInfo.Damage,
		//			m_HitboxInfo.Stun, m_HitboxInfo.HitboxDuration, m_HitboxInfo.Knockback, true, true,m_HitboxInfo.Element,m_HitboxInfo.ApplyProps);
	}
}
