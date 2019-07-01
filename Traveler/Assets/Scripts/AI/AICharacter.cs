using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AITaskManager))]
public class AICharacter : MonoBehaviour
{
    AITaskManager m_taskManager;

    public List<Goal> GoalList;

    private List<string> m_GoalObjectNames;
    public Goal m_currentGoal;
    private float m_currentPriority;
    private string m_currentBehaviourName;
    void Awake()
    {
        m_taskManager =  GetComponent<AITaskManager>();
        ReloadGoals();
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    private void storeData(CharData d)
    {
        d.PersistentStrings["CurrentBehaviour"] = m_currentBehaviourName;
        if (m_currentGoal != null)
            d.PersistentStrings["CurrentGoal"] = m_currentGoal.gameObject.name;
        else
            d.PersistentStrings["CurrentGoal"] = "none";
        d.PersistentFloats["CurrentBehaviourPriority"] = m_currentPriority;
        string goalList = "";
        foreach (Goal g in GoalList)
        {
            goalList += g.ExportString();
        }
        d.PersistentStrings["GoalList"] = goalList;
        //Debug.Log("Saving item: " + d.PersistentStrings["GoalList"]);
    }

    private void loadData(CharData d)
    {
        
        
        //Debug.Log("Loading a new Character: last goal: " + d.PersistentStrings["CurrentGoal"]);
        string savedItems = d.PersistentStrings["GoalList"];
        var arr = savedItems.Split('\n');
        foreach (string s in arr)
        {
            if (s.Length > 0)
            {
                var goalArr = s.Split('|');
                if (System.Type.GetType(goalArr[0]) != null)
                {
                    GameObject newGoal = Instantiate(ListAIObjects.Instance.GenericGoal,transform);
                    System.Type goalType = System.Type.GetType(goalArr[0]);
                    newGoal.AddComponent(goalType);
                    ((Goal)newGoal.GetComponent(goalType)).InitializeVars(goalArr);
                }
            }
        }
        if (d.PersistentStrings["CurrentGoal"] != "none")
        {
            GameObject g = (GameObject)Resources.Load(d.PersistentStrings["CurrentBehaviour"]);
            Transform t = transform.Find(d.PersistentStrings["CurrentGoal"]);
            if (g != null && t != null)
                SetBehaviour(g, t.gameObject.GetComponent<Goal>(), d.PersistentFloats["CurrentBehaviourPriority"]);
        }
        ReloadGoals();
        OnStart();
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

    private void Update()
    {
        //Debug.Log(m_currentGoal);
    }

    public void SetBehaviour(GameObject g, Goal originGoal, float priorityScore)
    {
        Debug.Log("Setting behaviour to: " + g + " from: " + originGoal);
        m_taskManager.SetBehaviour(g,originGoal);
        m_currentPriority = priorityScore;
        m_currentBehaviourName = g.name;
        m_currentGoal = originGoal;
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
        /*Debug.Log("Parent: ");
        Debug.Log(b.ParentGoal);
        Debug.Log(b.PriorityScore);
        Debug.Log("CurrentGoal: " + m_currentGoal);*/
        if (m_currentGoal == null)
        {
            SetBehaviour(b.BehaviourPrefab,b.ParentGoal,b.PriorityScore);
            return;
        }
        if (b.PriorityScore * b.ParentGoal.GoalPriority >
            m_currentPriority * m_currentGoal.GoalPriority) {
            SetBehaviour(b.BehaviourPrefab, b.ParentGoal,b.PriorityScore);
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
    public AIBehaviour(string name, GameObject prefab, Goal parentGoal, float score = 1.0f)
    {
        BehaviourName = name;
        BehaviourPrefab = prefab;
        ParentGoal = parentGoal;
        PriorityScore = score;
    }
}