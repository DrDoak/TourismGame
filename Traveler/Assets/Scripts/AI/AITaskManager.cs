using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITaskManager : MonoBehaviour {

	public Task m_currentTask;

	Dictionary<TaskType, List<Task>> MyTasks;

	Dictionary<TaskType, List<Transition>> GenericTransitions;

	// Use this for initialization
	void Awake () {
		GenericTransitions = new Dictionary<TaskType, List<Transition>> ();
		ReloadTasks ();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_currentTask != null)
        {
            m_currentTask.OnUpdate();

            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnUpdate();
            }
        }
    }

	void ReloadTasks() {
		Task[] tList = GetComponentsInChildren<Task> ();
		MyTasks = new Dictionary<TaskType, List<Task>>();
		foreach (Task t in tList) {
			t.MasterAI = this;
			if (m_currentTask == null || t.IsInitialTask)
				TransitionToTask (t);
			AddTask (t);
		}
	}

    public void SetBehaviour( GameObject o)
    {
        GameObject newG = Instantiate(o);
        Task[] tList = newG.GetComponentsInChildren<Task>();
        foreach (Task t in tList)
        {
            t.transform.parent = transform;
        }
        ReloadTasks();
        Destroy(newG);
    }

	public void OnHit(HitInfo hb) { 
		if (m_currentTask != null) {
			m_currentTask.OnHit (hb);
			foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType]) {
				t.OnHit (hb);
			}
		}
	}
	public void OnSight(Observable o) { 
		if (m_currentTask != null) {
			m_currentTask.OnSight (o);
			foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType]) {
				t.OnSight (o);
			}
		}
	}
    public void OnStart()
    {
        if (m_currentTask != null)
        {
            m_currentTask.OnStart();
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnStart();
            }
        }
    }
    public void OnTime()
    {
        if (m_currentTask != null)
        {
            m_currentTask.OnTime();
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnTime();
            }
        }
    }
    public void OnEnterZone(Zone z)
    {
        if (m_currentTask != null)
        {
            m_currentTask.OnEnterZone(z);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnEnterZone(z);
            }
        }
    }
    public void OnExitZone(Zone z)
    {
        if (m_currentTask != null)
        {
            m_currentTask.OnExitZone(z);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnExitZone(z);
            }
        }
    }
    public void TransitionToTask(Task t) {
		if (m_currentTask != null)
			m_currentTask.SetActive (false);
		m_currentTask = t;
		m_currentTask.SetActive (true);
		m_currentTask.OnTransition ();
		if (!GenericTransitions.ContainsKey (m_currentTask.MyTaskType))
			GenericTransitions [m_currentTask.MyTaskType] = new List<Transition> ();
	}

	public void TransitionToTask(TaskType tt) {
		if (MyTasks.ContainsKey(tt))
			TransitionToTask (MyTasks [tt] [0]);
	}

	public void AddTask(Task t) {
		t.Init ();
		if (!MyTasks.ContainsKey(t.MyTaskType))
			MyTasks[t.MyTaskType] = new List<Task>();
		
		if (!MyTasks [t.MyTaskType].Contains (t)) {
			MyTasks [t.MyTaskType].Add (t);
			addTransitions (t.TransitionsTo);
			foreach(Transition tt in t.TransitionsFrom) {
				tt.MasterAI = this;
			}
		}
	}

	public void RemoveTask(Task t) {
		if (!MyTasks.ContainsKey(t.MyTaskType))
			MyTasks[t.MyTaskType] = new List<Task>();
		if (MyTasks [t.MyTaskType].Contains (t))
			MyTasks [t.MyTaskType].Remove (t);
		removeTransitions (t.TransitionsTo);
	}

	void addTransitions(List<Transition> lt) {
		foreach (Transition t in lt) {
			t.MasterAI = this;
			if (!GenericTransitions [t.OriginType].Contains (t))
				GenericTransitions [t.OriginType].Add (t);
		}
	}

	void removeTransitions (List<Transition> lt) {
		foreach (Transition t in lt) {
			if (GenericTransitions [t.OriginType].Contains (t))
				GenericTransitions [t.OriginType].Remove (t);
		}
	}

}
