using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AITaskManager))]
public class AICharacter : MonoBehaviour
{
    AITaskManager m_taskManager;

    public List<Goal> GoalList;
    public Behaviour CurrentBehaviour;

    void Awake()
    {
        m_taskManager =  GetComponent<AITaskManager>();
        ReloadGoals();
    }

    void ReloadGoals()
    {
        Goal[] gList = GetComponentsInChildren<Goal>();
        GoalList = new List<Goal>();

        foreach (Goal g in gList)
        {
            g.SetMasterAI(this);
            GoalList.Add(g);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetBehaviour(GameObject behaviourTemplate)
    {
        m_taskManager.SetBehaviour(behaviourTemplate);
    }

    public void OnHit(HitInfo hb)
    {
        foreach (Goal g in GoalList)
        {
            g.OnHit(hb);
        }
        m_taskManager.OnHit(hb);
    }

    public void OnSight(Observable o)
    {
        foreach (Goal g in GoalList)
        {
            g.OnSight(o);
        }
        m_taskManager.OnSight(o);
    }

    private void OnStart()
    {
        foreach( Goal g in GoalList)
        {
            g.OnStart();
        }
        m_taskManager.OnStart();
    }

    public void OnTime()
    {
        foreach (Goal g in GoalList)
        {
            g.OnTime();
        }
        m_taskManager.OnTime();
    }

    public void OnEnterZone(Zone z)
    {
        foreach (Goal g in GoalList)
        {
            g.OnEnterZone(z);
        }
        m_taskManager.OnEnterZone(z);
    }
    public void OnExitZone(Zone z)
    {
        foreach (Goal g in GoalList)
        {
            g.OnExitZone(z);
        }
        m_taskManager.OnExitZone(z);
    }
    public void ProposeNewBehaviour(Behaviour b)
    {
        if (b.BehaviourPrefab == null)
            return;
        //Debug.Log("Parent: ");
        //Debug.Log(b.ParentGoal);
        //Debug.Log(b.PriorityScore);
        if (CurrentBehaviour == null)
        {
            SetBehaviour(b.BehaviourPrefab);
            return;
        }
        if (b.PriorityScore * b.ParentGoal.GoalPriority > 
            CurrentBehaviour.PriorityScore * CurrentBehaviour.ParentGoal.GoalPriority) {
            SetBehaviour(b.BehaviourPrefab);
        }
    }
}


public class Behaviour
{
    public string BehaviourName;
    public GameObject BehaviourPrefab;
    public Goal ParentGoal;
    public float PriorityScore;
    public Behaviour(string name, GameObject prefab, Goal parentGoal, float score = 1.0f)
    {
        BehaviourName = name;
        BehaviourPrefab = prefab;
        ParentGoal = parentGoal;
        PriorityScore = score;
    }
}