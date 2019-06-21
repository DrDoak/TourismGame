using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AITaskManager))]
public class AICharacter : MonoBehaviour
{
    AITaskManager m_taskManager;

    public List<Goal> GoalList;
    public AIBehaviour CurrentBehaviour;

    private List<string> m_GoalObjectNames;
    private string m_currentBehaviour;
    void Awake()
    {
        m_taskManager =  GetComponent<AITaskManager>();
        ReloadGoals();
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    private void storeData(CharData d)
    {
        d.PersistentStrings["CurrentBehaviour"] = m_currentBehaviour;
        string goalList = "";
        foreach (Goal g in GoalList)
        {
            goalList += g.PrefabName + "/n";
        }
        d.PersistentStrings["GoalList"] = goalList;
    }

    private void loadData(CharData d)
    {
        SetBehaviour(findBehaviour(d.PersistentStrings["CurrentBehaviour"]));
        string savedItems = d.PersistentStrings["GoalList"];
        var arr = savedItems.Split('\n');
        foreach (string s in arr)
        {
            if (s.Length > 0)
            {
                Instantiate((GameObject)Resources.Load(s));
            }
        }
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

    public void SetBehaviour(GameObject behaviourTemplate)
    {

        m_taskManager.SetBehaviour(behaviourTemplate);
        m_currentBehaviour = behaviourTemplate.name;
    }

    private GameObject findBehaviour(string name)
    {
        foreach (Goal g in GoalList)
        {
            Transform t = g.gameObject.transform.Find(name);
            if (t != null)
                return t.gameObject;
        }
        return null;
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
    public void ProposeNewBehaviour(AIBehaviour b)
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

[System.Serializable]
public class AIBehaviour
{
    public string BehaviourName;
    public string PrefabName;
    public GameObject BehaviourPrefab;
    public Goal ParentGoal;
    public float PriorityScore;
    public List<string> ExtraVars;
    public AIBehaviour(string name, GameObject prefab, Goal parentGoal, float score = 1.0f)
    {
        BehaviourName = name;
        BehaviourPrefab = prefab;
        ParentGoal = parentGoal;
        PriorityScore = score;
    }
}