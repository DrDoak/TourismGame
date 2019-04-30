using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxMulti : Hitbox {

	public float refreshTime = 0.1f;
	public float timeSinceLast = 0.0f;

	private bool lastFacingLeft = false;

	void Start() {
		//lastFacingLeft = Creator.GetComponent<PhysicsSS> ().FacingLeft;
		Init ();
	}
	void Update () {
		timeSinceLast += Time.deltaTime;
		if (timeSinceLast > refreshTime) {
			timeSinceLast = 0.0f;
			m_collidedObjs.Clear ();
			foreach (Attackable cont in m_overlappingControl) {
				OnAttackable (cont);
			}
		}		
		OrientToCreator ();
		base.Tick ();
	}


	private void OrientToCreator() {
		/*if (Creator != null && Creator.GetComponent<PhysicsSS> ().FacingLeft != lastFacingLeft) {
			Knockback = new Vector2(Knockback.x * -1f, Knockback.y);
			transform.localPosition = new Vector3( transform.localPosition.x * -1f, transform.localPosition.y);
			lastFacingLeft = Creator.GetComponent<PhysicsSS> ().FacingLeft;
		}*/
	}
}
