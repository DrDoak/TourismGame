using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Observer : MonoBehaviour {

	public float detectionRange = 15.0f;

	public List<Observable> VisibleObjs = new List<Observable>();
	float nextScan;
	float scanInterval = 0.5f;
	float postLineVisibleTime = 3.0f;
    Orientation m_orient;

	Dictionary<Observable,float> m_lastTimeSeen;
	// Use this for initialization
	void Start () {
		m_lastTimeSeen = new Dictionary<Observable,float> ();
        m_orient = GetComponent<Orientation>();
		nextScan = UnityEngine.Random.Range (Time.timeSinceLevelLoad,Time.timeSinceLevelLoad + scanInterval);
	}

	void Update() {
		if (Time.timeSinceLevelLoad > nextScan) {
			scanForEnemies ();
		}
	}

	void scanForEnemies() {
        Observable[] allObs = FindObjectsOfType<Observable> ();
		float lts = Time.realtimeSinceStartup;
		foreach (Observable o in allObs) {
			Vector3 otherPos = o.transform.position;
			Vector3 myPos = transform.position;
            float cDist = Vector3.Distance(otherPos, myPos);
            if (o.gameObject != gameObject && cDist < detectionRange && 
                m_orient.FacingPoint(otherPos)) {
				RaycastHit[] hits = Physics.RaycastAll (myPos, otherPos - myPos, cDist);
				Debug.DrawRay (myPos, otherPos - myPos, Color.green);
				float minDist = float.MaxValue;
				foreach (RaycastHit h in hits) {
					GameObject oObj = h.collider.gameObject;
					if (oObj != gameObject && !h.collider.isTrigger) {
						minDist = Mathf.Min(minDist,Vector3.Distance (transform.position,h.point));
					}
				}
				float diff = Mathf.Abs (cDist - minDist);
				if (cDist < minDist) {
                    m_lastTimeSeen[o] = lts;
                    if (!VisibleObjs.Contains (o)) {
                        //Debug.Log("On Sight!: " + o.gameObject + " minDist: " + minDist + " dist: " + cDist);
						OnSight (o);
                    }
				}
			}
		}
		if (VisibleObjs.Count > 0) {
			for (int i= VisibleObjs.Count - 1; i >= 0; i --) {
				Observable o = VisibleObjs [i];
				if (o == null) { // c.gameObject == null) {
					VisibleObjs.RemoveAt (i);
				} else if (m_lastTimeSeen.ContainsKey(o)) {
					
					if (lts - m_lastTimeSeen[o] > postLineVisibleTime) {
                        //Debug.Log ("Cannot see, lts = " + lts + " m_lasttime: " + m_lastTimeSeen [o] + " post: " + postLineVisibleTime);
						o.removeObserver (this);
						//outOfSight (o, true);
						VisibleObjs.RemoveAt (i);
					} else if (Mathf.Abs(lts - m_lastTimeSeen[o]) > 0.05f){
						//Out of sight thing.
					}	
				}
			}
		}
		nextScan = Time.timeSinceLevelLoad + scanInterval;
    }

    internal void OnSight(Observable o) {
		//ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnSight (o));
		if (GetComponent<AICharacter>()) {
			GetComponent<AICharacter> ().OnSight (o);
		}
		o.addObserver (this);
		VisibleObjs.Add (o);
	}
	public bool IsVisible(Observable o) {
		return VisibleObjs.Contains (o);
	}
	void OnDestroy() {
		foreach (Observable o in VisibleObjs) {
			o.removeObserver (this);	
		}
	}

	/*private bool SeeThroughTag( GameObject obj ) {
		return (obj.CompareTag ("JumpThru") || (obj.transform.parent != null &&
			obj.transform.parent.CompareTag ("JumpThru")));
	}*/
}