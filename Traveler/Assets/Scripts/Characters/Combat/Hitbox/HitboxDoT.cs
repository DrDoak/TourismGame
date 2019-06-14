using UnityEngine;
using System.Collections.Generic; 

public class HitboxDoT : Hitbox {
	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	new void Update () {
		Tick ();
	}
	protected override void Tick() {
		if (!m_hasDuration || Duration > 0.0f) {
			foreach(Attackable a in m_overlappingControl) {
				a.TakeHit (ToHitInfo());
			}
			Duration = Duration - Time.deltaTime;
		} else if (m_hasDuration) {
			GameObject.Destroy (gameObject);
		}
	}

	new internal void OnTriggerEnter(Collider other) {
		if (other.gameObject.GetComponent<Attackable>() && !m_overlappingControl.Contains(other.gameObject.GetComponent<Attackable> ())) {
			m_overlappingControl.Add (other.gameObject.GetComponent<Attackable> ()); 
		}
	} 
	new internal void OnTriggerExit(Collider other) {
		if (other.gameObject.GetComponent<Attackable> () && m_overlappingControl.Contains(other.gameObject.GetComponent<Attackable> ())) {
			m_overlappingControl.Remove (other.gameObject.GetComponent<Attackable> ()); //Removes the object from the list
		}
	}
}
