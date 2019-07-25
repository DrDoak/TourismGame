using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Goal : MonoBehaviour
{
    protected AICharacter m_masterAI;

    private float m_goalPriority;

    public virtual float GoalPriority { get { return m_goalPriority; } private set { m_goalPriority = value; } }

    public virtual string GoalName { get { return "DefaultGoal"; } }

    public virtual void OnHit(HitInfo hb) { }

    public virtual void OnSight(Observable o) { }

    public virtual void OnStart() { }

    public virtual void OnTime() { }

    public virtual void OnEnterZone(Zone z) { }

    public virtual void OnExitZone(Zone z) { }

    [SerializeField]
    protected int NumGoalVariables = 0;
    [SerializeField]
    protected StringDictionary GoalVariables;

    public void SetGoalPriority(float newP)
    {
        m_goalPriority = newP;
    }
    public void SetMasterAI(AICharacter master)
    {
        m_masterAI = master;
    }
    
    public string ExportString()
    {
        string s = this.GetType().ToString();
        s = s + "|";
        s += name + "|";
        foreach (string k in GoalVariables.Keys)
        {
            s = s + k + ":" + GoalVariables[k] + "|";
        }
        return s;
    }

    public void InitializeVars(string[] initList)
    {
        if (GoalVariables == null)
            GoalVariables = new StringDictionary();
        name = initList[1];
        foreach (string s in initList)
        {
            var keyValueCombo = s.Split(':');
            if (keyValueCombo.Length > 1)
            {
                GoalVariables.Add(keyValueCombo[0], keyValueCombo[1]);
            }
        }
    }

    void Start()
    {
        init();
    }

    protected virtual void initVariableDictionary() {
        int c = GoalVariables.Keys.Count;
        for (int i = c; i < NumGoalVariables; i++)
        {
            GoalVariables.Add(i.ToString(), "");
        }
    }

    protected virtual void init() {
        if (GoalVariables == null)
            GoalVariables = new StringDictionary();
        initVariableDictionary();
    }

    public void SetVariable(string key, string value, Task origin)
    {
        GoalVariables[origin.GetType() + "-" + origin.ParentGoal.GetType() + "-" + key] = value;
    }
    public string GetVariable(string key, Task origin)
    {
        if (!ContainsKey(key,origin))
            return "";
        return GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key];
    }
    public bool ContainsKey(string key, Task origin)
    {
        return GoalVariables.ContainsKey(origin.GetType() + "-" + origin.ParentGoal.name + "-" + key);
    }
    public void SetVariable(string key, string value, Transition origin)
    {
        GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key] = value;
    }
    public string GetVariable(string key, Transition origin)
    {
        if (!ContainsKey(key, origin))
            return "";
        return GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key];
    }
    public bool ContainsKey(string key, Transition origin)
    {
        return GoalVariables.ContainsKey(origin.GetType() + "-" + origin.ParentGoal.name + "-" + key);
    }
}
