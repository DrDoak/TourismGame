using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {NEUTRAL,AGGRESSIVE,ATTACK};

public class Task : MonoBehaviour {

	public AITaskManager MasterAI;
	public TaskType MyTaskType;
	public bool IsInitialTask = false;

	bool m_active = false;

	public List<Transition> TransitionsTo;
	public List<Transition> TransitionsFrom;

	public GameObject Target;
    public Dictionary<string, GameObject> TargetDict;
	public AITaskManager ParentAI;

	// Use this for initialization
	void Start () {
		
	}

	public void Init() {
		TransitionsTo = new List<Transition> ();
		TransitionsFrom = new List<Transition> ();

		foreach (Transition t in GetComponents<Transition>()) {
			if (t.TypeOfTransition == TransitionType.FROM_THIS_TASK) {
				t.OriginTask = this;
				TransitionsFrom.Add (t);
			} else {
				t.TargetTask = this;
				TransitionsTo.Add (t);
			}
		}
	}
	
	// Update is called once per frame
	public void OnUpdate () {
		foreach (Transition t in TransitionsFrom) {
			if (t.isActiveAndEnabled)
				t.OnUpdate ();
		}
		if (m_active) {
			OnActiveUpdate ();
		}
	}

	public void OnSight(Observable o) { 
		foreach (Transition t in TransitionsFrom) {
			if (t.isActiveAndEnabled)
				t.OnSight (o);
		}
	}

	public void OnHit(HitInfo hb) { 
		foreach (Transition t in TransitionsFrom) {
			if (t.isActiveAndEnabled)
				t.OnHit (hb);
		}
	}

    public void OnStart()
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnStart();
        }
    }

    public void OnTime()
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnTime();
        }
    }
    public void OnEnterZone(Zone z)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnEnterZone(z);
        }
    }
    public void OnExitZone(Zone z)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnExitZone(z);
        }
    }
    public void RequestTransition(Task t) {
	}

	public void RequestTransition(TaskType tt) {
	}

	public void SetActive(bool act) {
		m_active = act;
	}

	public virtual void OnTransition() {}

	public virtual void OnActiveUpdate() {
	}
}
