using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour {

	public float WindRange = 1f;
	public float MinInterval;
	public float MaxInterval;
	public Vector2 WindForce;
	public float ForceRangeX;
	public float ForceRangeY;
	List<Rigidbody> m_windObjs;

	float m_untilNext;

	// Use this for initialization
	void Start () {
		m_windObjs = new List<Rigidbody> ();
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("WindObj")) {
			m_windObjs.Add (o.GetComponent<Rigidbody> ());
		}
		m_untilNext = Random.Range (MinInterval, MaxInterval);
	}
	
	// Update is called once per frame
	void Update () {
		m_untilNext -= Time.deltaTime;
		if (m_untilNext <= 0) {
			ExertWind ();
		}
	}
	void ExertWind() {
		Vector2 wind = new Vector2 (WindForce.x + Random.Range (-ForceRangeX/2f, ForceRangeX / 2f),
			WindForce.y + Random.Range (-ForceRangeY / 2f, ForceRangeY / 2f));
		foreach (Rigidbody rb in m_windObjs) {
			rb.AddForce (wind);
		}
		m_untilNext = Random.Range (MinInterval, MaxInterval);
	}
	void OnDrawGizmos() {
		Gizmos.color = new Color (0f, 0.5f, 1f, .2f);
		Gizmos.DrawSphere (transform.position,WindRange);
	}
}
