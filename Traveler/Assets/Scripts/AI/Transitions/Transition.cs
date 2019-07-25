using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionType {FROM_THIS_TASK, TO_THIS_TASK}

public class Transition : MonoBehaviour {

	public TransitionType TypeOfTransition;

	[HideInInspector]
	public AITaskManager MasterAI;
    [HideInInspector]
    public Goal ParentGoal;

    public TaskType OriginType;
	public Task OriginTask;
	public TaskType TargetType;
	public Task TargetTask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init() {
	}

	public virtual void OnUpdate() {}

	public virtual void OnHit(HitInfo hb) {}

	public virtual void OnSight(Observable o) {}

    public virtual void OnExitZone(Zone z) { }

    public virtual void OnEnterZone(Zone z) { }

    public virtual void OnTime() { }

    public virtual void OnStart() { }

    public void TriggerTransition() {
		if (TypeOfTransition == TransitionType.FROM_THIS_TASK) {
			if (TargetTask != null)
				MasterAI.TransitionToTask (TargetTask);
			else
				MasterAI.TransitionToTask (TargetType);
		} else {
			MasterAI.TransitionToTask (GetComponent<Task>());
		}
	}
    public virtual void OnLoad(Goal g)
    {

    }

    public virtual void OnSave(Goal g)
    {

    }

}