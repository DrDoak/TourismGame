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

    public StringDictionary GoalVariables;

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

    protected virtual void initVariableDictionary() { }

    protected virtual void init() {
        if (GoalVariables == null)
            GoalVariables = new StringDictionary();
        initVariableDictionary();
    }
}
